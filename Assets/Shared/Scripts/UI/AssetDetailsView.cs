using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
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
    public class AssetDetailsView : View
    {
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private AbstractGameEvent m_BackEvent;
        [SerializeField] private TextMeshProUGUI m_NameText = null;
        [SerializeField] private TextMeshProUGUI m_TokenIdText = null;
        [SerializeField] private TextMeshProUGUI m_CollectionText = null;
        [SerializeField] private TextMeshProUGUI m_StatusText = null;
        [SerializeField] private Transform m_AttributesListParent = null;
        [SerializeField] private AttributeView m_AttributeObj = null;
        private List<AttributeView> m_Attributes = new List<AttributeView>();

        [SerializeField] private RawImage m_Image = null;
        [SerializeField] HyperCasualButton m_SellButton;
        [SerializeField] HyperCasualButton m_CancelButton;
        [SerializeField] private GameObject m_Progress = null;

        // To remove
        [SerializeField] private GameObject signatureContainer;
        [SerializeField] private InputField orderSignature;
        [SerializeField]
        HyperCasualButton m_ConfirmButton;

        private AssetModel m_Asset;
        private string preparedListing;
        private string listingId;

        async void OnEnable()
        {
            m_AttributeObj.gameObject.SetActive(false); // Disable the template attribute object
            m_BackButton.AddListener(OnBackButtonClick); // Listen for back button click
        }

        /// <summary>
        /// Initialises the UI based on the asset
        /// </summary>
        public async void Initialise(AssetModel asset)
        {
            m_Asset = asset;
            UpdateData();

            // Fetch sale status
            bool isOnSale = await IsListed(m_Asset.token_id);
            m_StatusText.text = isOnSale ? "Listed" : "Not listed";
            m_SellButton.gameObject.SetActive(!isOnSale);
            m_CancelButton.gameObject.SetActive(isOnSale);

            // Hide progress and signature input
            m_Progress.SetActive(false);
            signatureContainer.gameObject.SetActive(false);

            // Download and display the image
            if (!string.IsNullOrEmpty(m_Asset.image))
            {
                await DownloadImage(m_Asset.image);
            }

            // Set listeners to buttons
            m_SellButton.AddListener(OnSellButtonClick);
            m_ConfirmButton.AddListener(ConfirmSell);
            m_CancelButton.AddListener(OnCancel);
        }

        /// <summary>
        /// Updates the text fields with asset data.
        /// </summary>
        private void UpdateData()
        {
            m_NameText.text = m_Asset.name;
            m_TokenIdText.text = $"Token ID: {m_Asset.token_id}";
            m_CollectionText.text = $"Collection: {m_Asset.contract_address}";

            // Clears all currently listed attributes.
            foreach (AttributeView attribute in m_Attributes)
            {
                Destroy(attribute.gameObject);
            }
            m_Attributes.Clear();

            // Populate attributes
            foreach (AssetAttribute attribute in m_Asset.attributes)
            {
                AttributeView newAttribute = Instantiate(m_AttributeObj, m_AttributesListParent); // Create a new asset object
                newAttribute.gameObject.SetActive(true);
                newAttribute.Initialise(attribute); // Initialise the view with data
                m_Attributes.Add(newAttribute); // Add to the list of displayed attributes
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
                    ListingResponse listingResponse = JsonUtility.FromJson<ListingResponse>(responseBody);

                    // Check if the listing exists
                    return listingResponse.result.Length > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to check sale status: {ex.Message}");
            }

            return false;
        }

        private async UniTask<string> GetWalletAddress()
        {
            List<string> accounts = await Passport.Instance.ZkEvmRequestAccounts();
            return accounts[0]; // Get the first wallet address
        }

        private async void OnSellButtonClick()
        {
            try
            {
                m_SellButton.gameObject.SetActive(false);
                m_Progress.SetActive(true);
                signatureContainer.gameObject.SetActive(false);

                string address = await GetWalletAddress();
                var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("offererAddress", address),
                new KeyValuePair<string, string>("amount", "9000000000000000000"), // TODO allow user to enter any amount
                new KeyValuePair<string, string>("tokenId", m_Asset.token_id)
            };
                using var client = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:6060/prepareListing/skin") { Content = new FormUrlEncodedContent(nvc) };
                using var res = await client.SendAsync(req);

                string responseBody = await res.Content.ReadAsStringAsync();
                PrepareListingResponse response = JsonUtility.FromJson<PrepareListingResponse>(responseBody);

                // Approved the Immutable Seaport contract to transfer assets from skin collection on their behalf they'll need to do so before they create an order
                if (response.transactionToSend != null && response.transactionToSend.to != null)
                {
                    string transactionHash = await Passport.Instance.ZkEvmSendTransaction(new TransactionRequest()
                    {
                        to = response.transactionToSend.to,
                        data = response.transactionToSend.data, // a22cb465 setApprovalForAll
                        value = "0"
                    });
                }

                if (response.toSign != null && response.preparedListing != null)
                {
                    preparedListing = response.preparedListing;
                    signatureContainer.gameObject.SetActive(true);
                    Debug.Log($"Sign: {response.toSign}");
                }
                else
                {
                    m_SellButton.gameObject.SetActive(true);
                }
                m_Progress.SetActive(false);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to sell {ex.Message}");
                m_SellButton.gameObject.SetActive(true);
                m_Progress.SetActive(false);
                signatureContainer.gameObject.SetActive(false);
            }
        }

        private async void ConfirmSell()
        {
            try
            {
                m_SellButton.gameObject.SetActive(false);
                m_Progress.SetActive(true);
                signatureContainer.gameObject.SetActive(false);

                string address = await GetWalletAddress();
                var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("signature", orderSignature.text),
                new KeyValuePair<string, string>("preparedListing", preparedListing),
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
                    m_Progress.SetActive(false);
                    m_CancelButton.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log($"Failed to confirm sell");
                    m_SellButton.gameObject.SetActive(true);
                    m_Progress.SetActive(false);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to confirm sell: {ex.Message}");
                m_SellButton.gameObject.SetActive(true);
                m_Progress.SetActive(false);
            }
        }

        private async void OnCancel()
        {
            try
            {
                m_CancelButton.gameObject.SetActive(false);
                m_Progress.SetActive(true);

                string address = await GetWalletAddress();
                var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("offererAddress", address),
                new KeyValuePair<string, string>("listingId", listingId),
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
                }

                m_SellButton.gameObject.SetActive(true);
                m_Progress.SetActive(false);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to cancel {ex.Message}");
                m_CancelButton.gameObject.SetActive(true);
                m_Progress.SetActive(false);
            }
        }

        /// <summary>
        /// Downloads the image from the given URL and displays it.
        /// </summary>
        private async UniTask DownloadImage(string url)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    m_Image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    m_Image.gameObject.SetActive(true);
                }
                else
                {
                    m_Image.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Handles the back button click
        /// </summary>
        private void OnBackButtonClick()
        {
            m_BackEvent.Raise();
        }

        /// <summary>
        /// Cleans up event listeners and data
        /// </summary>
        private void OnDisable()
        {
            // Remove listener from the back button
            m_BackButton.RemoveListener(OnBackButtonClick);

            m_NameText.text = "";
            m_TokenIdText.text = "";
            m_CollectionText.text = "";
            m_StatusText.text = "";
            m_Image.texture = null;

            m_Asset = null;
        }
    }
}