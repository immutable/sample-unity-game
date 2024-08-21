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
        private int m_ItemCount = 0;

        public event Action<int, GameObject> OnCreateItemView; // Event for item setup

        private RectTransform m_RectTransform;
        private List<GameObject> m_VisibleItems = new List<GameObject>();
        private int m_ItemHeight;
        private bool m_IsInitialised = false; // Flag to check if the component is initialised

        private void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
            if (m_ItemPrefab != null)
            {
                m_ItemHeight = (int)m_ItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
            }
        }

        private void Start()
        {
            if (!m_IsInitialised)
            {
                InitializeScrollView();
            }
        }

        private void InitializeScrollView()
        {
            m_IsInitialised = true;
            m_ScrollRect.onValueChanged.AddListener(OnScroll);
            PopulateItems();
        }

        private void OnScroll(Vector2 scrollPosition)
        {
            if (scrollPosition.y < 0.1f) // Check if we are near the bottom
            {
                PopulateItems();
            }
        }

        private void PopulateItems()
        {
            if (m_ItemCount <= 0) return;

            int itemHeight = m_ItemHeight;
            int visibleItemCount = Mathf.CeilToInt(m_RectTransform.rect.height / itemHeight);
            int startIndex = m_VisibleItems.Count;
            int endIndex = Mathf.Min(startIndex + visibleItemCount, m_ItemCount);

            for (int i = startIndex; i < endIndex; i++)
            {
                if (i >= m_VisibleItems.Count)
                {
                    CreateItem(i);
                }
            }

            // Remove items that are no longer visible
            for (int i = m_VisibleItems.Count - 1; i >= endIndex; i--)
            {
                Destroy(m_VisibleItems[i]);
                m_VisibleItems.RemoveAt(i);
            }
        }

        private void CreateItem(int index)
        {
            GameObject item = Instantiate(m_ItemPrefab, m_Content);
            item.SetActive(true);
            m_VisibleItems.Add(item);
            OnCreateItemView?.Invoke(index, item);
        }

        public int TotalItemCount
        {
            set
            {
                m_ItemCount = value;
                if (m_IsInitialised)
                {
                    PopulateItems(); // Refresh the items
                }
            }
        }

        /// <summary>
        /// Clears all items from the scroll view and resets its state.
        /// </summary>
        public void Clear()
        {
            // Destroy all visible items
            foreach (GameObject item in m_VisibleItems)
            {
                Destroy(item);
            }
            m_VisibleItems.Clear();

            // Reset item count
            m_ItemCount = 0;
        }
    }
}
