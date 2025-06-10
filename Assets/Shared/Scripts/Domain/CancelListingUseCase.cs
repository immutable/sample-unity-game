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
    public class CancelListingUseCase
    {
        private static readonly Lazy<CancelListingUseCase> s_Instance = new(() => new CancelListingUseCase());

        private readonly OrderbookApi m_OrderbookApi = new(new Configuration { BasePath = Config.BASE_URL });

        private CancelListingUseCase()
        {
        }

        public static CancelListingUseCase Instance => s_Instance.Value;

        /// <summary>
        ///     Cancels the specified listing.
        /// </summary>
        /// <param name="listingId">The unique identifier of the listing to cancel.</param>
        /// <param name="confirmListing">
        ///     If true, the function will poll the listing endpoint to confirm that the listing status
        ///     has changed to "CANCELLED". If false, the function will not verify the listing status.
        /// </param>
        public async UniTask CancelListing(string listingId, bool confirmListing = true)
        {
            try
            {
                var request = new CancelOrdersOnChainRequest(
                    accountAddress: SaveManager.Instance.WalletAddress, orderIds: new List<string> { listingId });

                var response = await m_OrderbookApi.CancelOrdersOnChainAsync(request);
                var transactionAction = response?.CancellationAction.PopulatedTransactions;

                if (transactionAction?.To == null)
                    throw new Exception("Failed to cancel listing.");

                var txResponse = await Passport.Instance.ZkEvmSendTransactionWithConfirmation(
                    new TransactionRequest
                    {
                        to = transactionAction.To,
                        data = transactionAction.Data,
                        value = "0"
                    });

                if (txResponse.status != "1")
                    throw new Exception("Failed to cancel listing.");

                if (confirmListing) await OrderbookUseCase.Instance.ConfirmListingStatus(listingId, "CANCELLED");
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                throw;
            }
        }

        /// <summary>
        ///     Handles API exceptions by logging relevant details.
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