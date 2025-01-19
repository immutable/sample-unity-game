using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using HyperCasual.Core;
using UnityEngine;
using TMPro;
using Immutable.Api.ZkEvm.Client;
using Immutable.Api.ZkEvm.Model;
using Immutable.Api.ZkEvm.Api;

namespace HyperCasual.Runner
{
    /// <summary>
    /// Represents the Inventory screen displaying the player’s NFTs, such as skins and power-ups.
    /// </summary>
    public class InventoryScreen : View
    {
        public enum AssetType { Skin, Powerups }

        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private AbstractGameEvent m_BackEvent;
        [SerializeField] private BalanceObject m_Balance;
        [SerializeField] private TMP_Dropdown m_TypeDropdown;
        [SerializeField] private InventoryListObject m_InventoryObj;
        [SerializeField] private InfiniteScrollGridView m_ScrollView;

        private MetadataSearchApi m_MetadataSearchApi = new(new Configuration { BasePath = Config.BASE_URL });
        private AssetType m_Type = AssetType.Skin;
        private readonly List<NFTBundle> m_Assets = new();
        private bool m_IsLoadingMore;
        private Page m_Page;

        private void OnEnable()
        {
            m_InventoryObj.gameObject.SetActive(false);

            m_BackButton.RemoveListener(OnBackButtonClick);
            m_BackButton.AddListener(OnBackButtonClick);

            m_ScrollView.OnCreateItemView += OnCreateItemView;
            if (m_Assets.Count == 0) LoadAssets();

            ConfigureFilters();

            m_Balance.UpdateBalance();
        }

        /// <summary>
        /// Sets up the filter dropdown
        /// </summary>
        private void ConfigureFilters()
        {
            m_TypeDropdown.ClearOptions();
            m_TypeDropdown.AddOptions(Enum.GetNames(typeof(AssetType)).ToList());
            m_TypeDropdown.value = m_Type == AssetType.Skin ? 0 : 1;

            m_TypeDropdown.onValueChanged.AddListener(delegate
            {
                ResetInventory();
                m_Type = (AssetType)m_TypeDropdown.value;
                LoadAssets();
            });
        }

        /// <summary>
        /// Configures each item view in the inventory list.
        /// </summary>
        /// <param name="index">The index of the asset in the list.</param>
        /// <param name="item">The game object representing the asset view.</param>
        private void OnCreateItemView(int index, GameObject item)
        {
            if (index < m_Assets.Count)
            {
                var asset = m_Assets[index];
                var itemComponent = item.GetComponent<InventoryListObject>();
                itemComponent.Initialise(asset);

                var clickable = item.GetComponent<ClickableView>();
                if (clickable != null)
                {
                    clickable.ClearAllSubscribers();
                    clickable.OnClick += () =>
                    {
                        m_Balance.ClosePanel();

                        var view = UIManager.Instance.GetView<InventoryAssetDetailsView>();
                        UIManager.Instance.Show(view);
                        view.Initialise(asset);
                    };
                }
            }

            if (index >= m_Assets.Count - 8 && !m_IsLoadingMore) LoadAssets();
        }

        /// <summary>
        /// Loads the player's assets and adds them to the scroll view
        /// </summary>
        private async void LoadAssets()
        {
            if (m_IsLoadingMore) return;

            m_IsLoadingMore = true;

            var assets = await GetAssets();
            if (assets.Any())
            {
                m_Assets.AddRange(assets);
                m_ScrollView.TotalItemCount = m_Assets.Count;
            }

            m_IsLoadingMore = false;
        }

        /// <summary>
        /// Fetches the player’s NFTs from the API based on the selected asset type.
        /// </summary>
        /// <returns>A list of NFT bundles.</returns>
        private async UniTask<List<NFTBundle>> GetAssets()
        {
            Debug.Log("Fetching assets...");

            try
            {
                var nextCursor = m_Page?.NextCursor;
                if (m_Page != null && string.IsNullOrEmpty(nextCursor))
                {
                    Debug.Log("No more assets to load.");
                    return new List<NFTBundle>();
                }

                var contractAddress = m_Type == AssetType.Skin ? Contract.SKIN : Contract.PACK;

                var result = await m_MetadataSearchApi.SearchNFTsAsync(
                    Config.CHAIN_NAME,
                    new List<string> { contractAddress },
                    SaveManager.Instance.WalletAddress,
                    onlyIncludeOwnerListings: true,
                    pageSize: Config.PAGE_SIZE,
                    pageCursor: nextCursor
                );

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
                Debug.LogError($"Error fetching NFTs: {ex.Message}");
            }

            return new List<NFTBundle>();
        }

        /// <summary>
        /// Handles the back button click by resetting the view and raising the back event.
        /// </summary>
        private void OnBackButtonClick()
        {
            ResetInventory();
            m_BackEvent.Raise();
        }

        /// <summary>
        /// Resets the inventory to its default state.
        /// </summary>
        private void ResetInventory()
        {
            m_Type = AssetType.Skin;
            m_Assets.Clear();
            m_Page = null;
            m_ScrollView.TotalItemCount = 0;
            m_ScrollView.Clear();
            m_Balance.ClosePanel();
        }
    }
}
