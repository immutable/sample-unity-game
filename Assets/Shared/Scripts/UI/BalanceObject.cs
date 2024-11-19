#nullable enable
using System;
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
        [SerializeField] private TextMeshProUGUI m_ValueText;

        /// <summary>
        /// Retrieves the player's balance.
        /// </summary>
        /// <returns>The player's balance as a string, or null if the balance could not be fetched.</returns>
        public async UniTask<string?> UpdateBalance()
        {
            Debug.Log("Fetching player's balance...");

            gameObject.SetActive(false);
            m_ValueText.text = "- IMR";

            try
            {
                var balance = await GetTokenBalanceUseCase.Instance.GetBalance();
                m_ValueText.text = $"{balance} IMR";

                if (gameObject != null) gameObject.SetActive(true);

                return balance == "0.0" ? "0" : balance;
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to get balance: {ex.Message}");
            }

            return null;
        }
    }
}