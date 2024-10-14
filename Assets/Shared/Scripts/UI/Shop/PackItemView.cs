using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     Represents a pack's item
    /// </summary>
    public class PackItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_ItemName;
        [SerializeField] private TextMeshProUGUI m_ItemAmount;
        [SerializeField] private ImageUrlObject m_ItemImage;

        public void Initialise(PackItem item)
        {
            m_ItemName.text = item.name;
            m_ItemAmount.text = $"x{item.amount.ToString()}";
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            m_ItemImage.LoadUrl(item.image);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}