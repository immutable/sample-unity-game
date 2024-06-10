using System;
using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using Immutable.Passport;
using Cysharp.Threading.Tasks;
using System.Net.Http;

namespace HyperCasual.Runner
{
    public class InventoryView : View
    {
        [SerializeField]
        HyperCasualButton m_BackButton;
        [SerializeField]
        private AssetListObject assetObj = null;
        [SerializeField]
        private Transform listParent = null;
        private List<AssetListObject> listedAssets = new List<AssetListObject>();

        private ApiService Api = new();

        async void OnEnable()
        {
            assetObj.gameObject.SetActive(false);
            m_BackButton.AddListener(OnBackButtonClick);
            if (Passport.Instance != null)
            {
                // Get user skins
                // TODO doesn't support pagingation
                List<TokenModel> assets = await GetUserSkins();

                // Delete existing list
                foreach (AssetListObject uiAsset in listedAssets)
                {
                    Destroy(uiAsset.gameObject);
                }
                listedAssets.Clear();

                // Populate UI list elements
                foreach (TokenModel asset in assets)
                {
                    AssetListObject newAsset = GameObject.Instantiate(assetObj, listParent);
                    newAsset.gameObject.SetActive(true);

                    newAsset.Initialise(asset);
                    listedAssets.Add(newAsset);
                }
            }
        }

        private async UniTask<string> GetWalletAddress()
        {
            List<string> accounts = await Passport.Instance.ZkEvmRequestAccounts();
            return accounts[0]; // Get the first wallet address
        }

        private async UniTask<List<TokenModel>> GetUserSkins()
        {
            Debug.Log("Getting user skins fox...");
            try
            {
                string address = await GetWalletAddress(); // Get the player's wallet address to mint the fox to
                string skinContractAddress = "0x52A1016eCca06bDBbdd9440E7AA9166bD5366aE1";

                if (address != null)
                {
                    using var client = new HttpClient();
                    string url = $"https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/accounts/{address}/nfts?contract_address={skinContractAddress}&page_size=10";
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Debug.Log($"Get Tokens response: {responseBody}");
                        ListTokenResponse tokenResponse = JsonUtility.FromJson<ListTokenResponse>(responseBody);
                        List<TokenModel> tokens = new List<TokenModel>();
                        tokens.AddRange(tokenResponse.result);
                        return tokens;
                    }
                    else
                    {
                        return new List<TokenModel>();
                    }
                }

                return new List<TokenModel>();
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to get user's skin: {ex.Message}");
                return new List<TokenModel>();
            }
        }

        void OnDisable()
        {
            m_BackButton.RemoveListener(OnBackButtonClick);
            foreach (AssetListObject uiAsset in listedAssets)
            {
                Destroy(uiAsset.gameObject);
            }
            listedAssets.Clear();
        }

        void OnBackButtonClick()
        {
            UIManager.Instance.GoBack();
        }
    }
}

[Serializable]
public class ListTokenResponse
{
    public List<TokenModel> result;
}

[Serializable]
public class TokenModel
{
    public string token_id;
    public string image;
    public string name;
    public string contract_address;
}