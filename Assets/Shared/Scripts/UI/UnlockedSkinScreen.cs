using System;
using Cysharp.Threading.Tasks;
using HyperCasual.Core;
using Immutable.Passport;
using Immutable.Passport.Model;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     This View contains celebration screen functionalities
    /// </summary>
    public class UnlockedSkinScreen : View
    {
        public enum CraftSkinState
        {
            Crafting, // Crafting a skin
            Crafted, // Successfully crafted a skin
            Failed // Failed to craft
        }

        [SerializeField] private GameObject m_Title;

        [SerializeField] private GameObject m_Loading;

        [SerializeField] private TextMeshProUGUI m_ErrorMessage;

        [SerializeField] private HyperCasualButton m_CraftButton;

        [SerializeField] private HyperCasualButton m_NextButton;

        [SerializeField] private HyperCasualButton m_TryAgainButton;

        [SerializeField] private AbstractGameEvent m_NextLevelEvent;

        [SerializeField] private AbstractGameEvent m_CollectSkinEvent;

        private CraftSkinState? m_CraftState;

        public CraftSkinState? CraftState
        {
            get => m_CraftState;
            set
            {
                m_CraftState = value;
                switch (m_CraftState)
                {
                    case CraftSkinState.Crafting:
                        Debug.Log("Crafting new skin...");
                        ShowLoading(true);
                        ShowCraftButton(false);
                        ShowNextButton(false);
                        ShowError(false);
                        break;
                    case CraftSkinState.Crafted:
                        Debug.Log("Crafted new skin!");
                        ShowLoading(false);
                        ShowCraftButton(true);
                        ShowNextButton(true);
                        ShowError(false);
                        break;
                    case CraftSkinState.Failed:
                        Debug.Log("Failed to craft new skin!");
                        ShowLoading(false);
                        ShowCraftButton(false);
                        ShowNextButton(true);
                        ShowError(true);
                        break;
                }
            }
        }

        public async void OnEnable()
        {
            // Set listener to 'Next' button
            m_NextButton.RemoveListener(OnNextButtonClicked);
            m_NextButton.AddListener(OnNextButtonClicked);

            // Set listener to "Burn 3 coins to collect at the next level" (i.e. craft) button
            m_CraftButton.RemoveListener(OnCraftButtonClicked);
            m_CraftButton.AddListener(OnCraftButtonClicked);

            // Set listener to "Try Again" button
            m_TryAgainButton.RemoveListener(OnTryAgainButtonClicked);
            m_TryAgainButton.AddListener(OnTryAgainButtonClicked);

            switch (m_CraftState)
            {
                case CraftSkinState.Crafting:
                    break;
                case CraftSkinState.Crafted:
                    // Skin crafted successfully, go to collect skin screen
                    // Need to add some delay otherwise game even won't get triggered
                    await UniTask.Delay(TimeSpan.FromMilliseconds(1), DelayType.Realtime);
                    CollectSkin();
                    break;
                case CraftSkinState.Failed:
                    break;
                default:
                    // There's no craft state, so reset screen to initial state
                    ShowLoading(false);
                    ShowCraftButton(true);
                    ShowNextButton(true);
                    ShowError(false);
                    break;
            }
        }

        private async void Craft()
        {
            m_CraftState = CraftSkinState.Crafting;

            // Burn tokens and mint a new skin i.e. crafting a skin
            var transactionHash = await Passport.Instance.ZkEvmSendTransaction(new TransactionRequest
            {
                to = Contract.TOKEN, // Immutable Runner Token contract address
                data = "0x1e957f1e", // Call craftSkin() in the contract
                value = "0"
            });
            Debug.Log($"Craft transaction hash: {transactionHash}");

            m_CraftState = CraftSkinState.Crafted;

            // If successfully crafted skin and this screen is visible, go to collect skin screen
            // otherwise it will be picked in the OnEnable function above when this screen reappears
            if (m_CraftState == CraftSkinState.Crafted && gameObject.active) CollectSkin();
        }

        private void CollectSkin()
        {
            m_CollectSkinEvent.Raise();
            m_CraftState = null;
        }

        private void OnCraftButtonClicked()
        {
            m_NextLevelEvent.Raise();
            // Craft in the background, while the player plays the next level
            Craft();
        }

        private void OnTryAgainButtonClicked()
        {
            Craft();
        }

        private void OnNextButtonClicked()
        {
            m_NextLevelEvent.Raise();
        }

        private void ShowCraftButton(bool show)
        {
            m_CraftButton.gameObject.SetActive(show);
        }

        private void ShowNextButton(bool show)
        {
            m_NextButton.gameObject.SetActive(show);
        }

        private void ShowLoading(bool show)
        {
            m_Loading.gameObject.SetActive(show);
        }

        private void ShowError(bool show)
        {
            m_TryAgainButton.gameObject.SetActive(show);
            m_ErrorMessage.gameObject.SetActive(show);
        }
    }
}