using System;
using UnityEngine;

namespace HyperCasual.Runner
{
    public static class Config
    {
        public const string SERVER_URL = "https://sample-passport-unity-game-api.dev.immutable.com/fox";
        // public const string SERVER_URL = "http://localhost:6060";

        public const string CLIENT_ID = "mp6rxfMDwwZDogcdgNrAaHnG0qMlXuMK"; // Testnet
                                                                            // public const string CLIENT_ID = "UnB98ngnXIZIEJWGJOjVe1BpCx5ix7qc"; // WebGL

        public const string ENVIRONMENT = "sandbox";

#if (UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX
        public const string REDIRECT_URI = "immutablerunner://callback";
        public const string LOGOUT_REIDIRECT_URI = "immutablerunner://logout";
#elif UNITY_WEBGL && !UNITY_EDITOR
        private static readonly string url = Application.absoluteURL;
        private static readonly Uri uri = new Uri(url);
        private static readonly string scheme = uri.Scheme;
        private static readonly string hostWithPort = uri.IsDefaultPort ? uri.Host : $"{uri.Host}:{uri.Port}";
        private static readonly string fullPath =
 uri.AbsolutePath.EndsWith("/") ? uri.AbsolutePath : uri.AbsolutePath.Substring(0, uri.AbsolutePath.LastIndexOf('/') + 1);

        public static readonly string REDIRECT_URI = $"{scheme}://{hostWithPort}{fullPath}callback.html";
        public static readonly string LOGOUT_REIDIRECT_URI = $"{scheme}://{hostWithPort}{fullPath}logout.html";
#else
        public const string REDIRECT_URI = null;
        public const string LOGOUT_REIDIRECT_URI = null;
#endif

        public const string CHAIN_NAME = "imtbl-zkevm-testnet";
        public const string BASE_URL = "https://api.sandbox.immutable.com";
        public const string RPC_URL = "https://rpc.testnet.immutable.com";
        public const int PAGE_SIZE = 20;
    }

    public static class Contract
    {
        public const string SKIN = "0xc8df1b1693e2beffd2e484a825a357c6a3d998f2"; // Testnet team
        public const string TOKEN = "0xb237501b35dfdcad274299236a141425469ab9ba"; // Testnet team
        public const string PACK = "0x8525b5e782f3fbe6460057460be020146b63ed0f"; // Testnet team

        public const string USDC = "0x3B2d8A1931736Fc321C24864BceEe981B11c3c57"; // Transak Testnet USDC
    }
}