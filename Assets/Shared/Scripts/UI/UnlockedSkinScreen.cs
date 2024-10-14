using System.ComponentModel;
using HyperCasual.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Immutable.Passport;
using Immutable.Passport.Model;


[Serializable]
public class GetAssetsResponse
{
    public Asset[] result;
}

[Serializable]
public class Asset
{
    public string id;
    public string token_id;
    public string token_address;
}

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

        private async void Craft()
        {
            try {
                m_CraftState = CraftSkinState.Crafting;

                // burn
                Asset[] assets = await GetAssets();
                if (assets.Length == 0)
                {
                    Debug.Log("No assets to burn");
                    m_CraftState = CraftSkinState.Failed;
                    return;
                }

                CreateTransferResponseV1 transferResult = await Passport.Instance.ImxTransfer(
                    new UnsignedTransferRequest("ERC721", "1", "0x0000000000000000000000000000000000000000", assets[0].token_id, assets[0].token_address)
                );
                Debug.Log($"Transfer(id={transferResult.transfer_id} receiver={transferResult.receiver} status={transferResult.status})");

                m_CraftState = CraftSkinState.Crafted;

                // If successfully crafted skin and this screen is visible, go to collect skin screen
                // otherwise it will be picked in the OnEnable function above when this screen reappears
                if (m_CraftState == CraftSkinState.Crafted && gameObject.active)
                {
                    CollectSkin();
                }
            } catch (Exception ex) {
                Debug.Log($"Failed to craft skin: {ex.Message}");
                m_CraftState = CraftSkinState.Failed;
            }
        }

        private async UniTask<Asset[]> GetAssets()
        {
            const string collection = "0xcf77af96b269169f149b3c23230e103bda67fd0c";
            string address = await Passport.Instance.GetAddress();
            Debug.Log($"Wallet address: {address}");

            if (address != null)
            {
                using var client = new HttpClient();
                string url = $"https://api.sandbox.x.immutable.com/v1/assets?user={address}&collection={collection}&status=imx";
                using var req = new HttpRequestMessage(HttpMethod.Get, url);
                using var res = await client.SendAsync(req);

                // Parse JSON and extract token_id
                string content = await res.Content.ReadAsStringAsync();
                Debug.Log($"Get Assets response: {content}");

                GetAssetsResponse body = JsonUtility.FromJson<GetAssetsResponse>(content);
                Debug.Log($"Get Assets result: {body.result.Length}");
                return body.result;
            }
            return null;
        }

        private void CollectSkin()
        {
            m_CollectSkinEvent.Raise();
            CraftState = null;
        }

        private void OnCraftButtonClicked()
        {
            // Craft in the background, while the player plays the next level
            Craft();
            // m_NextLevelEvent.Raise();
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
