using System;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HyperCasual.Runner
{
    public class OrderbookUseCase
    {
        private static readonly Lazy<OrderbookUseCase> s_Instance = new(() => new OrderbookUseCase());

        private OrderbookUseCase() { }

        public static OrderbookUseCase Instance => s_Instance.Value;

        /// <summary>
        /// Confirms the status of a listing by repeatedly polling until it matches the specified status or the operation times out.
        /// </summary>
        /// <param name="listingId">The unique identifier of the listing.</param>
        /// <param name="desiredStatus">The target status to be matched (e.g., "ACTIVE", "FILLED" or "CANCELLED").</param>
        /// <returns>
        /// A <see cref="UniTask"/> that completes when the listing reaches the desired status or the operation times out.
        /// </returns>
        public async UniTask ConfirmListingStatus(string listingId, string desiredStatus)

        {
            const int timeoutDuration = 60000; // Timeout duration in milliseconds
            const int pollDelay = 2000; // Delay between polls in milliseconds

            using var client = new HttpClient();
            var startTimeMs = Time.time * 1000;
            var url = $"{Config.BASE_URL}/v1/chains/{Config.CHAIN_NAME}/orders/listings/{listingId}";

            while (true)
            {
                if (Time.time * 1000 - startTimeMs > timeoutDuration)
                {
                    Debug.Log($"Failed to confirm listing status: {desiredStatus}.");
                    return;
                }

                try
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var listingResponse = JsonUtility.FromJson<ListingResponse>(responseBody);

                        if (listingResponse.result?.status.name == desiredStatus)
                        {
                            Debug.Log($"Listing {listingId} is {desiredStatus}.");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                await UniTask.Delay(pollDelay);
            }
        }
    }
}
