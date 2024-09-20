namespace HyperCasual.Runner
{
    public static class Config
    {
        public const string CLIENT_ID = "";

        // Redirect URLs
#if (UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX
        public const string REDIRECT_URI = "imxsample://callback";
        public const string LOGOUT_URI = "imxsample://callback/logout";
#else
        public const string REDIRECT_URI = null;
        public const string LOGOUT_URI = null;
#endif

        // Immutable zkEVM
        public const string ZK_TOKEN_TOKEN_ADDRESS = "";
        public const string ZK_SKIN_TOKEN_ADDRESS = "";

        // Immutable X
        public const string TOKEN_TOKEN_ADDRESS = "";
        public const string SKIN_TOKEN_ADDRESS = "";


        public const string SERVER_BASE_URL = "http://localhost:6060";
    }
}