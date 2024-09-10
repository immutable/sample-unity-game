using System;
using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using HyperCasual.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Immutable.Passport;
using Cysharp.Threading.Tasks;
using System.Net.Http;
using TMPro;
using Immutable.Search.Client;
using Immutable.Search.Model;
using Immutable.Search.Api;

namespace HyperCasual.Runner
{
    /// <summary>
    /// The inventory view which displays the player's assets (e.g. skins).
    /// </summary>
    public class InventoryScreen : View
    {
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private AbstractGameEvent m_BackEvent;
        [SerializeField] private BalanceObject m_Balance;
        [SerializeField] private AssetListObject m_AssetObj = null;
        [SerializeField] private Transform m_ListParent = null;
        [SerializeField] private InfiniteScrollView m_ScrollView;
        private List<StackBundle> m_Assets = new List<StackBundle>();

        // Pagination
        private bool m_IsLoadingMore = false;
        private Page m_Page;

        /// <summary>
        /// Sets up the inventory list and fetches the player's assets.
        /// </summary>
        private async void OnEnable()
        {
            // Hide asset template item
            m_AssetObj.gameObject.SetActive(false);

            m_BackButton.RemoveListener(OnBackButtonClick);
            m_BackButton.AddListener(OnBackButtonClick);

            if (Passport.Instance != null)
            {
                // Setup infinite scroll view and load assets
                m_ScrollView.OnCreateItemView += OnCreateItemView;
                LoadAssets();

                // Gets the player's balance
                m_Balance.UpdateBalance();
            }
        }

        /// <summary>
        /// Configures the asset list item view
        /// </summary>
        private void OnCreateItemView(int index, GameObject item)
        {
            if (index < m_Assets.Count)
            {
                // AssetModel asset = m_Assets[index];
                StackBundle asset = m_Assets[index];

                // Initialise the view with asset
                var itemComponent = item.GetComponent<AssetListObject>();
                itemComponent.Initialise(asset);
                // Set up click listener
                var clickable = item.GetComponent<ClickableView>();
                if (clickable != null)
                {
                    clickable.ClearAllSubscribers();
                    clickable.OnClick += () =>
                    {
                        AssetDetailsView view = UIManager.Instance.GetView<AssetDetailsView>();
                        UIManager.Instance.Show(view, true);
                        view.Initialise(asset);
                    };
                }
            }

            // Load more assets if nearing the end of the list
            if (index >= m_Assets.Count - 5 && !m_IsLoadingMore)
            {
                LoadAssets();
            }
        }

        /// <summary>
        /// Loads assets and adds them to the scroll view.
        /// </summary>
        private async void LoadAssets()
        {
            if (m_IsLoadingMore)
            {
                return;
            }

            m_IsLoadingMore = true;

            List<StackBundle> assets = await GetStacks();
            if (assets != null && assets.Count > 0)
            {
                m_Assets.AddRange(assets);
                m_ScrollView.TotalItemCount = m_Assets.Count;
            }

            m_IsLoadingMore = false;
        }

        // Uses mocked stacks endpoint
        private async UniTask<List<StackBundle>> GetStacks()
        {
            Debug.Log("Fetching stacks assets...");

            List<StackBundle> stacks = new List<StackBundle>();

            Configuration config = new Configuration();
            config.BasePath = Config.SEARCH_BASE_URL;
            var apiInstance = new SearchApi(config);

            try
            {
                string address = SaveManager.Instance.WalletAddress;

                if (string.IsNullOrEmpty(address))
                {
                    Debug.LogError("Could not get player's wallet");
                    return stacks;
                }

                string? nextCursor = null;
                if (!string.IsNullOrEmpty(m_Page?.NextCursor))
                {
                    nextCursor = m_Page.NextCursor;
                }
                else if (m_Page != null && (m_Page.NextCursor != null || m_Assets.Count < Config.PAGE_SIZE))
                {
                    Debug.Log("No more assets to load");
                    return stacks;
                }

                SearchStacksResult result = await apiInstance.SearchStacksAsync(Config.CHAIN_NAME, new List<string> { Contract.SKIN }, accountAddress: address, pageCursor: nextCursor, pageSize: 6);
                m_Page = result.Page;
                return result.Result;
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to fetch assets: {ex.Message}");
            }

            // try
            // {
            //     string address = SaveManager.Instance.WalletAddress;

            //     if (string.IsNullOrEmpty(address))
            //     {
            //         Debug.LogError("Could not get player's wallet");
            //         return stacks;
            //     }

            //     string url = $"http://localhost:6060/v1/chains/imtbl-zkevm-testnet/search/stacks?account_address={address}&contract_address={Contract.SKIN}&page_size=6";

            //     // Pagination
            //     if (!string.IsNullOrEmpty(m_Page?.next_cursor))
            //     {
            //         url += $"&page_cursor={m_Page.next_cursor}";
            //     }
            //     else if (m_Page != null && m_Page.next_cursor != null)
            //     {
            //         Debug.Log("No more player assets to load");
            //         return stacks;
            //     }

            //     using var client = new HttpClient();
            //     HttpResponseMessage response = await client.GetAsync(url);

            //     if (response.IsSuccessStatusCode)
            //     {
            //         string responseBody = await response.Content.ReadAsStringAsync();
            //         Debug.Log($"Assets response: {responseBody}");

            //         if (!string.IsNullOrEmpty(responseBody))
            //         {
            //             StacksResponse stacksResponse = JsonUtility.FromJson<StacksResponse>(responseBody);
            //             stacks = stacksResponse?.result ?? new List<StacksResult>();

            //             // Update pagination information
            //             m_Page = stacksResponse?.page;
            //         }
            //     }
            //     else
            //     {
            //         // TODO use dialogs
            //         Debug.Log($"Failed to fetch assets");
            //     }
            // }
            // catch (Exception ex)
            // {
            //     Debug.Log($"Failed to fetch assets: {ex.Message}");
            // }

            return stacks;
        }

        /// <summary>
        /// Cleans up views and handles the back button click
        /// </summary>
        private void OnBackButtonClick()
        {
            // Clear the asset list
            m_Assets.Clear();

            // Reset pagination information
            m_Page = null;

            // Reset the InfiniteScrollView
            m_ScrollView.TotalItemCount = 0;
            m_ScrollView.Clear();

            // Trigger back button event
            m_BackEvent.Raise();
        }
    }
}
