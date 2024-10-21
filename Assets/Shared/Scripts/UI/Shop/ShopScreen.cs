using System;
using System.Collections.Generic;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using HyperCasual.Core;
using Immutable.Passport;
using UnityEngine;
using Xsolla.Core;
using TMPro;
using Immutable.Api.Client;
using Immutable.Api.Model;
using Immutable.Api.Api;
using UnityEngine.Serialization;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     The inventory view which displays the player's assets (e.g. skins).
    /// </summary>
    public class ShopScreen : View
    {
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private HyperCasualButton m_AddButton;
        [SerializeField] private AbstractGameEvent m_BackEvent;
        [SerializeField] private BalanceObject m_Balance;
        [SerializeField] private ShopListObject m_ItemObj;
        [SerializeField] private Transform m_ListParent;
        [SerializeField] private InfiniteScrollGridView m_ScrollView;
        [SerializeField] private AddFunds m_AddFunds;

        private readonly List<Pack> m_Packs = new();

        /// <summary>
        ///     Sets up the inventory list and fetches the player's assets.
        /// </summary>
        private void OnEnable()
        {
            // Hide pack template item
            m_ItemObj.gameObject.SetActive(false);

            m_BackButton.RemoveListener(OnBackButtonClick);
            m_BackButton.AddListener(OnBackButtonClick);

            m_AddButton.RemoveListener(OnAddFundsButtonClick);
            m_AddButton.AddListener(OnAddFundsButtonClick);

            if (Passport.Instance != null)
            {
                // Setup infinite scroll view and load packs
                m_ScrollView.OnCreateItemView += OnCreateItemView;
                if (m_Packs.Count == 0) LoadPacks();

                // Gets the player's balance
                m_Balance.UpdateBalance();
            }
        }

        /// <summary>
        ///     Configures the asset list item view
        /// </summary>
        private void OnCreateItemView(int index, GameObject item)
        {
            if (index < m_Packs.Count)
            {
                var pack = m_Packs[index];

                // Initialise the view with asset
                var itemComponent = item.GetComponent<ShopListObject>();
                itemComponent.Initialise(pack);
                // Set up click listener
                var clickable = item.GetComponent<ClickableView>();
                if (clickable != null)
                {
                    clickable.ClearAllSubscribers();
                    clickable.OnClick += () =>
                    {
                        var view = UIManager.Instance.GetView<PackDetailsView>();
                        UIManager.Instance.Show(view);
                        view.Initialise(pack);
                    };
                }
            }
        }

        /// <summary>
        ///     Loads assets and adds them to the scroll view.
        /// </summary>
        private async void LoadPacks()
        {
            var packs = await GetPacks();
            if (packs != null && packs.Count > 0)
            {
                m_Packs.AddRange(packs);
                m_ScrollView.TotalItemCount = m_Packs.Count;
            }
        }

        private async UniTask<List<Pack>> GetPacks()
        {
            Debug.Log("Fetching packs...");

            var packs = new List<Pack>();

            try
            {
                var url = $"{Config.SERVER_URL}/packs";

                using var client = new HttpClient();
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log($"Assets response: {responseBody}");

                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        packs = JsonUtility.FromJson<Packs>(responseBody).result;
                    }
                }
                else
                {
                    Debug.Log("Failed to fetch packs");
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to fetch packs: {ex.Message}");
            }

            return packs;
        }

        /// <summary>
        ///     Cleans up views and handles the back button click
        /// </summary>
        private void OnBackButtonClick()
        {
            // Clear the asset list
            m_Packs.Clear();

            // Reset the InfiniteScrollView
            m_ScrollView.TotalItemCount = 0;
            m_ScrollView.Clear();

            // Trigger back button event
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