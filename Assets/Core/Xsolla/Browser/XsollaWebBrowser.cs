using UnityEngine;
#if UNITY_WEBGL || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
#endif

namespace Xsolla.Core
{
    public class XsollaWebBrowser : MonoBehaviour
    {
        private static IInAppBrowser _inAppBrowser;

        public static IInAppBrowser InAppBrowser
        {
            get
            {
#if !(UNITY_EDITOR || UNITY_STANDALONE)
                return null;
#else
                if (_inAppBrowser == null)
                {
                    var prefab = Resources.Load<GameObject>(Constants.WEB_BROWSER_RESOURCE_PATH);
                    if (prefab == null)
                    {
                        XDebug.LogError("Prefab InAppBrowser not found in Resources folder.");
                    }
                    else
                    {
                        var go = Instantiate(prefab);
                        go.name = "XsollaWebBrowser";
                        DontDestroyOnLoad(go);
                        _inAppBrowser = go.GetComponent<IInAppBrowser>();
                    }
                }

                return _inAppBrowser;
#endif
            }
        }

        public static void Open(string url, bool forcePlatformBrowser = false)
        {
            XDebug.Log($"WebBrowser. Open url: {url}");
#if UNITY_EDITOR || UNITY_STANDALONE
            if (InAppBrowser != null && !forcePlatformBrowser)
            {
                InAppBrowser.Open(url);
                InAppBrowser.AddInitHandler(() => InAppBrowser.UpdateSize(450, 760));
            }
            else
            {
                Application.OpenURL(url);
            }
#elif UNITY_WEBGL
#pragma warning disable 0618
			Application.ExternalEval($"window.open(\"{url}\",\"_blank\")");
#pragma warning restore 0618
			return;
#else
            Application.OpenURL(url);
#endif
        }

        public static void Close(float delay = 0)
        {
            InAppBrowser?.Close(delay);
        }

#if UNITY_WEBGL
		[DllImport("__Internal")]
		private static extern void OpenPaystationWidget(string token, bool sandbox);

		[DllImport("__Internal")]
		private static extern void ClosePaystationWidget();

		private static Action<bool> WebGLBrowserClosedCallback;

		public static void ClosePaystationWidget(bool isManually)
		{
			WebGLBrowserClosedCallback?.Invoke(isManually);
			ClosePaystationWidget();
		}
#endif
    }
}