using System;
using System.Collections.Generic;
using System.Numerics;
using System.Net.Http;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Immutable.Passport;
using Immutable.Passport.Model;

namespace HyperCasual.Runner
{
    public class AssetDetailsView : View
    {
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private BalanceObject m_Balance;
        [SerializeField] private RawImage m_Image;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_TokenIdText;
        [SerializeField] private TextMeshProUGUI m_CollectionText;
        [SerializeField] private TextMeshProUGUI m_StatusText;
        [SerializeField] private Transform m_AttributesListParent;
        [SerializeField] private AttributeView m_AttributeObj;
        [SerializeField] private HyperCasualButton m_SellButton;
        [SerializeField] private HyperCasualButton m_CancelButton;
        [SerializeField] private GameObject m_Progress;
        [SerializeField] private CustomDialog m_CustomDialog;

        private List<AttributeView> m_Attributes = new List<AttributeView>();
        private AssetModel m_Asset;
        private Listing m_Listing;

        private void OnEnable()
        {
            m_AttributeObj.gameObject.SetActive(false); // Disable the template attribute object

            m_BackButton.RemoveListener(OnBackButtonClick);
            m_BackButton.AddListener(OnBackButtonClick);

            // Gets the player's balance
            m_Balance.UpdateBalance();
        }

        /// <summary>
        /// Initialises the UI based on the asset.
        /// </summary>
        /// <param name="asset">The asset to display.</param>
        public async void Initialise(AssetModel asset)
        {
            m_Asset = asset;
            UpdateData();

            m_Progress.SetActive(false); // Hide progress

            m_SellButton.RemoveListener(OnSellButtonClicked);
            m_SellButton.AddListener(OnSellButtonClicked);
            m_CancelButton.RemoveListener(OnCancelButtonClicked);
            m_CancelButton.AddListener(OnCancelButtonClicked);
        }

        /// <summary>
        /// Updates the UI with the asset's details.
        /// </summary>
        private async void UpdateData()
        {
            m_NameText.text = m_Asset.name;
            m_TokenIdText.text = $"Token ID: {m_Asset.token_id}";
            m_CollectionText.text = $"Collection: {m_Asset.contract_address}";

            // Clear existing attributes
            ClearAttributes();

            // Populate attributes
            foreach (AssetAttribute attribute in m_Asset.attributes)
            {
                AttributeView newAttribute = Instantiate(m_AttributeObj, m_AttributesListParent);
                newAttribute.gameObject.SetActive(true);
                newAttribute.Initialise(attribute);
                m_Attributes.Add(newAttribute);
            }

            // Update sale status
            bool isOnSale = await IsListed(m_Asset.token_id);
            m_StatusText.text = isOnSale ? "Listed" : "Not listed";
            m_SellButton.gameObject.SetActive(!isOnSale);
            m_CancelButton.gameObject.SetActive(isOnSale);

            // Download and display the image
            if (!string.IsNullOrEmpty(m_Asset.image))
            {
                await DownloadImage(m_Asset.image);
            }
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
                    ListingResponse listingResponse = JsonUtility.FromJson<ListingResponse>(responseBody);

                    if (listingResponse.result.Length > 0)
                    {
                        m_Listing = listingResponse.result[0];
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
        /// Handles the click event for the sell button.
        /// </summary>
        private async void OnSellButtonClicked()
        {
            (bool result, string price) = await m_CustomDialog.ShowDialog(
                $"List {m_Asset.name} for sale",
                "Enter your price below (in IMR):",
                "Confirm",
                negativeButtonText: "Cancel",
                showInputField: true
            );

            if (result)
            {
                decimal amount = Math.Floor(decimal.Parse(price) * (decimal)BigInteger.Pow(10, 18));
                await PrepareListing($"{amount}");
            }
        }

        /// <summary>
        /// Prepares the listing for the asset.
        /// </summary>
        /// <param name="price">The price of the asset in smallest unit.</param>
        private async UniTask PrepareListing(string price)
        {
            try
            {
                m_SellButton.gameObject.SetActive(false);
                m_Progress.SetActive(true);

                string address = await GetWalletAddress();
                var nvc = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("offererAddress", address),
                    new KeyValuePair<string, string>("amount", price),
                    new KeyValuePair<string, string>("tokenId", m_Asset.token_id)
                };
                using var client = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:6060/prepareListing/skin") { Content = new FormUrlEncodedContent(nvc) };
                using var res = await client.SendAsync(req);

                string responseBody = await res.Content.ReadAsStringAsync();
                PrepareListingResponse response = JsonUtility.FromJson<PrepareListingResponse>(responseBody);

                if (response.transactionToSend?.to != null)
                {
                    Debug.Log($"Transaction to: {response.transactionToSend.to}");
                    Debug.Log($"Transaction data: {response.transactionToSend.data}");
                    string transactionHash = await Passport.Instance.ZkEvmSendTransaction(new TransactionRequest
                    {
                        to = response.transactionToSend.to,
                        data = response.transactionToSend.data,
                        value = "0"
                    });
                }

                if (response.toSign != null && response.preparedListing != null)
                {
                    // Prompt for signature
                    Debug.Log($"Sign: {response.toSign}");
                    (bool result, string signature) = await m_CustomDialog.ShowDialog(
                        "Confirm listing",
                        "Enter signed payload:",
                        "Confirm",
                        negativeButtonText: "Cancel",
                        showInputField: true
                    );
                    if (result)
                    {
                        await ListAsset(signature, response.preparedListing, address);
                    }
                    else
                    {
                        m_SellButton.gameObject.SetActive(true);
                    }
                }
                else
                {
                    m_SellButton.gameObject.SetActive(true);
                }
                m_Progress.SetActive(false);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to sell: {ex.Message}");
                m_SellButton.gameObject.SetActive(true);
                m_Progress.SetActive(false);
                await m_CustomDialog.ShowDialog("Error", "Failed to prepare listing", "OK");
            }
        }

        /// <summary>
        /// Finalises the listing of the asset.
        /// </summary>
        /// <param name="signature">The signature for the listing.</param>
        /// <param name="preparedListing">The prepared listing data.</param>
        /// <param name="address">The wallet address of the user.</param>
        private async UniTask ListAsset(string signature, string preparedListing, string address)
        {
            try
            {
                m_SellButton.gameObject.SetActive(false);
                m_Progress.SetActive(true);

                var nvc = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("signature", signature),
                    new KeyValuePair<string, string>("preparedListing", preparedListing)
                };
                using var client = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:6060/createListing/skin") { Content = new FormUrlEncodedContent(nvc) };
                using var res = await client.SendAsync(req);

                if (res.IsSuccessStatusCode)
                {
                    string responseBody = await res.Content.ReadAsStringAsync();
                    CreateListingResponse response = JsonUtility.FromJson<CreateListingResponse>(responseBody);
                    Debug.Log($"Listing ID: {response.result.id}");

                    m_SellButton.gameObject.SetActive(false);
                    m_CancelButton.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("Failed to confirm sell");
                    m_SellButton.gameObject.SetActive(true);
                }

                m_Progress.SetActive(false);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to list: {ex.Message}");
                m_SellButton.gameObject.SetActive(true);
                m_Progress.SetActive(false);
                await m_CustomDialog.ShowDialog("Error", "Failed to list", "OK");
            }
        }

        /// <summary>
        /// Cancels the listing of the asset.
        /// </summary>
        private async void OnCancelButtonClicked()
        {
            try
            {
                m_CancelButton.gameObject.SetActive(false);
                m_Progress.SetActive(true);

                string address = await GetWalletAddress();
                var nvc = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("offererAddress", address),
                    new KeyValuePair<string, string>("listingId", m_Listing.id),
                    new KeyValuePair<string, string>("type", "hard")
                };

                using var client = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:6060/cancelListing/skin") { Content = new FormUrlEncodedContent(nvc) };
                using var res = await client.SendAsync(req);

                string responseBody = await res.Content.ReadAsStringAsync();
                TransactionToSend response = JsonUtility.FromJson<TransactionToSend>(responseBody);
                if (response != null && response.to != null)
                {
                    string transactionHash = await Passport.Instance.ZkEvmSendTransaction(new TransactionRequest()
                    {
                        to = response.to, // Immutable seaport contract
                        data = response.data, // fd9f1e10 cancel
                        value = "0"
                    });

                    m_SellButton.gameObject.SetActive(true);
                }
                else
                {
                    m_SellButton.gameObject.SetActive(true);

                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to cancel: {ex.Message}");
                m_CancelButton.gameObject.SetActive(true);
                await m_CustomDialog.ShowDialog("Error", "Failed to cancel listing", "OK");
            }

            m_Progress.SetActive(false);
        }

        private void OnBackButtonClick()
        {
            UIManager.Instance.GoBack();
        }

        /// <summary>
        /// Downloads and displays the asset's image.
        /// </summary>
        /// <param name="url">The URL of the image.</param>
        private async UniTask DownloadImage(string url)
        {
            using var webRequest = UnityWebRequestTexture.GetTexture(url);
            await webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                m_Image.texture = texture;
            }
            else
            {
                Debug.Log($"Failed to download image: {webRequest.error}");
            }
        }

        /// <summary>
        /// Cleans up data
        /// </summary>
        private void OnDisable()
        {
            m_NameText.text = "";
            m_TokenIdText.text = ""; ;
            m_CollectionText.text = ""; ;
            m_StatusText.text = ""; ;
            m_Image.texture = null;

            m_Asset = null;
            ClearAttributes();
        }
    }
}
