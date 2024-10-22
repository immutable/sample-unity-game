#nullable enable
using System;
using System.Net.Http;
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
                using var client = new HttpClient();
                var response = await client.GetAsync($"{Config.SERVER_URL}/balance?address={SaveManager.Instance.WalletAddress}");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log($"Balance response: {responseBody}");

                    var balanceResponse = JsonUtility.FromJson<BalanceResponse>(responseBody);
                    if (balanceResponse?.quantity != null) m_ValueText.text = $"{balanceResponse.quantity} IMR";

                    if (gameObject != null) gameObject.SetActive(true);

                    return balanceResponse?.quantity == "0.0" ? "0" : balanceResponse?.quantity;
                }

                Debug.LogWarning("Failed to retrieve the balance.");
                return null;
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to get balance: {ex.Message}");
            }

            return null;
        }
    }
}