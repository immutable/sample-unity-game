using System.ComponentModel;
using HyperCasual.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Immutable.Passport;
using Immutable.Passport.Model;

namespace HyperCasual.Runner
{
    /// <summary>
    /// This View contains celebration screen functionalities
    /// </summary>
    public class UnlockedSkinScreen : View
    {
        [SerializeField]
        GameObject m_Title;
        [SerializeField]
        GameObject m_Loading;
        [SerializeField]
        TextMeshProUGUI m_ErrorMessage;
        [SerializeField]
        HyperCasualButton m_CraftButton;
        [SerializeField]
        HyperCasualButton m_NextButton;
        [SerializeField]
        HyperCasualButton m_TryAgainButton;
        [SerializeField]
        AbstractGameEvent m_NextLevelEvent;
        [SerializeField]
        AbstractGameEvent m_CollectSkinEvent;

        public enum CraftSkinState
        {
            Crafting, // Crafting a skin
            Crafted, // Successfully crafted a skin
            Failed // Failed to craft
        }

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
                    default:
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
                    await Task.Delay(TimeSpan.FromMilliseconds(1));
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

        private async void Craft(List<TokenModel> tokens)
        {
            CraftState = CraftSkinState.Crafting;

            try
            {
                // Burn tokens and mint a new skin i.e. crafting a skin
                if (SaveManager.Instance.ZkEvm)
                {
                    string encodedData = await ApiService.GetTokenCraftSkinEcodedData(tokens[0].token_id, tokens[1].token_id, tokens[2].token_id);
                    TransactionReceiptResponse response = await Passport.Instance.ZkEvmSendTransactionWithConfirmation(new TransactionRequest()
                    {
                        to = Config.ZK_TOKEN_TOKEN_ADDRESS,
                        data = encodedData,
                        value = "0"
                    });
                    Debug.Log($"Craft transaction hash: {response.transactionHash}");

                    if (response.status != "1")
                    {
                        m_ErrorMessage.text = "Failed to craft your skin :(";
                        CraftState = CraftSkinState.Failed;
                        return;
                    }
                }
                else
                {
                    // Burn tokens first
                    string burnAddress = "0x0000000000000000000000000000000000000000";
                    var response = await Passport.Instance.ImxBatchNftTransfer(
                        tokens.Select(token => new NftTransferDetails(burnAddress, token.token_id, Config.TOKEN_TOKEN_ADDRESS.ToLower()))
                        .ToArray());
                    Debug.Log($"Successfully burned {response.transfer_ids.Length} coins.");

                    // Mint skin
                    bool mintedSkin = await ApiService.MintSkin(SaveManager.Instance.WalletAddress);

                    if (!mintedSkin)
                    {
                        Debug.Log($"Something went wrong while minting sking");
                        m_ErrorMessage.text = "Failed to craft your skin :(";
                        CraftState = CraftSkinState.Failed;
                        return;
                    }
                }

                CraftState = CraftSkinState.Crafted;

                // If successfully crafted skin and this screen is visible, go to collect skin screen
                // otherwise it will be picked in the OnEnable function above when this screen reappears
                if (m_CraftState == CraftSkinState.Crafted && gameObject.active)
                {
                    CollectSkin();
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Something went wrong while crafting {ex.Message}");
                m_ErrorMessage.text = "Failed to craft your skin :(";
                CraftState = CraftSkinState.Failed;
            }
        }

        private void CollectSkin()
        {
            m_CollectSkinEvent.Raise();
            CraftState = null;
        }

        private async void OnCraftButtonClicked()
        {
            CraftState = CraftSkinState.Crafting;

            // Check if user has enough tokens to craft a skin
            List<TokenModel> tokens = await ApiService.GetTokens(3, SaveManager.Instance.WalletAddress);
            if (tokens.Count > 0)
            {
                m_ErrorMessage.text = "You do not have enough tokens :(";
                CraftState = CraftSkinState.Failed;
                m_TryAgainButton.gameObject.SetActive(false);
                return;
            }

            m_NextLevelEvent.Raise();
            // Craft in the background, while the player plays the next level
            Craft(tokens);
        }

        private void OnTryAgainButtonClicked()
        {
            OnCraftButtonClicked();
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
