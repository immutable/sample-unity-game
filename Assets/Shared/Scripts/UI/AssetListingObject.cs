using System;
using Cysharp.Threading.Tasks;
using HyperCasual.Core;
using Immutable.Search.Model;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    public class AssetListingObject : View
    {
        [SerializeField] private TextMeshProUGUI m_TokenIdText;
        [SerializeField] private HyperCasualButton m_CancelButton;
        [SerializeField] private GameObject m_Progress;
        private Listing m_Asset;
        private Func<Listing, UniTask<bool>> m_OnCancel;

        private async void OnEnable()
        {
        }

        /// <summary>
        ///     Cleans up data
        /// </summary>
        private void OnDisable()
        {
            m_TokenIdText.text = "";

            m_Asset = null;
        }

        /// <summary>
        ///     Initialises the UI based on the order
        /// </summary>
        public async void Initialise(Listing listing, Func<Listing, UniTask<bool>> onCancel)
        {
            m_Asset = listing;
            m_OnCancel = onCancel;
            m_TokenIdText.text = $"Token ID: {m_Asset.TokenId}";

            // Hide progress
            m_Progress.SetActive(false);

            // Set listener to button
            m_CancelButton.RemoveListener(OnCancelButtonClick);
            m_CancelButton.AddListener(OnCancelButtonClick);
        }

        /// <summary>
        ///     Handles the cancel button click event.
        /// </summary>
        private async void OnCancelButtonClick()
        {
            m_CancelButton.gameObject.SetActive(false);
            m_Progress.SetActive(true);

            var success = await m_OnCancel(m_Asset);

            m_CancelButton.gameObject.SetActive(!success);
            m_Progress.SetActive(false);
        }
    }
}