using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using HyperCasual.Core;
using Immutable.Passport;
using Immutable.Api.ZkEvm.Api;
using Immutable.Api.ZkEvm.Client;
using Immutable.Api.ZkEvm.Model;
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
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private AbstractGameEvent m_BackEvent;
        [SerializeField] private HyperCasualButton m_AddButton;
        [SerializeField] private BalanceObject m_Balance;
        [SerializeField] private TMP_Dropdown m_ColoursDropdown;
        [SerializeField] private TMP_Dropdown m_SpeedDropdown;
        [SerializeField] private InfiniteScrollGridView m_ScrollView;
        [SerializeField] private MarketplaceListObject m_StackObj;
        [SerializeField] private AddFunds m_AddFunds;
        [SerializeField] private CustomDialog m_CustomDialog;

        private StacksApi m_StacksApi = new(new Configuration { BasePath = Config.BASE_URL });
        private readonly List<StackBundle> m_Stacks = new();
        private List<string> m_Colours = new();
        private List<string> m_Speeds = new();
        private bool m_IsLoadingMore;
        private Page m_Page;

        /// <summary>
        /// Resets the marketplace view, clearing the current stacks and resetting pagination.
        /// </summary>
        private void Reset()
        {
            m_Stacks.Clear();
            m_Page = null;
            m_ScrollView.TotalItemCount = 0;
            m_ScrollView.Clear();
        }

        /// <summary>
        /// Sets up the marketplace screen and loads initial stacks.
        /// </summary>
        private async void OnEnable()
        {
            m_StackObj.gameObject.SetActive(false);

            // Set up buttons
            m_BackButton.RemoveListener(OnBackButtonClick);
            m_BackButton.AddListener(OnBackButtonClick);
            m_AddButton.RemoveListener(OnAddFundsButtonClick);
            m_AddButton.AddListener(OnAddFundsButtonClick);

            if (Passport.Instance == null) return;

            m_ScrollView.OnCreateItemView += OnCreateItemView;
            if (m_Stacks.Count == 0) LoadStacks();

            ConfigureFilters();

            var balance = await m_Balance.UpdateBalance();
            if (balance == "0")
            {
                await m_CustomDialog.ShowDialog("Zero Balance", "Your IMR balance is 0. Play the game and collect some tokens!", "OK");
            }
        }

        /// <summary>
        /// Configures the dropdown filters for colours and speeds.
        /// </summary>
        private async void ConfigureFilters()
        {
            var filtersResponse = await m_StacksApi.ListFiltersAsync(
                chainName: Config.CHAIN_NAME,
                contractAddress: Contract.SKIN);
            var filters = filtersResponse.Result.Filters;

            // Configure Colours Dropdown
            m_ColoursDropdown.ClearOptions();
            m_Colours = new List<string> { "All" };
            var colourValues = filters.First(f => f.Name == "Colour")
                .Values.Select(v => v.Value).ToList();
            m_Colours.AddRange(colourValues);
            m_ColoursDropdown.AddOptions(m_Colours);
            m_ColoursDropdown.value = 0; // Default to "All"
            m_ColoursDropdown.onValueChanged.AddListener(_ =>
            {
                Reset();
                LoadStacks();
            });

            // Configure Speeds Dropdown
            m_SpeedDropdown.ClearOptions();
            m_Speeds = new List<string> { "All" };
            var speedValues = filters.First(f => f.Name == "Speed")
                .Values.Select(v => v.Value).ToList();
            speedValues.Sort(); // Sort speed values alphabetically
            m_Speeds.AddRange(speedValues);
            m_SpeedDropdown.AddOptions(m_Speeds);
            m_SpeedDropdown.value = 0; // Default to "All"
            m_SpeedDropdown.onValueChanged.AddListener(_ =>
            {
                Reset();
                LoadStacks();
            });
        }

        /// <summary>
        /// Configures each item view in the stack list.
        /// </summary>
        /// <param name="index">Index of the item in the stack list.</param>
        /// <param name="item">The game object representing the item view.</param>
        private void OnCreateItemView(int index, GameObject item)
        {
            if (index < m_Stacks.Count)
            {
                var stack = m_Stacks[index];

                var itemComponent = item.GetComponent<MarketplaceListObject>();
                itemComponent.Initialise(stack);

                var clickable = item.GetComponent<ClickableView>();
                if (clickable != null)
                {
                    clickable.ClearAllSubscribers();
                    clickable.OnClick += () =>
                    {
                        var view = UIManager.Instance.GetView<MarketplaceAssetDetailsView>();
                        UIManager.Instance.Show(view);
                        view.Initialise(stack);
                    };
                }
            }

            if (index >= m_Stacks.Count - 8 && !m_IsLoadingMore) LoadStacks();
        }

        /// <summary>
        /// Loads stacks and adds them to the scroll view.
        /// </summary>
        private async void LoadStacks()
        {
            if (m_IsLoadingMore) return;

            m_IsLoadingMore = true;

            var stacks = await GetStacks();
            if (stacks != null && stacks.Count > 0)
            {
                m_Stacks.AddRange(stacks);
                m_ScrollView.TotalItemCount = m_Stacks.Count;
            }

            m_IsLoadingMore = false;
        }

        /// <summary>
        /// Fetches the list of stacks from the API.
        /// </summary>
        /// <returns>List of stacks.</returns>
        private async UniTask<List<StackBundle>> GetStacks()
        {
            Debug.Log("Fetching stacks...");

            try
            {
                var nextCursor = m_Page?.NextCursor ?? null;
                if (m_Page != null && string.IsNullOrEmpty(nextCursor))
                {
                    Debug.Log("No more assets to load");
                    return new List<StackBundle>();
                }

                // Filter based on dropdown selections
                var filters = new Dictionary<string, object>();
                if (m_ColoursDropdown.value != 0)
                    filters["Colour"] = new
                    {
                        values = new List<string> { m_Colours[m_ColoursDropdown.value] },
                        condition = "eq"
                    };
                if (m_SpeedDropdown.value != 0)
                    filters["Speed"] = new
                    {
                        values = new List<string> { m_Speeds[m_SpeedDropdown.value] },
                        condition = "eq"
                    };

                var trait = filters.Count > 0 ? JsonConvert.SerializeObject(filters) : null;

                // Fetch stacks from the API
                var result = await m_StacksApi.SearchStacksAsync(
                    Config.CHAIN_NAME,
                    new List<string> { Contract.SKIN },
                    traits: trait,
                    onlyIfHasActiveListings: true,
                    pageSize: Config.PAGE_SIZE,
                    pageCursor: nextCursor);

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

            return new List<StackBundle>();
        }

        /// <summary>
        ///     Handles the back button click event, cleaning up views and navigating back.
        /// </summary>
        private void OnBackButtonClick()
        {
            Reset();
            m_BackEvent.Raise();
        }

        /// <summary>
        ///     handles the add funds button click
        /// </summary>
        private void OnAddFundsButtonClick()
        {
            m_AddFunds.Show();
        }
    }
}