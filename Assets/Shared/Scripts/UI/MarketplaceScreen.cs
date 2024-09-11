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
using Immutable.Search.Api;
using Immutable.Search.Client;
using Immutable.Search.Model;

namespace HyperCasual.Runner
{
    /// <summary>
    /// The marketplace view which displays currently listed skins.
    /// </summary>
    public class MarketplaceScreen : View
    {
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private AbstractGameEvent m_BackEvent;
        [SerializeField] private BalanceObject m_Balance;
        [SerializeField] private TMP_Dropdown m_ColoursDropdown;
        [SerializeField] private TMP_Dropdown m_SpeedDropdown;
        [SerializeField] private OrderListObject m_OrderObj = null;
        [SerializeField] private Transform m_ListParent = null;
        [SerializeField] private InfiniteScrollView m_ScrollView;
        private List<StackBundle> m_Orders = new List<StackBundle>();

        // Pagination
        private bool m_IsLoadingMore = false;
        private Page m_Page;

        /// <summary>
        /// Sets up the marketplace list and fetches active orers.
        /// </summary>
        private async void OnEnable()
        {
            // Hide order template item
            m_OrderObj.gameObject.SetActive(false);

            // Add listener to back button
            m_BackButton.RemoveListener(OnBackButtonClick);
            m_BackButton.AddListener(OnBackButtonClick);

            if (Passport.Instance != null)
            {
                // Setup infinite scroll view and load orders
                m_ScrollView.OnCreateItemView += OnCreateItemView;
                LoadOrders();

                // Setup filters
                SetupFilters();

                // Gets the player's balance
                m_Balance.UpdateBalance();
            }
        }

        /// <summary>
        /// Configures the dropdown filters for colours and speeds.
        /// </summary>
        private void SetupFilters()
        {
            m_ColoursDropdown.ClearOptions();
            var colours = new List<string> { "All", "Tropical Indigo", "Cyclamen", "Robin Egg Blue", "Mint", "Mindaro", "Amaranth Pink" };
            m_ColoursDropdown.AddOptions(colours);
            m_ColoursDropdown.value = 0; // Default to "All"

            m_SpeedDropdown.ClearOptions();
            var speeds = new List<string> { "All", "Slow", "Medium", "Fast" };
            m_SpeedDropdown.AddOptions(speeds);
            m_SpeedDropdown.value = 0; // Default to "All"
        }

        /// <summary>
        /// Configures the order list item view
        /// </summary>
        private void OnCreateItemView(int index, GameObject item)
        {
            if (index < m_Orders.Count)
            {
                StackBundle order = m_Orders[index];

                // Initialise the view with order
                var itemComponent = item.GetComponent<OrderListObject>();
                itemComponent.Initialise(order);
                // Set up click listener
                var clickable = item.GetComponent<ClickableView>();
                if (clickable != null)
                {
                    clickable.ClearAllSubscribers();
                    clickable.OnClick += () =>
                    {
                        OrderDetailsView view = UIManager.Instance.GetView<OrderDetailsView>();
                        UIManager.Instance.Show(view, true);
                        view.Initialise(order);
                    };
                }
            }

            // Load more orders if nearing the end of the list
            if (index >= m_Orders.Count - 5 && !m_IsLoadingMore)
            {
                LoadOrders();
            }
        }

        /// <summary>
        /// Loads orders and adds them to the scroll view.
        /// </summary>
        private async void LoadOrders()
        {
            if (m_IsLoadingMore)
            {
                return;
            }

            m_IsLoadingMore = true;

            List<StackBundle> orders = await GetStacks();
            if (orders != null && orders.Count > 0)
            {
                m_Orders.AddRange(orders);
                m_ScrollView.TotalItemCount = m_Orders.Count;
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
                string? nextCursor = null;
                if (!string.IsNullOrEmpty(m_Page?.NextCursor))
                {
                    nextCursor = m_Page.NextCursor;
                }
                else if (m_Page != null && (m_Page.NextCursor != null || m_Orders.Count < Config.PAGE_SIZE))
                {
                    Debug.Log("No more assets to load");
                    return stacks;
                }

                SearchStacksResult result = await apiInstance.SearchStacksAsync(Config.CHAIN_NAME, new List<string> { Contract.SKIN }, pageSize: Config.PAGE_SIZE);
                m_Page = result.Page;
                return result.Result;
            }
            catch (ApiException e)
            {
                Debug.LogError("Exception when calling: " + e.Message);
                Debug.LogError("Status Code: " + e.ErrorCode);
                Debug.LogError(e.StackTrace);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to fetch assets: {ex.Message}");
            }

            // List<StacksResult> stacks = new List<StacksResult>();

            // try
            // {
            //     string address = SaveManager.Instance.WalletAddress;

            //     if (string.IsNullOrEmpty(address))
            //     {
            //         Debug.LogError("Could not get player's wallet");
            //         return stacks;
            //     }

            //     string url = $"http://localhost:6060/v1/chains/imtbl-zkevm-testnet/search/stacks/marketplace?contract_address={Contract.SKIN}&page_size=6";

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
            // Clear the order list
            m_Orders.Clear();

            // Reset pagination information
            m_Page = null;

            // Reset the InfiniteScrollView
            m_ScrollView.TotalItemCount = 0;
            m_ScrollView.Clear(); // Resets the scroll view

            // Trigger back button event
            m_BackEvent.Raise();
        }
    }
}
