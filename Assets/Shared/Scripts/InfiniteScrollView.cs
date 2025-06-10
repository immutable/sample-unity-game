using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HyperCasual.Runner
{
    public class InfiniteScrollView : MonoBehaviour
    {
        [SerializeField] private ScrollRect m_ScrollRect; // The ScrollRect component
        [SerializeField] private RectTransform m_Content; // The content container for the scroll view
        [SerializeField] private GameObject m_ItemPrefab; // Prefab for the items in the scroll view
        private readonly List<GameObject> m_VisibleItems = new();
        private bool m_IsInitialised; // Flag to check if the component is initialised
        private int m_ItemCount;
        private int m_ItemHeight;

        private RectTransform m_RectTransform;

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
            m_RectTransform = GetComponent<RectTransform>();
            if (m_ItemPrefab != null) m_ItemHeight = (int)m_ItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
        }

        private void Start()
        {
            if (!m_IsInitialised) InitializeScrollView();
        }

        public event Action<int, GameObject> OnCreateItemView; // Event for item setup

        private void InitializeScrollView()
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

            var itemHeight = m_ItemHeight;
            var visibleItemCount = Mathf.CeilToInt(m_RectTransform.rect.height / itemHeight);
            var startIndex = m_VisibleItems.Count;
            var endIndex = Mathf.Min(startIndex + visibleItemCount, m_ItemCount);

            for (var i = startIndex; i < endIndex; i++)
                if (i >= m_VisibleItems.Count)
                    CreateItem(i);

            // Remove items that are no longer visible
            for (var i = m_VisibleItems.Count - 1; i >= endIndex; i--)
                if (i > 0)
                {
                    Destroy(m_VisibleItems[i]);
                    m_VisibleItems.RemoveAt(i);
                }
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