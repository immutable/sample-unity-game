using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     Handles the downloading and display of an image from a URL using a RawImage.
    /// </summary>
    public class ImageUrlObject : MonoBehaviour
    {
        [SerializeField] private RawImage m_Image;

        private void OnDisable()
        {
            m_Image.texture = null;
        }

        /// <summary>
        ///     Downloads an image from the provided URL and assigns it to the RawImage.
        ///     If the download fails or the URL is invalid, the image is hidden.
        /// </summary>
        /// <param name="url">The URL of the image to download.</param>
        public async UniTask LoadUrl(string url)
        {
            if (m_Image == null || string.IsNullOrEmpty(url)) return;

            m_Image.gameObject.SetActive(false);

            using var request = UnityWebRequestTexture.GetTexture(url);
            await request.SendWebRequest();

            // Ensure m_Image wasn't destroyed during the async operation.
            if (m_Image == null) return;

            if (request.result == UnityWebRequest.Result.Success)
            {
                var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                m_Image.texture = texture;
                m_Image.gameObject.SetActive(true);
            }
        }
    }
}