using System;
using System.Net.Http;
using System.Numerics;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     Represents an asset in the player's inventory
    /// </summary>
    public class PackListObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_DescriptionText;
        [SerializeField] private ImageUrlObject m_Image;

        private Pack m_Pack;

        private void OnEnable()
        {
            UpdateData();
        }

        /// <summary>
        ///     Initialises the asset object with relevant data and updates the UI.
        /// </summary>
        public void Initialise(Pack pack)
        {
            m_Pack = pack;

            UpdateData();
        }

        /// <summary>
        ///     Updates the text fields with data.
        /// </summary>
        private async void UpdateData()
        {
            if (m_Pack != null)
            {
                m_NameText.text = m_Pack.name;
                m_DescriptionText.text = m_Pack.description;
                m_Image.LoadUrl(m_Pack.image);
            }
        }
    }
}