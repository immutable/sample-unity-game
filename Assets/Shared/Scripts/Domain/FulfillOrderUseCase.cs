using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Immutable.Orderbook.Api;
using Immutable.Orderbook.Client;
using Immutable.Orderbook.Model;
using Immutable.Passport;
using Immutable.Passport.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace HyperCasual.Runner
{
    public class FulfillOrderUseCase
    {
        private static readonly Lazy<FulfillOrderUseCase> s_Instance = new(() => new FulfillOrderUseCase());

        private readonly OrderbookApi m_OrderbookApi = new(new Configuration { BasePath = Config.BASE_URL });

        private FulfillOrderUseCase() { }

        public static FulfillOrderUseCase Instance => s_Instance.Value;

        /// <summary>
        /// Executes an order by fulfilling a listing and optionally confirming its status.
        /// </summary>
        /// <param name="listingId">The unique identifier of the listing to fulfill.</param>
        /// <param name="fees">The taker fees</param>
        public async UniTask ExecuteOrder(string listingId, List<FulfillOrderRequestTakerFeesInner> fees)
        {
            try
            {
                var request = new FulfillOrderRequest(
                    takerAddress: SaveManager.Instance.WalletAddress,
                    listingId: listingId,
                    takerFees: fees);

                var createListingResponse = await m_OrderbookApi.FulfillOrderAsync(request);

                if (createListingResponse.Actions.Count > 0)
                {
                    foreach (var transaction in createListingResponse.Actions)
                    {
                        var transactionHash = await Passport.Instance.ZkEvmSendTransaction(new TransactionRequest
                        {
                            to = transaction.PopulatedTransactions.To,
                            data = transaction.PopulatedTransactions.Data,
                            value = "0"
                        });
                        Debug.Log($"Transaction hash: {transactionHash}");
                    }
                }
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                throw;
            }
        }

        /// <summary>
        /// Handles API exceptions by logging relevant details.
        /// </summary>
        private static void HandleApiException(ApiException e)
        {
            Debug.LogError($"API Error: {e.Message} (Status: {e.ErrorCode})");
            Debug.LogError(e.ErrorContent);
            Debug.LogError(e.StackTrace);
            var errorModel = JsonConvert.DeserializeObject<ErrorModel>($"{e.ErrorContent}");
            if (errorModel != null) throw new Exception(errorModel.message);
        }
    }
}
