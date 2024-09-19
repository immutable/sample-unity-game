using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HyperCasual.Core;
using Immutable.Passport;
using Immutable.Search.Api;
using Immutable.Search.Client;
using Immutable.Search.Model;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     The marketplace view which displays currently listed skins.
    /// </summary>
    public class MarketplaceScreen : View
    {
        private static readonly List<string> COLOURS = new()
            { "All", "Tropical Indigo", "Cyclamen", "Robin Egg Blue", "Mint", "Mindaro", "Amaranth Pink" };

        private static readonly List<string> SPEEDS = new() { "All", "Slow", "Medium", "Fast" };

        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private AbstractGameEvent m_BackEvent;
        [SerializeField] private BalanceObject m_Balance;
        [SerializeField] private TMP_Dropdown m_ColoursDropdown;
        [SerializeField] private TMP_Dropdown m_SpeedDropdown;
        [SerializeField] private OrderListObject m_OrderObj;
        [SerializeField] private Transform m_ListParent;
        [SerializeField] private InfiniteScrollView m_ScrollView;
        private readonly List<StackBundle> m_Orders = new();

        // Pagination
        private bool m_IsLoadingMore;
        private Page m_Page;

        private void Reset()
        {
            // Clear the order list
            m_Orders.Clear();

            // Reset pagination information
            m_Page = null;

            // Reset the InfiniteScrollView
            m_ScrollView.TotalItemCount = 0;
            m_ScrollView.Clear(); // Resets the scroll view
        }

        /// <summary>
        ///     Sets up the marketplace list and fetches active orers.
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
                if (m_Orders.Count == 0) LoadOrders();

                // Setup filters
                SetupFilters();

                // Gets the player's balance
                m_Balance.UpdateBalance();
            }
        }

        /// <summary>
        ///     Configures the dropdown filters for colours and speeds.
        /// </summary>
        private void SetupFilters()
        {
            m_ColoursDropdown.ClearOptions();
            m_ColoursDropdown.AddOptions(COLOURS);
            m_ColoursDropdown.value = 0; // Default to "All"
            m_ColoursDropdown.onValueChanged.AddListener(delegate
            {
                Reset();
                LoadOrders();
            });

            m_SpeedDropdown.ClearOptions();
            m_SpeedDropdown.AddOptions(SPEEDS);
            m_SpeedDropdown.value = 0; // Default to "All"
            m_SpeedDropdown.onValueChanged.AddListener(delegate
            {
                Reset();
                LoadOrders();
            });
        }

        /// <summary>
        ///     Configures the order list item view
        /// </summary>
        private void OnCreateItemView(int index, GameObject item)
        {
            if (index < m_Orders.Count)
            {
                var order = m_Orders[index];

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
                        var view = UIManager.Instance.GetView<OrderDetailsView>();
                        UIManager.Instance.Show(view);
                        view.Initialise(order);
                    };
                }
            }

            // Load more orders if nearing the end of the list
            if (index >= m_Orders.Count - 5 && !m_IsLoadingMore) LoadOrders();
        }

        /// <summary>
        ///     Loads orders and adds them to the scroll view.
        /// </summary>
        private async void LoadOrders()
        {
            if (m_IsLoadingMore) return;

            m_IsLoadingMore = true;

            var orders = await GetStacks();
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

            var stacks = new List<StackBundle>();

            var config = new Configuration();
            config.BasePath = Config.SEARCH_BASE_URL;
            var apiInstance = new SearchApi(config);

            try
            {
                string? nextCursor = null;
                if (!string.IsNullOrEmpty(m_Page?.NextCursor))
                {
                    nextCursor = m_Page.NextCursor;
                }
                else if (m_Page != null && string.IsNullOrEmpty(m_Page?.NextCursor))
                {
                    Debug.Log("No more assets to load");
                    return stacks;
                }

                // Filter by metadata
                var combinedObject = new Dictionary<string, object>();
                if (m_ColoursDropdown.value != 0 || m_SpeedDropdown.value != 0)
                {
                    if (m_ColoursDropdown.value != 0)
                    {
                        var colourObject = new
                        {
                            Colour = new
                            {
                                values = new List<string> { COLOURS[m_ColoursDropdown.value] },
                                condition = "eq"
                            }
                        };

                        combinedObject["Colour"] = colourObject.Colour;
                    }

                    if (m_SpeedDropdown.value != 0)
                    {
                        var speedObject = new
                        {
                            Speed = new
                            {
                                values = new List<string> { SPEEDS[m_SpeedDropdown.value] },
                                condition = "eq"
                            }
                        };

                        combinedObject["Speed"] = speedObject.Speed;
                    }
                }

                string? trait = null;
                if (combinedObject.Count > 0) trait = JsonConvert.SerializeObject(combinedObject);

                var result = await apiInstance.SearchStacksAsync(
                    Config.CHAIN_NAME,
                    new List<string> { Contract.SKIN },
                    trait: trait,
                    pageSize: Config.PAGE_SIZE, pageCursor: nextCursor);
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

            return stacks;
        }

        /// <summary>
        ///     Cleans up views and handles the back button click
        /// </summary>
        private void OnBackButtonClick()
        {
            Reset();

            // Trigger back button event
            m_BackEvent.Raise();
        }
    }
}