using System;
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

        private Action m_OnDismiss;

        public void Show(Action onDismiss)
        {
            m_OnDismiss = onDismiss;
            
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
            const string environment = Immutable.Passport.Model.Environment.SANDBOX;
            var email = await Passport.Instance.GetEmail();
            var walletAddress = await Passport.Instance.ZkEvmRequestAccounts();
            
            var onRamp = new OnRamp(environment, email, walletAddress.FirstOrDefault());
            var link = await onRamp.GetLink();
            Debug.Log($"onRamp.GetOnRampLink: {link}");

            m_TransakView.Show(link, () =>
            {
                m_Options.gameObject.SetActive(true);
                m_TransakView.gameObject.SetActive(false);
            });
        }

        private void OnTokenButtonClicked()
        {
            Application.OpenURL($"https://checkout-playground.sandbox.immutable.com/checkout/swap/?publishableKey=pk_imapik-test-7-hfC5T$W$eEDE8Mc5mp&fromTokenAddress={Contract.USDC}&toTokenAddress={Contract.TOKEN}");
        }

        private void OnCancelButtonClicked()
        {
            gameObject.SetActive(false);
            m_Options.gameObject.SetActive(true);
            m_TransakView.gameObject.SetActive(false);
            m_OnDismiss();
        }
    }
}
