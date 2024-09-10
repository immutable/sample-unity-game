namespace HyperCasual.Runner
{
    public static class Config
    {
        public const string CLIENT_ID = "ZJL7JvetcDFBNDlgRs5oJoxuAUUl6uQj";
        // public const string CLIENT_ID = "2Ng38UmEg0Morz1xOQLtsDs72Wx8uyGL"; // Devnet

#if (UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX
        public const string REDIRECT_URI = "immutablerunner://callback";
        public const string LOGOUT_REIDIRECT_URI = "immutablerunner://logout";
#else
        public const string REDIRECT_URI = null;
        public const string LOGOUT_REIDIRECT_URI = null;
#endif

        public const string CHAIN_NAME = "imtbl-zkevm-testnet";

        public const string SEARCH_BASE_URL = "http://localhost:6060";//"https://api.dev.immutable.com";
        public const int PAGE_SIZE = 6;
    }

    public static class Contract
    {
        public const string SKIN = "0xad826E89CDe60E4eE248980D35c0F5C1196ad059"; // Testnet
        // public const string SKIN = "0x9c8b8f69a900df9fe800e3f7cb13ca464339888c"; // Devnet 1
        // public const string SKIN = "0xcdbee7935e1b0eaabdee64219182602df0d8d094"; // Devnet
        public const string TOKEN = "0x912cd5f1cd67F1143b7a5796fd9e5063D755DAbe"; // Testnet
        // public const string TOKEN = "0x328766302e7617d0de5901f8da139dca49f3ec75"; // Devnet
    }
}