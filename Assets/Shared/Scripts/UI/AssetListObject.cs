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
    public class AssetListObject : MonoBehaviour
    {
        [SerializeField]
        HyperCasualButton m_SellButton;
        [SerializeField]
        HyperCasualButton m_CancelButton;
        [SerializeField]
        private TextMeshProUGUI nameText = null;

        [SerializeField]
        private TextMeshProUGUI tokenIdText = null;

        [SerializeField]
        private TextMeshProUGUI collectionText = null;

        [SerializeField]
        private RawImage collectionImage = null;
        [SerializeField]
        private GameObject progress = null;

        // To remove
        [SerializeField] private GameObject signatureContainer;
        [SerializeField] private InputField orderSignature;
        [SerializeField]
        HyperCasualButton m_ConfirmButton;

        private TokenModel asset;
        private string preparedListing;
        private string listingId;

        /// <summary>
        /// Initialises the ui asset based on AssetWithOrders object
        /// </summary>
        /// <param name="asset">Immutable X asset</param>
        public async void Initialise(TokenModel asset)
        {
            this.asset = asset;
            nameText.text = $"Name: {asset.name}";
            tokenIdText.text = $"Token ID: {asset.token_id}";
            collectionText.text = $"Contract: {asset.contract_address}";

            // Check sale status and show appropriate button
            bool isOnSale = await IsOnSale(asset.token_id);
            m_SellButton.gameObject.SetActive(!isOnSale);
            m_CancelButton.gameObject.SetActive(isOnSale);

            // Hide progress and signature input
            progress.SetActive(false);
            signatureContainer.gameObject.SetActive(false);

            if (asset.image != null)
            {
                StartCoroutine(DownloadImage(asset.image));
            }
            m_SellButton.AddListener(OnSellButtonClick);
            m_ConfirmButton.AddListener(ConfirmSell);
            m_CancelButton.AddListener(OnCancel);
        }

        private async UniTask<bool> IsOnSale(string tokenId)
        {
            Debug.Log("Getting user skins fox...");
            try
            {
                string skinContractAddress = "0x52A1016eCca06bDBbdd9440E7AA9166bD5366aE1";

                using var client = new HttpClient();
                string url = $"https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/orders/listings?sell_item_contract_address={skinContractAddress}&sell_item_token_id={tokenId}&status=ACTIVE";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ListingResponse listingResponse = JsonUtility.FromJson<ListingResponse>(responseBody);
                    if (listingResponse.result.Length > 0)
                    {
                        listingId = listingResponse.result[0].id;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to check sale status: {ex.Message}");
                return false;
            }
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
                progress.SetActive(true);
                signatureContainer.gameObject.SetActive(false);

                string address = await GetWalletAddress();
                var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("offererAddress", address),
                new KeyValuePair<string, string>("amount", "9000000000000000000"), // TODO allow user to enter any amount
                new KeyValuePair<string, string>("tokenId", asset.token_id)
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
                progress.SetActive(false);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to sell {ex.Message}");
                m_SellButton.gameObject.SetActive(true);
                progress.SetActive(false);
                signatureContainer.gameObject.SetActive(false);
            }
        }

        private async void ConfirmSell()
        {
            try
            {
                m_SellButton.gameObject.SetActive(false);
                progress.SetActive(true);
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
                    progress.SetActive(false);
                    m_CancelButton.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log($"Failed to confirm sell");
                    m_SellButton.gameObject.SetActive(true);
                    progress.SetActive(false);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to confirm sell: {ex.Message}");
                m_SellButton.gameObject.SetActive(true);
                progress.SetActive(false);
            }
        }

        private async void OnCancel()
        {
            try
            {
                m_CancelButton.gameObject.SetActive(false);
                progress.SetActive(true);

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
                progress.SetActive(false);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to cancel {ex.Message}");
                m_CancelButton.gameObject.SetActive(true);
                progress.SetActive(false);
            }
        }

        IEnumerator DownloadImage(string MediaUrl)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);

            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                collectionImage.gameObject.SetActive(false);
            }
            else
            {
                collectionImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            }
        }
    }
}

[Serializable]
public class PrepareListingResponse
{
    public TransactionToSend transactionToSend;
    public string toSign;
    public string preparedListing;
}

[Serializable]
public class TransactionToSend
{
    public string to;
    public string data;
}

[Serializable]
public class CreateListingResponse
{
    public Listing result;
}

[Serializable]
public class ListingResponse
{
    public Listing[] result;
}

[Serializable]
public class Listing
{
    public string id;
}