using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Vuplex.WebView;

namespace HyperCasual.Runner
{
    /// <summary>
    /// Loads the Transak on-ramp widget, allowing players to purchase tokens 
    /// using fiat currencies (e.g., USD).
    /// </summary>
    public class TransakView : MonoBehaviour
    {
        [SerializeField] Button m_CloseButton;
        [SerializeField] CanvasWebViewPrefab m_WebViewPrefab;

        private string m_Url = "https://www.immutable.com";

        public void Show(string url, UnityAction onClose)
        {
            m_CloseButton.onClick.RemoveListener(onClose);
            m_CloseButton.onClick.AddListener(onClose);

            m_Url = url;
            m_WebViewPrefab.InitialUrl = m_Url;

            gameObject.SetActive(true);
        }
    }
}
