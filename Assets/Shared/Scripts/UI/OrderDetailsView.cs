using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using Immutable.Passport;
using TMPro;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Immutable.Passport.Model;

namespace HyperCasual.Runner
{
    public class OrderDetailsView : View
    {
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private BalanceObject m_Balance;
        [SerializeField] private TextMeshProUGUI m_NameText = null;
        [SerializeField] private TextMeshProUGUI m_TokenIdText = null;
        [SerializeField] private TextMeshProUGUI m_CollectionText = null;
        [SerializeField] private TextMeshProUGUI m_AmountText = null;
        [SerializeField] private Transform m_AttributesListParent = null;
        [SerializeField] private AttributeView m_AttributeObj = null;
        private List<AttributeView> m_Attributes = new List<AttributeView>();

        [SerializeField] private ImageUrlObject m_Image = null;
        [SerializeField] private HyperCasualButton m_BuyButton;
        [SerializeField] private TextMeshProUGUI m_PlayersListingText = null;
        [SerializeField] private GameObject m_Progress = null;
        [SerializeField] private CustomDialog m_CustomDialog;

        private OrderModel m_Order;
        private Listing m_Listing;

        async void OnEnable()
        {
            m_AttributeObj.gameObject.SetActive(false); // Disable the template attribute object

            m_BackButton.RemoveListener(OnBackButtonClick);
            m_BackButton.AddListener(OnBackButtonClick);

            // Gets the player's balance
            m_Balance.UpdateBalance();
        }

        /// <summary>
        /// Initialises the UI based on the order
        /// </summary>
        public async void Initialise(OrderModel order)
        {
            m_Order = order;
            UpdateData();

            // Check if asset is the player's asset
            string address = await GetWalletAddress();
            bool isPlayersAsset = m_Order.account_address == address;
            if (isPlayersAsset)
            {
                m_PlayersListingText.gameObject.SetActive(true);
                m_BuyButton.gameObject.SetActive(false);
            }
            else
            {
                m_PlayersListingText.gameObject.SetActive(false);

                // Fetch sale status
                bool isOnSale = await IsListed(m_Order.asset.token_id);
                m_BuyButton.gameObject.SetActive(!isOnSale);
            }

            // Hide progress
            m_Progress.SetActive(false);

            // Set listeners to button
            m_BuyButton.RemoveListener(OnBuyButtonClick);
            m_BuyButton.AddListener(OnBuyButtonClick);
        }

        /// <summary>
        /// Updates the text fields with asset data.
        /// </summary>
        private async void UpdateData()
        {
            // Get and display asset details
            await GetDetails(m_Order.sell[0].token_id);

            m_NameText.text = m_Order.asset.name;
            m_TokenIdText.text = $"Token ID: {m_Order.asset.token_id}";
            m_CollectionText.text = $"Collection: {m_Order.asset.contract_address}";

            // Price
            if (m_Order.buy.Length > 0)
            {
                string amount = m_Order.buy[0].amount;
                decimal quantity = (decimal)BigInteger.Parse(amount) / (decimal)BigInteger.Pow(10, 18);
                m_AmountText.text = $"{quantity} IMR";
            }

            // Clears all existing attributes
            ClearAttributes();

            // Populate attributes
            foreach (AssetAttribute attribute in m_Order.asset.attributes)
            {
                AttributeView newAttribute = Instantiate(m_AttributeObj, m_AttributesListParent); // Create a new asset object
                newAttribute.gameObject.SetActive(true);
                newAttribute.Initialise(attribute); // Initialise the view with data
                m_Attributes.Add(newAttribute); // Add to the list of displayed attributes
            }

            // Download and display the image
            m_Image.LoadUrl(m_Order.asset.image);
        }

        /// <summary>
        /// Removes all the attribute views
        /// </summary>
        private void ClearAttributes()
        {
            foreach (AttributeView attribute in m_Attributes)
            {
                Destroy(attribute.gameObject);
            }
            m_Attributes.Clear();
        }

        /// <summary>
        /// Fetches asset details for the given token ID.
        /// </summary>
        private async UniTask GetDetails(string tokenId)
        {
            try
            {
                using var client = new HttpClient();
                string url = $"https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/collections/{Contract.SKIN}/nfts/{tokenId}";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    AssetResponse assetResponse = JsonUtility.FromJson<AssetResponse>(responseBody);
                    if (assetResponse?.result != null)
                    {
                        m_Order.asset = assetResponse.result;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get details: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if the asset is listed for sale.
        /// </summary>
        /// <param name="tokenId">The token ID of the asset.</param>
        /// <returns>True if the asset is listed, otherwise false.</returns>
        private async UniTask<bool> IsListed(string tokenId)
        {
            try
            {
                using var client = new HttpClient();
                string url = $"https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/orders/listings?sell_item_contract_address={Contract.SKIN}&sell_item_token_id={tokenId}&status=ACTIVE";

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ListingsResponse listingsResponse = JsonUtility.FromJson<ListingsResponse>(responseBody);

                    if (listingsResponse.result.Length > 0)
                    {
                        m_Listing = listingsResponse.result[0];
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to check sale status: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Retrieves the wallet address of the user.
        /// </summary>
        /// <returns>The wallet address.</returns>
        private async UniTask<string> GetWalletAddress()
        {
            List<string> accounts = await Passport.Instance.ZkEvmRequestAccounts();
            return accounts[0]; // Get the first wallet address
        }

        /// <summary>
        /// Handles the buy button click event. Sends a request to fulfil an order, 
        /// processes the response, and updates the UI accordingly.
        /// </summary>
        private async void OnBuyButtonClick()
        {
            try
            {
                m_BuyButton.gameObject.SetActive(false);
                m_Progress.SetActive(true);

                string address = await GetWalletAddress();
                var nvc = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("fulfillerAddress", address),
                    new KeyValuePair<string, string>("listingId", m_Order.id),
                    new KeyValuePair<string, string>("fees", m_Order.fees.ToJson().Replace("recipient_address", "recipientAddress"))
                };

                using var client = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Post, "http://localhost:6060/fillOrder/skin")
                {
                    Content = new FormUrlEncodedContent(nvc)
                };
                using var res = await client.SendAsync(req);

                string responseBody = await res.Content.ReadAsStringAsync();
                FulfullOrderResponse response = JsonUtility.FromJson<FulfullOrderResponse>(responseBody);
                if (response.transactionsToSend != null)
                {
                    foreach (TransactionToSend tx in response.transactionsToSend)
                    {
                        string transactionHash = await Passport.Instance.ZkEvmSendTransaction(new TransactionRequest
                        {
                            to = tx.to, // Immutable seaport contract
                            data = tx.data, // 87201b41 fulfillAvailableAdvancedOrders
                            value = "0"
                        });
                    }

                    // Validate that order is fulfilled
                    await ConfirmListingStatus();
                    m_BuyButton.gameObject.SetActive(false);
                    m_PlayersListingText.gameObject.SetActive(true);
                    m_Balance.UpdateBalance(); // Update user's balance on successful buy
                }
                else
                {
                    m_BuyButton.gameObject.SetActive(true);
                }

                m_Progress.SetActive(false);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to buy: {ex.Message}");
                m_BuyButton.gameObject.SetActive(true);
                m_Progress.SetActive(false);
                await m_CustomDialog.ShowDialog("Error", "Failed to buy", "OK");
            }
        }

        /// <summary>
        /// Polls the order status until it transitions to FULFILLED or the operation times out after 1 minute.
        /// </summary>
        private async UniTask ConfirmListingStatus()
        {
            Debug.Log($"Confirming order is filled...");

            bool conditionMet = await PollingHelper.PollAsync(
                $"https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/orders/listings/{m_Order.id}",
                (responseBody) =>
                {
                    ListingResponse listingResponse = JsonUtility.FromJson<ListingResponse>(responseBody);
                    return listingResponse.result?.status.name == "FILLED";
                });

            if (conditionMet)
            {
                Debug.Log($"Order confirmed as filled.");
            }
            else
            {
                await m_CustomDialog.ShowDialog("Error", $"Failed to confirm if order is filled", "OK");
            }
        }

        /// <summary>
        /// Handles the back button click
        /// </summary>
        private void OnBackButtonClick()
        {
            UIManager.Instance.GoBack();
        }

        /// <summary>
        /// Cleans up data
        /// </summary>
        private void OnDisable()
        {
            m_NameText.text = "";
            m_TokenIdText.text = "";
            m_CollectionText.text = "";

            m_Order = null;
            ClearAttributes();
        }
    }
}