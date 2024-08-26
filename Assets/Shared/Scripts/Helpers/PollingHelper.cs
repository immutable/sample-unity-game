using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Net.Http;

namespace HyperCasual.Runner
{
    public static class PollingHelper
    {
        /// <summary>
        /// Polls a given URL until a condition is met or the operation times out.
        /// </summary>
        /// <param name="url">The URL to poll.</param>
        /// <param name="condition">A function that takes the response body as input and returns true if the condition is met.</param>
        /// <param name="pollIntervalMs">The polling interval in milliseconds (default is 2000 ms).</param>
        /// <param name="timeoutMs">The timeout duration in milliseconds (default is 60000 ms).</param>
        /// <returns>Returns true if the condition was met before timing out, false otherwise.</returns>
        public static async UniTask<bool> PollAsync(string url, Func<string, bool> condition, int pollIntervalMs = 2000, int timeoutMs = 60000)
        {
            using var client = new HttpClient();
            float startTimeMs = Time.time * 1000;
            bool conditionMet = false;

            while (!conditionMet)
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        conditionMet = condition(responseBody);
                    }
                    else
                    {
                        Debug.LogWarning($"Polling received non-success status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                if (!conditionMet)
                {
                    // Check if timeout has been reached
                    if ((Time.time * 1000) - startTimeMs > timeoutMs)
                    {
                        Debug.LogWarning("Polling timed out.");
                        return false;
                    }

                    await UniTask.Delay(pollIntervalMs); // Wait for the specified polling interval before checking again
                }
            }

            Debug.Log("Condition met, polling completed.");
            return true;
        }
    }
}