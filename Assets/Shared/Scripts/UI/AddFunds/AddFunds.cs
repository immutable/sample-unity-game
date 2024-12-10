using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Immutable.Marketplace.OnRamp;
using Immutable.Passport;

namespace HyperCasual.Runner
{
    /// <summary>
    /// Provides various options for players to add funds to the game.
    /// </summary>
    public class AddFunds : MonoBehaviour
    {
        [SerializeField] GameObject m_Options;
        [SerializeField] Button m_FiatButton;
        [SerializeField] Button m_TokenButton;
        [SerializeField] Button m_CancelButton;
        
        [SerializeField] TransakView m_TransakView;

        public void Show()
        {
            m_FiatButton.onClick.RemoveListener(OnFiatButtonClicked);
            m_FiatButton.onClick.AddListener(OnFiatButtonClicked);
            
            m_TokenButton.onClick.RemoveListener(OnTokenButtonClicked);
            m_TokenButton.onClick.AddListener(OnTokenButtonClicked);
            
            m_CancelButton.onClick.RemoveListener(OnCancelButtonClicked);
            m_CancelButton.onClick.AddListener(OnCancelButtonClicked);
            
            m_Options.gameObject.SetActive(true);
            m_TransakView.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        private async void OnFiatButtonClicked()
        {
            string environment = Immutable.Passport.Model.Environment.SANDBOX;
            string email = await Passport.Instance.GetEmail();
            List<string> walletAddress = await Passport.Instance.ZkEvmRequestAccounts();
            OnRamp onRamp = new OnRamp(environment, email, walletAddress.FirstOrDefault());
            string link = await onRamp.GetLink();
            Debug.Log($"onRamp.GetOnRampLink: {link}");

            m_TransakView.Show($"https://global-stg.transak.com/?apiKey=d14b44fb-0f84-4db5-affb-e044040d724b&network=immutablezkevm&defaultPaymentMethod=credit_debit_card&disablePaymentMethods=&productsAvailed=buy&exchangeScreenTitle=Buy&themeColor=0D0D0D&defaultCryptoCurrency=IMX&defaultFiatAmount=50&defaultFiatCurrency=usd&walletAddress={walletAddress.FirstOrDefault()}&cryptoCurrencyList=imx%2Ceth%2Cusdc", () =>
            {
                m_Options.gameObject.SetActive(true);
                m_TransakView.gameObject.SetActive(false);
            });
        }

        private void OnTokenButtonClicked()
        {
            Application.OpenURL($"https://checkout-playground.sandbox.immutable.com/embedded/add-funds/?publishableKey=pk_imapik-test-Xdera%40&passportClientId={Config.CLIENT_ID}&redirectUri={Config.REDIRECT_URI}");
        }

        private void OnCancelButtonClicked()
        {
            gameObject.SetActive(false);
            m_Options.gameObject.SetActive(true);
            m_TransakView.gameObject.SetActive(false);
        }
    }
}
