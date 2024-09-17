using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     Represents an asset in the player's inventory
    /// </summary>
    public class AssetListObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_TokenIdText;
        [SerializeField] private TextMeshProUGUI m_CollectionText;
        [SerializeField] private ImageUrlObject m_Image;

        private AssetModel m_Asset;

        /// <summary>
        ///     Sets up the inventory list and fetches the player's assets.
        /// </summary>
        private void OnEnable()
        {
            UpdateData();
        }

        /// <summary>
        ///     Initialises the asset object with relevant data and updates the UI.
        /// </summary>
        public void Initialise(AssetModel asset)
        {
            m_Asset = asset;
            UpdateData();
        }

        /// <summary>
        ///     Updates the text fields with asset data.
        /// </summary>
        private async void UpdateData()
        {
            if (m_Asset != null)
            {
                m_NameText.text = m_Asset.name;
                m_CollectionText.text = m_Asset.contract_address;
                m_Image.LoadUrl(m_Asset.image);
            }
        }
    }
}