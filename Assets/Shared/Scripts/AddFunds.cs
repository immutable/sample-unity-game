using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Immutable.Marketplace.OnRamp;
using Immutable.Passport;
using Xsolla.Core;

namespace HyperCasual.Runner
{
    /// <summary>
    /// This View contains celebration screen functionalities
    /// </summary>
    public class AddFunds : MonoBehaviour
    {
        [SerializeField]
        Button m_FiatButton;
        [SerializeField]
        Button m_TokenButton;
        [SerializeField]
        Button m_CancelButton;
        public void Show()
        {
            m_FiatButton.onClick.AddListener(OnFiatButtonClicked);
            m_TokenButton.onClick.AddListener(OnTokenButtonClicked);
            m_CancelButton.onClick.AddListener(OnCancelButtonClicked);
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

            XsollaWebBrowser.Open(link);
        }

        private void OnTokenButtonClicked()
        {
            Application.OpenURL($"https://checkout-playground.sandbox.immutable.com/embedded/add-funds/?publishableKey=pk_imapik-test-Xdera%40&passportClientId={Config.CLIENT_ID}&redirectUri={Config.REDIRECT_URI}");
        }

        private void OnCancelButtonClicked()
        {
            gameObject.SetActive(false);
        }
    }
}
