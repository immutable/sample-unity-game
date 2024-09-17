using System;
using System.Net.Http;
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
        ///     Gets the player's balance
        /// </summary>
        public async void UpdateBalance()
        {
            Debug.Log("Fetching player's balance...");

            gameObject.SetActive(false);
            m_ValueText.text = "- IMR";

            try
            {
                var address = SaveManager.Instance.WalletAddress;

                if (string.IsNullOrEmpty(address))
                {
                    Debug.LogError("Could not get player's wallet");
                    return;
                }

                using var client = new HttpClient();
                var response = await client.GetAsync($"http://localhost:6060/balance?address={address}");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log($"Balance response: {responseBody}");
                    var balanceResponse = JsonUtility.FromJson<BalanceResponse>(responseBody);
                    if (balanceResponse?.quantity != null) m_ValueText.text = $"{balanceResponse.quantity} IMR";
                }
                else
                {
                    Debug.Log("Failed to get balance");
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to get balance: {ex.Message}");
            }

            gameObject?.SetActive(true);
        }
    }
}