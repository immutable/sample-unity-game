using System;
using System.Globalization;
using System.Numerics;
using Cysharp.Threading.Tasks;
using Immutable.Orderbook.Api;
using Immutable.Orderbook.Client;
using Immutable.Passport;

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
            
            // Convert hex to dec
            var balanceDec = BigInteger.Parse(balanceHex.Replace("0x", ""), NumberStyles.HexNumber);
            
            // Convert smallest unit to main unit
            var balance = BigInteger.Divide(balanceDec, BigInteger.Pow(10, 18));
            
            // Round to two decimal places
            var decimalValue = (decimal)balance;
            var roundedValue = Math.Round(decimalValue, 2);
            
            return roundedValue.ToString("F2");
        }
    }
}
