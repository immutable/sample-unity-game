using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using Immutable.Passport;
using TMPro;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Immutable.Passport.Model;
using Immutable.Search.Model;

namespace HyperCasual.Runner
{
    public class AssetListingObject : View
    {
        [SerializeField] private TextMeshProUGUI m_TokenIdText = null;
        [SerializeField] private HyperCasualButton m_CancelButton;
        [SerializeField] private GameObject m_Progress = null;
        private Listing m_Asset;
        private Func<Listing, UniTask<bool>> m_OnCancel;

        async void OnEnable()
        {
        }

        /// <summary>
        /// Initialises the UI based on the order
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
        /// Handles the cancel button click event.
        /// </summary>
        private async void OnCancelButtonClick()
        {
            m_CancelButton.gameObject.SetActive(false);
            m_Progress.SetActive(true);

            bool success = await m_OnCancel(m_Asset);

            m_CancelButton.gameObject.SetActive(!success);
            m_Progress.SetActive(false);
        }

        /// <summary>
        /// Cleans up data
        /// </summary>
        private void OnDisable()
        {
            m_TokenIdText.text = "";

            m_Asset = null;
        }
    }
}