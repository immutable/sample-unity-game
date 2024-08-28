using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using Immutable.Passport;
using TMPro;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace HyperCasual.Runner
{
    /// <summary>
    /// Represents an asset in the player's inventory
    /// </summary>
    public class AssetListObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText = null;
        [SerializeField] private TextMeshProUGUI m_TokenIdText = null;
        [SerializeField] private TextMeshProUGUI m_CollectionText = null;
        [SerializeField] private TextMeshProUGUI m_StatusText = null;
        [SerializeField] private ImageUrlObject m_Image = null;

        private StacksResult m_Asset;

        /// <summary>
        /// Initialises the asset object with relevant data and updates the UI.
        /// </summary>
        public void Initialise(StacksResult asset)
        {
            m_Asset = asset;
            UpdateData();
        }

        /// <summary>
        /// Sets up the inventory list and fetches the player's assets.
        /// </summary>
        private void OnEnable()
        {
            UpdateData();
        }

        /// <summary>
        /// Updates the text fields with asset data.
        /// </summary>
        private async void UpdateData()
        {
            if (m_Asset != null)
            {
                m_NameText.text = m_Asset.stack.name;
                m_CollectionText.text = m_Asset.stack.contract_address;
                m_StatusText.text = m_Asset.listings?.Count > 0 == true ? "Listed" : "Not listed";
                m_Image.LoadUrl(m_Asset.stack.image);
            }
        }
    }
}