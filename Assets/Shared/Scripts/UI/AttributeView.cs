using Immutable.Search.Model;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     Represents an asset's attribute
    /// </summary>
    public class AttributeView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Type;
        [SerializeField] private TextMeshProUGUI m_Value;

        public async void Initialise(NFTMetadataAttribute attribute)
        {
            m_Type.text = attribute.TraitType;
            m_Value.text = attribute.Value.GetString();
        }
    }
}