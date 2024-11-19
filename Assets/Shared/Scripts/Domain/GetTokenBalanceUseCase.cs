using System;
using Cysharp.Threading.Tasks;
using Immutable.Orderbook.Api;
using Immutable.Orderbook.Client;

namespace HyperCasual.Runner
{
    public class GetTokenBalanceUseCase
    {
        private static readonly Lazy<GetTokenBalanceUseCase> s_Instance = new(() => new GetTokenBalanceUseCase());

        private readonly OrderbookApi m_OrderbookApi = new(new Configuration { BasePath = Config.BASE_URL });

        private GetTokenBalanceUseCase() { }

        public static GetTokenBalanceUseCase Instance => s_Instance.Value;

        /// <summary>
        /// Gets the user's ERC20 token balance
        /// </summary>
        public async UniTask<string> GetBalance()
        {
            var result = await m_OrderbookApi.TokenBalanceAsync(
                walletAddress: SaveManager.Instance.WalletAddress,
                contractAddress: Contract.TOKEN
            );
            return result.Quantity;
        }
    }
}
