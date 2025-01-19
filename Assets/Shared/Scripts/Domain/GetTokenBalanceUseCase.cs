using System;
using System.Globalization;
using System.Numerics;
using Cysharp.Threading.Tasks;
using Immutable.Orderbook.Api;
using Immutable.Orderbook.Client;
using Immutable.Passport;
using Nethereum.Web3;

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

        /// <summary>
        /// Gets the user's USDC balance
        /// </summary>
        public async UniTask<string> GetUsdcBalance()
        {
            var web3 = new Web3(Config.RPC_URL);

            var abi = @"[{'constant':true,'inputs':[{'name':'account','type':'address'}],'name':'balanceOf','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'}]";

            var contract = web3.Eth.GetContract(abi, Contract.USDC);

            var balanceOfFunction = contract.GetFunction("balanceOf");

            var balance = await balanceOfFunction.CallAsync<BigInteger>(SaveManager.Instance.WalletAddress);

            var quantity = Web3.Convert.FromWei(balance, 6);

            return quantity.ToString("F2");
        }
    }
}
