using System;
using System.Globalization;
using System.Numerics;
using Cysharp.Threading.Tasks;
using Immutable.Orderbook.Api;
using Immutable.Orderbook.Client;
using Immutable.Passport;
using UnityEngine;

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
            var rounded = Math.Round(Convert.ToDecimal(result.Quantity), 2);
            return rounded.ToString("F2");
        }
        
        /// <summary>
        /// Gets the user's IMX balance
        /// </summary>
        public async UniTask<string> GetImxBalance()
        {
            var balanceHex = await Passport.Instance.ZkEvmGetBalance(SaveManager.Instance.WalletAddress);
            
            var balance = BigInteger.Parse(balanceHex.Replace("0x", ""), NumberStyles.HexNumber);

            // Convert from smallest unit to main unit
            var decimalValue = (decimal)balance / 1_000_000_000_000_000_000m;

            // Round to 2 decimal places
            var roundedValue = Math.Round(decimalValue, 2);

            return roundedValue.ToString("F2");
        }
    }
}
