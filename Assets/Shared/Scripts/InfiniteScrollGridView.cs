using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HyperCasual.Runner
{
    public class InfiniteScrollGridView : MonoBehaviour
    {
        [SerializeField] private ScrollRect m_ScrollRect; // The ScrollRect component
        [SerializeField] private RectTransform m_Content; // The content container for the scroll view
        [SerializeField] private GameObject m_ItemPrefab; // Prefab for the items in the scroll view

        private readonly List<GameObject> m_VisibleItems = new();
        private bool m_IsInitialised; // Flag to check if the component is initialised
        private int m_ItemCount;

        public int TotalItemCount
        {
            set
            {
                m_ItemCount = value;
                if (m_IsInitialised) PopulateItems(); // Refresh the items
            }
        }

        private void Awake()
        {
            if (!m_IsInitialised) InitialiseScrollView();
        }

        private void Start()
        {
            if (!m_IsInitialised) InitialiseScrollView();
        }

        public event Action<int, GameObject> OnCreateItemView; // Event for item setup

        private void InitialiseScrollView()
        {
            m_IsInitialised = true;
            m_ScrollRect.onValueChanged.AddListener(OnScroll);
            PopulateItems();
        }

        private void OnScroll(Vector2 scrollPosition)
        {
            if (scrollPosition.y < 0.1f) // Check if we are near the bottom
                PopulateItems();
        }

        private void PopulateItems()
        {
            if (m_ItemCount <= 0) return;

            var visibleItemCount = m_VisibleItems.Count;
            var additionalItemCount =
                Mathf.Min(10, m_ItemCount - visibleItemCount); // Load more in batches of 10 or whatever is left

            for (var i = 0; i < additionalItemCount; i++) CreateItem(visibleItemCount + i);
        }

        private void CreateItem(int index)
        {
            var item = Instantiate(m_ItemPrefab, m_Content);
            item.SetActive(true);
            m_VisibleItems.Add(item);
            OnCreateItemView?.Invoke(index, item);
        }

        /// <summary>
        ///     Clears all items from the scroll view and resets its state.
        /// </summary>
        public void Clear()
        {
            // Destroy all visible items
            foreach (var item in m_VisibleItems) Destroy(item);
            m_VisibleItems.Clear();

            // Reset item count
            m_ItemCount = 0;
        }
    }
}