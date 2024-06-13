using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
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
    public class OrderListObject : MonoBehaviour
    {
        [SerializeField]
        HyperCasualButton m_BuyButton;
        [SerializeField]
        HyperCasualButton m_CancelButton;
        [SerializeField]
        private TextMeshProUGUI nameText = null;

        [SerializeField]
        private TextMeshProUGUI amountText = null;

        [SerializeField]
        private RawImage collectionImage = null;
        [SerializeField]
        private GameObject progress = null;

        // To remove
        [SerializeField] private GameObject signatureContainer;
        [SerializeField] private InputField orderSignature;
        [SerializeField]
        HyperCasualButton m_ConfirmButton;

        private OrderModel order;
        private string preparedListing;
        private string listingId;

        public async void Initialise(OrderModel order)
        {
            this.order = order;
            if (order.buy.Length > 0)
            {
                string amount = order.buy[0].amount;
                decimal quantity = (decimal)BigInteger.Parse(amount) / (decimal)BigInteger.Pow(10, 18);
                amountText.text = $"Amount: {quantity} IMR";
            }

            // Check is it's the user's listing
            string address = await GetWalletAddress();
            bool IsUsersListing = order.account_address == address;
            m_BuyButton.gameObject.SetActive(!IsUsersListing);
            m_CancelButton.gameObject.SetActive(IsUsersListing);

            // Get NFT
            await GetNft(order.sell[0].token_id);

            // Hide progress and signature input
            progress.SetActive(false);
            signatureContainer.gameObject.SetActive(false);

            m_BuyButton.AddListener(OnBuyButtonClick);
            m_ConfirmButton.AddListener(ConfirmSell);
            m_CancelButton.AddListener(OnCancel);
        }

        private async UniTask GetNft(string tokenId)
        {
            try
            {
                string skinContractAddress = "0x52A1016eCca06bDBbdd9440E7AA9166bD5366aE1";

                using var client = new HttpClient();
                string url = $"https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/collections/{skinContractAddress}/nfts/{tokenId}";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    TokenResponse tokenResponse = JsonUtility.FromJson<TokenResponse>(responseBody);
                    if (tokenResponse != null && tokenResponse.result != null)
                    {
                        nameText.text = $"Name: {tokenResponse.result.name}";
                        if (tokenResponse.result.image != null)
                        {
                            StartCoroutine(DownloadImage(tokenResponse.result.image));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to get NFT: {ex.Message}");
            }
        }

        private async UniTask<string> GetWalletAddress()
        {
            List<string> accounts = await Passport.Instance.ZkEvmRequestAccounts();
            return accounts[0]; // Get the first wallet address
        }

        private async void OnBuyButtonClick()
        {
            try
            {
                m_BuyButton.gameObject.SetActive(false);
                progress.SetActive(true);
                signatureContainer.gameObject.SetActive(false);

                string address = await GetWalletAddress();
                var nvc = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("fulfillerAddress", address),
                    new KeyValuePair<string, string>("listingId", order.id),
                    new KeyValuePair<string, string>("fees", order.fees.ToJson().Replace("recipient_address","recipientAddress")) // TODO
                };
                using var client = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:6060/fillOrder/skin") { Content = new FormUrlEncodedContent(nvc) };
                using var res = await client.SendAsync(req);

                string responseBody = await res.Content.ReadAsStringAsync();
                Debug.Log($"responseBody: {responseBody}");
                FulfullOrderResponse response = JsonUtility.FromJson<FulfullOrderResponse>(responseBody);

                // One transaction to call Immutable Runner Token approve 095ea7b3
                // One to call Immutable Seaport 0x7d117aa8bd6d31c4fa91722f246388f38ab1942c fulfillAvailableAdvancedOrders (87201b41)

                if (response.transactionsToSend != null)
                {
                    foreach (TransactionToSend tx in response.transactionsToSend)
                    {
                        string transactionHash = await Passport.Instance.ZkEvmSendTransaction(new TransactionRequest()
                        {
                            to = tx.to,
                            data = tx.data,
                            value = "0"
                        });
                    }

                    m_BuyButton.gameObject.SetActive(false);
                }
                else
                {
                    m_BuyButton.gameObject.SetActive(true);
                }

                progress.SetActive(false);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to sell {ex.Message}");
                m_BuyButton.gameObject.SetActive(true);
                progress.SetActive(false);
                signatureContainer.gameObject.SetActive(false);
            }
        }

        private async void ConfirmSell()
        {
            try
            {
                m_BuyButton.gameObject.SetActive(false);
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

                    m_BuyButton.gameObject.SetActive(false);
                    progress.SetActive(false);
                }
                else
                {
                    Debug.Log($"Failed to confirm sell");
                    m_BuyButton.gameObject.SetActive(true);
                    progress.SetActive(false);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to confirm sell: {ex.Message}");
                m_BuyButton.gameObject.SetActive(true);
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
                new KeyValuePair<string, string>("listingId", order.id),
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