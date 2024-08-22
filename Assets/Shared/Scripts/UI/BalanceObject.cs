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
    public class BalanceObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_ValueText;

        /// <summary>
        /// Gets the player's balance
        /// </summary>
        public async void UpdateBalance()
        {
            Debug.Log("Fetching player's balance...");

            gameObject.SetActive(false);
            m_ValueText.text = "- IMR";

            try
            {
                string address = await GetWalletAddress();

                if (string.IsNullOrEmpty(address))
                {
                    Debug.LogError("Could not get player's wallet");
                    return;
                }

                using var client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync($"http://localhost:6060/balance?address={address}");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log($"Balance response: {responseBody}");
                    BalanceResponse balanceResponse = JsonUtility.FromJson<BalanceResponse>(responseBody);
                    if (balanceResponse?.quantity != null)
                    {
                        m_ValueText.text = $"{balanceResponse.quantity} IMR";
                    }
                }
                else
                {
                    Debug.Log($"Failed to get balance");
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to get balance: {ex.Message}");
            }

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Retrieves the players's wallet address.
        /// </summary>
        private async UniTask<string> GetWalletAddress()
        {
            List<string> accounts = await Passport.Instance.ZkEvmRequestAccounts();
            return accounts.Count > 0 ? accounts[0] : string.Empty; // Return the first wallet address
        }
    }
}