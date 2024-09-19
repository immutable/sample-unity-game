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
    ///     Displays a marketplace view for currently listed skins.
    /// </summary>
    public class MarketplaceScreen : View
    {
        // Lists of available colours and speeds for filtering
        private static readonly List<string> s_Colours = new()
            { "All", "Tropical Indigo", "Cyclamen", "Robin Egg Blue", "Mint", "Mindaro", "Amaranth Pink" };

        private static readonly List<string> s_Speeds = new() { "All", "Slow", "Medium", "Fast" };

        // Back button and its event
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private AbstractGameEvent m_BackEvent;

        // Player's balance display
        [SerializeField] private BalanceObject m_Balance;

        // Dropdown filters for colours and speeds
        [SerializeField] private TMP_Dropdown m_ColoursDropdown;
        [SerializeField] private TMP_Dropdown m_SpeedDropdown;

        // Infinite scrolling list of stacks
        [SerializeField] private InfiniteScrollView m_ScrollView;

        // Template for displaying a stack
        [SerializeField] private OrderListObject m_StackObj;

        // List to store the loaded stacks
        private readonly List<StackBundle> m_Stacks = new();

        // Pagination and loading state
        private bool m_IsLoadingMore;
        private Page m_Page;

        /// <summary>
        ///     Resets the marketplace view, clearing the current stacks and resetting pagination.
        /// </summary>
        private void Reset()
        {
            m_Stacks.Clear();
            m_Page = null;

            // Reset the scroll view
            m_ScrollView.TotalItemCount = 0;
            m_ScrollView.Clear();
        }

        /// <summary>
        ///     Sets up the marketplace screen and loads initial stacks.
        /// </summary>
        private void OnEnable()
        {
            // Hide the stack template
            m_StackObj.gameObject.SetActive(false);

            // Attach back button listener
            m_BackButton.RemoveListener(OnBackButtonClick);
            m_BackButton.AddListener(OnBackButtonClick);

            if (Passport.Instance == null) return;

            // Set up the infinite scroll view and load stacks
            m_ScrollView.OnCreateItemView += OnCreateItemView;
            if (m_Stacks.Count == 0) LoadStacks();

            // Initialise dropdown filters
            SetupFilters();

            // Update player's balance
            m_Balance.UpdateBalance();
        }

        /// <summary>
        ///     Configures the dropdown filters for colours and speeds.
        /// </summary>
        private void SetupFilters()
        {
            // Set up colour dropdown
            m_ColoursDropdown.ClearOptions();
            m_ColoursDropdown.AddOptions(s_Colours);
            m_ColoursDropdown.value = 0; // Default to "All"
            m_ColoursDropdown.onValueChanged.AddListener(delegate
            {
                Reset();
                LoadStacks();
            });

            // Set up speed dropdown
            m_SpeedDropdown.ClearOptions();
            m_SpeedDropdown.AddOptions(s_Speeds);
            m_SpeedDropdown.value = 0; // Default to "All"
            m_SpeedDropdown.onValueChanged.AddListener(delegate
            {
                Reset();
                LoadStacks();
            });
        }

        /// <summary>
        ///     Configures each item view in the stack list.
        /// </summary>
        /// <param name="index">Index of the item in the stack list.</param>
        /// <param name="item">GameObject representing the item view.</param>
        private void OnCreateItemView(int index, GameObject item)
        {
            if (index < m_Stacks.Count)
            {
                var stack = m_Stacks[index];

                // Initialise the item view with the stack data
                var itemComponent = item.GetComponent<OrderListObject>();
                itemComponent.Initialise(stack);

                // Set up click handling for the item
                var clickable = item.GetComponent<ClickableView>();
                if (clickable != null)
                {
                    clickable.ClearAllSubscribers();
                    clickable.OnClick += () =>
                    {
                        var view = UIManager.Instance.GetView<OrderDetailsView>();
                        UIManager.Instance.Show(view);
                        view.Initialise(stack);
                    };
                }
            }

            // Load more stacks if nearing the end of the list
            if (index >= m_Stacks.Count - 5 && !m_IsLoadingMore) LoadStacks();
        }

        /// <summary>
        ///     Loads stacks and adds them to the scroll view.
        /// </summary>
        private async void LoadStacks()
        {
            if (m_IsLoadingMore) return;

            m_IsLoadingMore = true;

            // Fetch the next set of stacks
            var stacks = await GetStacks();
            if (stacks != null && stacks.Count > 0)
            {
                m_Stacks.AddRange(stacks);
                m_ScrollView.TotalItemCount = m_Stacks.Count;
            }

            m_IsLoadingMore = false;
        }

        /// <summary>
        ///     Fetches the list of stacks from the API.
        /// </summary>
        /// <returns>List of StackBundle objects representing the stacks.</returns>
        private async UniTask<List<StackBundle>> GetStacks()
        {
            Debug.Log("Fetching stacks...");

            var stacks = new List<StackBundle>();
            var config = new Configuration { BasePath = Config.SEARCH_BASE_URL };
            var apiInstance = new SearchApi(config);

            try
            {
                var nextCursor = m_Page?.NextCursor ?? null;
                if (m_Page != null && string.IsNullOrEmpty(nextCursor))
                {
                    Debug.Log("No more assets to load");
                    return stacks;
                }

                // Filter based on dropdown selections
                var filters = new Dictionary<string, object>();
                if (m_ColoursDropdown.value != 0)
                    filters["Colour"] = new
                    {
                        values = new List<string> { s_Colours[m_ColoursDropdown.value] },
                        condition = "eq"
                    };
                if (m_SpeedDropdown.value != 0)
                    filters["Speed"] = new
                    {
                        values = new List<string> { s_Speeds[m_SpeedDropdown.value] },
                        condition = "eq"
                    };

                var trait = filters.Count > 0 ? JsonConvert.SerializeObject(filters) : null;

                // Fetch stacks from the API
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
                Debug.LogError($"API error: {e.Message} (Status Code: {e.ErrorCode})");
                Debug.LogError(e.StackTrace);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error fetching stacks: {ex.Message}");
            }

            return stacks;
        }

        /// <summary>
        ///     Handles the back button click event, cleaning up views and navigating back.
        /// </summary>
        private void OnBackButtonClick()
        {
            Reset();
            m_BackEvent.Raise();
        }
    }
}