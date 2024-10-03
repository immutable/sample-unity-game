using System;

namespace HyperCasual.Runner
{
    [Serializable]
    public class Transaction
    {
        public string to;
        public string data;
        public string amount;
    }
}