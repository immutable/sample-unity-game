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
    public class MarketplaceView : View
    {
        [SerializeField]
        HyperCasualButton m_BackButton;
        [SerializeField]
        AbstractGameEvent m_BackEvent;
        [SerializeField]
        private OrderListObject assetObj = null;
        [SerializeField]
        private Transform listParent = null;
        private List<OrderListObject> listedOrders = new List<OrderListObject>();

        private ApiService Api = new();

        async void OnEnable()
        {
            assetObj.gameObject.SetActive(false);
            m_BackButton.AddListener(OnBackButtonClick);
            if (Passport.Instance != null)
            {
                // Get user skins
                // TODO doesn't support pagingation
                List<OrderModel> assets = await GetOrders();

                // Delete existing list
                foreach (OrderListObject uiAsset in listedOrders)
                {
                    Destroy(uiAsset.gameObject);
                }
                listedOrders.Clear();

                // Populate UI list elements
                foreach (OrderModel asset in assets)
                {
                    OrderListObject newAsset = GameObject.Instantiate(assetObj, listParent);
                    newAsset.gameObject.SetActive(true);

                    newAsset.Initialise(asset);
                    listedOrders.Add(newAsset);
                }
            }
        }

        private async UniTask<string> GetWalletAddress()
        {
            List<string> accounts = await Passport.Instance.ZkEvmRequestAccounts();
            return accounts[0]; // Get the first wallet address
        }

        private async UniTask<List<OrderModel>> GetOrders()
        {
            try
            {
                string address = await GetWalletAddress(); // Get the player's wallet address to mint the fox to
                string skinContractAddress = "0x52A1016eCca06bDBbdd9440E7AA9166bD5366aE1";

                if (address != null)
                {
                    using var client = new HttpClient();
                    string url = $"https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/orders/listings?sell_item_contract_address={skinContractAddress}&page_size=10&status=ACTIVE";
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Debug.Log($"Get listing response: {responseBody}");
                        ListOrderResponse listOrderResponse = JsonUtility.FromJson<ListOrderResponse>(responseBody);
                        List<OrderModel> orders = new List<OrderModel>();
                        orders.AddRange(listOrderResponse.result);
                        return orders;
                    }
                    else
                    {
                        return new List<OrderModel>();
                    }
                }

                return new List<OrderModel>();
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to get orders: {ex.Message}");
                return new List<OrderModel>();
            }
        }

        void OnDisable()
        {
            m_BackButton.RemoveListener(OnBackButtonClick);
            foreach (OrderListObject uiAsset in listedOrders)
            {
                Destroy(uiAsset.gameObject);
            }
            listedOrders.Clear();
        }

        void OnBackButtonClick()
        {
            m_BackEvent.Raise();
        }
    }
}