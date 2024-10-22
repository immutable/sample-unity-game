using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Immutable.Orderbook.Api;
using Immutable.Orderbook.Client;
using Immutable.Orderbook.Model;
using Immutable.Passport;
using Immutable.Passport.Model;
using Newtonsoft.Json;
using UnityEngine;
using ERC1155Item = Immutable.Orderbook.Model.ERC1155Item;
using ERC20Item = Immutable.Orderbook.Model.ERC20Item;
using ERC721Item = Immutable.Orderbook.Model.ERC721Item;

namespace HyperCasual.Runner
{
    public class CreateOrderUseCase
    {
        private static readonly Lazy<CreateOrderUseCase> s_Instance = new(() => new CreateOrderUseCase());

        private readonly OrderbookApi m_OrderbookApi = new(new Configuration { BasePath = Config.BASE_URL });

        private CreateOrderUseCase() { }

        public static CreateOrderUseCase Instance => s_Instance.Value;

        /// <summary>
        /// Creates a new listing for the specified NFT.
        /// </summary>
        /// <param name="contractAddress">The address of the NFT's contract.</param>
        /// <param name="contractType">The type of the contract (e.g., "ERC721" or "ERC1155").</param>
        /// <param name="tokenId">The ID of the NFT.</param>
        /// <param name="price">
        /// The sale price of the NFT, represented as a string amount in IMR (scaled by 10^18).
        /// </param>
        /// <param name="amountToSell">
        /// The quantity of the NFT to sell. "1" for ERC721 tokens and a higher number for ERC1155 tokens.
        /// </param>
        /// <param name="buyContractAddress">
        /// The contract address of the token used to purchase the NFT.
        /// </param>
        /// <param name="confirmListing">
        /// If true, the function will continuously poll the marketplace endpoint to ensure the listing status 
        /// updates to "ACTIVE" upon creation. If false, the function will not verify the listing status.
        /// </param>
        /// <returns>
        /// A <see cref="UniTask{String}"/> that returns the listing ID if the sale is successfully created.
        /// </returns>
        public async UniTask<string> CreateListing(
            string contractAddress, string contractType, string tokenId,
            string price, string amountToSell, string buyContractAddress, bool confirmListing = true)
        {
            try
            {
                if (contractType == "ERC721" && amountToSell != "1")
                {
                    throw new ArgumentException("Invalid arguments: 'amountToSell' must be '1' when listing an ERC721.");
                }

                var listingData = await PrepareListing(contractAddress, contractType, tokenId, price, amountToSell, buyContractAddress);

                await SignAndSubmitApproval(listingData);

                var signature = await SignListing(listingData);

                var listingId = await ListAsset(signature, listingData);

                if (confirmListing) await OrderbookUseCase.Instance.ConfirmListingStatus(listingId, "ACTIVE");

                return listingId;
            }
            catch (ApiException e)
            {
                HandleApiException(e);
                throw;
            }
        }

        /// <summary>
        /// Prepares a listing for the specified NFT and purchase details.
        /// </summary>
        private async UniTask<PrepareListing200Response> PrepareListing(
            string contractAddress, string contractType, string tokenId,
            string price, string amountToSell, string buyContractAddress)
        {
            var sellRequest = CreateSellRequest(contractType, contractAddress, tokenId, amountToSell);
            var buyRequest = new ERC20Item(price, buyContractAddress);

            return await m_OrderbookApi.PrepareListingAsync(new PrepareListingRequest(
                makerAddress: SaveManager.Instance.WalletAddress,
                sell: sellRequest,
                buy: new PrepareListingRequestBuy(buyRequest)
            ));
        }

        /// <summary>
        /// Creates the appropriate sell request based on the contract type.
        /// </summary>
        private static PrepareListingRequestSell CreateSellRequest(
            string contractType, string contractAddress, string tokenId, string amountToSell)
        {
            return contractType.ToUpper() switch
            {
                "ERC1155" => new PrepareListingRequestSell(new ERC1155Item(amountToSell, contractAddress, tokenId)),
                "ERC721" => new PrepareListingRequestSell(new ERC721Item(contractAddress, tokenId)),
                _ => throw new Exception($"Unsupported contract type: {contractType}")
            };
        }

        /// <summary>
        /// Signs and submits approval if required by the listing.
        /// </summary>
        private async UniTask SignAndSubmitApproval(PrepareListing200Response listingData)
        {
            var transactionAction = listingData.Actions
                .FirstOrDefault(action => action.ActualInstance is TransactionAction)?
                .GetTransactionAction();

            if (transactionAction == null) return;

            var response = await Passport.Instance.ZkEvmSendTransactionWithConfirmation(
                new TransactionRequest
                {
                    to = transactionAction.PopulatedTransactions.To,
                    data = transactionAction.PopulatedTransactions.Data,
                    value = "0"
                });

            if (response.status != "1")
                throw new Exception("Failed to sign and submit approval.");
        }

        /// <summary>
        /// Signs the listing with the user's wallet.
        /// </summary>
        private async UniTask<string> SignListing(PrepareListing200Response listingData)
        {
            var signableAction = listingData.Actions
                .FirstOrDefault(action => action.ActualInstance is SignableAction)?
                .GetSignableAction();

            if (signableAction == null)
                throw new Exception("No valid listing to sign.");

            var messageJson = JsonConvert.SerializeObject(signableAction.Message, Formatting.Indented);
            return await Passport.Instance.ZkEvmSignTypedDataV4(messageJson);
        }

        /// <summary>
        /// Finalises the listing and returns the listing ID.
        /// </summary>
        private async UniTask<string> ListAsset(string signature, PrepareListing200Response listingData)
        {
            var response = await m_OrderbookApi.CreateListingAsync(new CreateListingRequest(
                new List<FeeValue>(),
                listingData.OrderComponents,
                listingData.OrderHash,
                signature
            ));
            return response.Result.Id;
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
