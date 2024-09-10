using System;
using UnityEngine;

namespace HyperCasual.Runner
{
    public static class Config
    {
        public const string CLIENT_ID = "UnB98ngnXIZIEJWGJOjVe1BpCx5ix7qc";

#if (UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX
        public const string REDIRECT_URI = "immutablerunner://callback";
        public const string LOGOUT_REIDIRECT_URI = "immutablerunner://logout";
#elif UNITY_WEBGL && !UNITY_EDITOR
        private static readonly string url = Application.absoluteURL;
        private static readonly Uri uri = new Uri(url);
        private static readonly string scheme = uri.Scheme;
        private static readonly string hostWithPort = uri.IsDefaultPort ? uri.Host : $"{uri.Host}:{uri.Port}";
        private static readonly string fullPath = uri.AbsolutePath.EndsWith("/") ? uri.AbsolutePath : uri.AbsolutePath.Substring(0, uri.AbsolutePath.LastIndexOf('/') + 1);

        public static readonly string REDIRECT_URI = $"{scheme}://{hostWithPort}{fullPath}callback.html";
        public static readonly string LOGOUT_REIDIRECT_URI = $"{scheme}://{hostWithPort}{fullPath}logout.html";
#else
        public const string REDIRECT_URI = null;
        public const string LOGOUT_REIDIRECT_URI = null;
#endif
    }

    public static class Contract
    {
        public const string SKIN = "0xe3429b4fc1a648508b2864d77597a48e02cd7255"; // Testnet
        // public const string SKIN = "0x9c8b8f69a900df9fe800e3f7cb13ca464339888c"; // Devnet
        public const string TOKEN = "0x6bFaC7387e317cFcB5801ce8b86C991FaeD912aD"; // Testnet
        // public const string TOKEN = "0x328766302e7617d0de5901f8da139dca49f3ec75"; // Devnet
    }
}