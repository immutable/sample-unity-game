using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace HyperCasual.Runner
{
    public class ImageUrlObject : MonoBehaviour
    {
        [SerializeField] private RawImage m_Image;

        /// <summary>
        /// Downloads and displays the image from the given URL.
        /// </summary>
        public async UniTask LoadUrl(string url)
        {
            m_Image.gameObject.SetActive(false);

            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                await request.SendWebRequest();

                if (m_Image == null)
                {
                    return;
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    m_Image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    m_Image.gameObject.SetActive(true);
                }
                else
                {
                    m_Image.gameObject.SetActive(false);
                }
            }
        }

        private void OnDisable()
        {
            m_Image.texture = null;
        }
    }
}