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
    public class FulfilOrderUseCase
    {
        private static readonly Lazy<FulfilOrderUseCase> s_Instance = new(() => new FulfilOrderUseCase());

        private readonly OrderbookApi m_OrderbookApi = new(new Configuration { BasePath = Config.BASE_URL });

        private FulfilOrderUseCase() { }

        public static FulfilOrderUseCase Instance => s_Instance.Value;

        /// <summary>
        /// Executes an order by fulfilling a listing and optionally confirming its status.
        /// </summary>
        /// <param name="listingId">The unique identifier of the listing to fulfil.</param>
        /// <param name="fees">The taker fees</param>
        /// <param name="confirmListing">
        /// If true, the function will poll the listing endpoint to confirm that the listing status 
        /// has changed to "FILLED". If false, the function will not verify the listing status.
        /// </param>
        public async UniTask ExecuteOrder(string listingId, List<FulfillOrderRequestTakerFeesInner> fees, bool confirmListing = true)
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

                if (confirmListing) await OrderbookUseCase.Instance.ConfirmListingStatus(listingId, "FILLED");
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
