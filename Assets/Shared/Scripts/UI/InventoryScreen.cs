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

namespace HyperCasual.Runner
{
    /// <summary>
    /// The inventory view which displays the player's assets (e.g. skins).
    /// </summary>
    public class InventoryScreen : View
    {
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private AbstractGameEvent m_BackEvent;
        [SerializeField] private AssetListObject m_AssetObj = null;
        [SerializeField] private Transform m_ListParent = null;
        [SerializeField] private InfiniteScrollView m_ScrollView;
        [SerializeField] private AssetItemClickedEvent m_AssetClickedEvent;
        private List<AssetModel> m_Assets = new List<AssetModel>();

        // Pagination
        private bool m_IsLoadingMore = false;
        private PageModel m_Page;

        /// <summary>
        /// Sets up the inventory list and fetches the player's assets.
        /// </summary>
        private async void OnEnable()
        {
            // Hide asset template item
            m_AssetObj.gameObject.SetActive(false);

            // Add listener to back button
            m_BackButton.AddListener(OnBackButtonClick);

            if (Passport.Instance != null)
            {
                // Setup infinite scroll view and load assets
                m_ScrollView.OnCreateItemView += OnCreateItemView;
                LoadAssets();
            }
        }

        /// <summary>
        /// Configures the asset list item view
        /// </summary>
        private void OnCreateItemView(int index, GameObject item)
        {
            if (index < m_Assets.Count)
            {
                AssetModel asset = m_Assets[index];

                // Initialise the view with asset
                var itemComponent = item.GetComponent<AssetListObject>();
                itemComponent.Initialise(asset);
                // Set up click listener
                var clickable = item.GetComponent<ClickableView>();
                if (clickable != null)
                {
                    clickable.OnClick += () =>
                    {
                        m_AssetClickedEvent.Raise(asset);
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

            List<AssetModel> assets = await GetAssets();
            if (assets != null && assets.Count > 0)
            {
                m_Assets.AddRange(assets);
                m_ScrollView.TotalItemCount = m_Assets.Count;
            }

            m_IsLoadingMore = false;
        }

        /// <summary>
        /// Retrieves the players's wallet address.
        /// </summary>
        private async UniTask<string> GetWalletAddress()
        {
            List<string> accounts = await Passport.Instance.ZkEvmRequestAccounts();
            return accounts.Count > 0 ? accounts[0] : string.Empty; // Return the first wallet address
        }

        /// <summary>
        /// Parses the JSON response body to extract asset tokens.
        /// Updates the pagination info from the response.
        /// </summary>
        private List<AssetModel> ParseAssetsResponse(string responseBody)
        {
            if (string.IsNullOrEmpty(responseBody))
            {
                return new List<AssetModel>();
            }

            AssetsResponse response = JsonUtility.FromJson<AssetsResponse>(responseBody);
            if (response == null)
            {
                return new List<AssetModel>();
            }

            // Update pagination information
            m_Page = response.page;

            return response.result ?? new List<AssetModel>();
        }

        /// <summary>
        /// Fetches the player's skins.
        /// </summary>
        private async UniTask<List<AssetModel>> GetAssets()
        {
            Debug.Log("Fetching player assets...");

            List<AssetModel> tokens = new List<AssetModel>();

            try
            {
                string address = await GetWalletAddress();

                if (string.IsNullOrEmpty(address))
                {
                    Debug.LogError("Could not get player's wallet");
                    return tokens;
                }

                string url = $"https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/accounts/{address}/nfts?contract_address={Contract.SKIN_CONTRACT}&page_size=20";

                // Pagination
                if (m_Page?.next_cursor != null)
                {
                    url += $"&page_cursor={m_Page.next_cursor}";
                }
                else if (m_Page != null && m_Page.next_cursor != null)
                {
                    Debug.Log("No more player assets to load");
                    return tokens;
                }

                using var client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log($"Assets response: {responseBody}");

                    tokens = ParseAssetsResponse(responseBody);
                }
                else
                {
                    // TODO use dialogs
                    Debug.Log($"Failed to fetch assets");
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to fetch assets: {ex.Message}");
            }

            return tokens;
        }

        /// <summary>
        /// Cleans up event listeners and views
        /// </summary>
        private void OnDisable()
        {
            // Remove listener from the back button
            m_BackButton.RemoveListener(OnBackButtonClick);

            // // Clear the asset list
            // m_Assets.Clear();

            // // Reset pagination information
            // m_Page = null;

            // // Reset the InfiniteScrollView
            // m_ScrollView.TotalItemCount = 0;
            // m_ScrollView.Clear(); // Resets the scroll view
        }


        /// <summary>
        /// Handles the back button click
        /// </summary>
        private void OnBackButtonClick()
        {
            Debug.Log("hello");
            m_BackEvent.Raise();
        }
    }
}
