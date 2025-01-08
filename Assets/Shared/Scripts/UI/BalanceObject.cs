#nullable enable
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     Represents an asset in the player's inventory
    /// </summary>
    public class BalanceObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_AddressText;
        [SerializeField] private GameObject m_Panel;
        [SerializeField] private TextMeshProUGUI m_ImrValue;
        [SerializeField] private TextMeshProUGUI m_ImxValue;
        [SerializeField] private TextMeshProUGUI m_UsdcValue;
        
        [SerializeField] private HyperCasualButton m_AddFundsButton;
        [SerializeField] private AddFunds m_AddFunds;

        private Canvas m_RootCanvas;

        private void Start()
        {
            Debug.Log("Balance object start");
            gameObject.SetActive(false);
            m_Panel.SetActive(false);

            // Cache root Canvas
            var rootCanvas = FindObjectsOfType<Canvas>().FirstOrDefault(c => c.name == "Canvas");
            if (rootCanvas == null)
            {
                gameObject.SetActive(false);
                return;
            }

            m_RootCanvas = rootCanvas;

            // Wallet address
            var address = SaveManager.Instance.WalletAddress;
            if (address == null) return;
            m_AddressText.text = address[..6] + "..." + address[^4..];

            // Show panel on wallet address click
            var clickable = m_AddressText.GetComponent<ClickableView>();
            if (clickable != null)
            {
                clickable.ClearAllSubscribers();
                clickable.OnClick += TogglePanel;
            }

            // Add funds button
            m_AddFundsButton.RemoveListener(ShowAddFunds);
            m_AddFundsButton.AddListener(ShowAddFunds);

            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Displays or closes the panel
        /// </summary>
        private void TogglePanel()
        {
            if (m_Panel.activeSelf)
            {
                m_Panel.SetActive(false);
                return;
            }
            
            // Update balance
#pragma warning disable CS4014
            UpdateBalance();
#pragma warning restore CS4014

            // Position the panel
            m_Panel.transform.SetParent(m_RootCanvas.transform, false);
            m_Panel.transform.SetAsLastSibling();

            var panelRectTransform = m_Panel.GetComponent<RectTransform>();
            panelRectTransform.anchorMin = new Vector2(1, 1);
            panelRectTransform.anchorMax = new Vector2(1, 1);
            panelRectTransform.pivot = new Vector2(1, 1);

            panelRectTransform.anchoredPosition = new Vector2(-32, -88);
            
            m_Panel.SetActive(true);
        }

        /// <summary>
        /// Displays various options for players to add funds to the game.
        /// </summary>
        private void ShowAddFunds()
        {
#pragma warning disable CS4014
            m_AddFunds.Show(() => UpdateBalance());
#pragma warning restore CS4014
            
            TogglePanel();
        }

        /// <summary>
        /// Retrieves the player's IMR and IMX balances.
        /// </summary>
        /// <returns>The player's IMR balance as a string, or null if the balance could not be fetched.</returns>
        public async UniTask<string?> UpdateBalance()
        {
            Debug.Log("Fetching player's balance...");

            try
            {
                // IMR balance
                var imrBalance = await GetTokenBalanceUseCase.Instance.GetBalance();
                m_ImrValue.text = imrBalance;
                
                // IMX balance
                var imxBalance = await GetTokenBalanceUseCase.Instance.GetImxBalance();
                m_ImxValue.text = imxBalance;
                
                // USDC balance
                var usdcBalance = await GetTokenBalanceUseCase.Instance.GetUsdcBalance();
                m_UsdcValue.text = usdcBalance;

                return imrBalance == "0.0" ? "0" : imrBalance;
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to get balance: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Closes the panel
        /// </summary>
        public void ClosePanel()
        {
            if (!m_Panel.activeSelf) return;
            m_Panel.SetActive(false);
        }
    }
}
