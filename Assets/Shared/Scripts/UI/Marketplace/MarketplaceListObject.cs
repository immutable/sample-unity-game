using System.Numerics;
using Cysharp.Threading.Tasks;
using Immutable.Api.Model;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     Represents an individual list item in the marketplace view.
    /// </summary>
    public class MarketplaceListObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private TextMeshProUGUI m_CountText;
        [SerializeField] private ImageUrlObject m_Image;

        private StackBundle m_Stack;

        /// <summary>
        ///     If an order is already assigned, refresh the displayed data
        /// </summary>
        private async void OnEnable()
        {
            if (m_Stack != null) await UpdateData();
        }

        /// <summary>
        ///     Initialises the list item with the provided stack data.
        /// </summary>
        /// <param name="stack">The stack data to display in the UI.</param>
        public async void Initialise(StackBundle stack)
        {
            m_Stack = stack;

            await UpdateData();
        }

        /// <summary>
        ///     Updates the UI elements (name, amount, count, and image) with the order's data.
        /// </summary>
        private async UniTask UpdateData()
        {
            // Display the floor price if available
            if (m_Stack.Market?.FloorListing != null)
            {
                // Format the amount
                var amount = m_Stack.Market.FloorListing.PriceDetails.Amount;
                var quantity = (decimal)BigInteger.Parse(amount) / (decimal)BigInteger.Pow(10, 18);
                m_AmountText.text = $"Floor price: {quantity} IMR";
            }
            else
            {
                m_AmountText.text = "Floor price: N/A";
            }

            // Update name and count of the asset in the stack
            m_NameText.text = m_Stack.Stack.Name;
            m_CountText.text = $"Total count: {m_Stack.StackCount}";

            // Load and display the image
            await m_Image.LoadUrl(m_Stack.Stack.Image);
        }
    }
}