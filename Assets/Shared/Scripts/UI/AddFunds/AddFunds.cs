using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Immutable.Marketplace.Bridge;
using Immutable.Marketplace.OnRamp;
using Immutable.Marketplace.Swap;
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
        [SerializeField] Button m_SwapButton;
        [SerializeField] Button m_BridgeButton;
        [SerializeField] Button m_CancelButton;
        
        [SerializeField] TransakView m_TransakView;

        private Action m_OnDismiss;

        public void Show(Action onDismiss)
        {
            m_OnDismiss = onDismiss;
            
            m_FiatButton.onClick.RemoveListener(OnFiatButtonClicked);
            m_FiatButton.onClick.AddListener(OnFiatButtonClicked);
            
            m_SwapButton.onClick.RemoveListener(OnSwapButtonClicked);
            m_SwapButton.onClick.AddListener(OnSwapButtonClicked);
            
            m_BridgeButton.onClick.RemoveListener(OnBridgeButtonClicked);
            m_BridgeButton.onClick.AddListener(OnBridgeButtonClicked);
            
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
            var link = onRamp.GetLink();
            Debug.Log($"onRamp.GetOnRampLink: {link}");

            m_TransakView.Show(link, () =>
            {
                m_Options.gameObject.SetActive(true);
                m_TransakView.gameObject.SetActive(false);
            });
        }

        private void OnSwapButtonClicked()
        {
            var swap = new Swap(Immutable.Passport.Model.Environment.SANDBOX);
            var link = swap.GetLink(fromTokenAddress: Contract.USDC, toTokenAddress: Contract.TOKEN);
            Application.OpenURL(link);
        }
        
        private void OnBridgeButtonClicked()
        {
            // On supports mainnet
            var bridge = new Bridge(Immutable.Passport.Model.Environment.PRODUCTION);
            var link = bridge.GetLink(toChain: "13371");
            Application.OpenURL(link);
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
