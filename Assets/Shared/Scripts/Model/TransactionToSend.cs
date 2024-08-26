using System;
using System.Collections;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class TransactionToSend
    {
        public string to;
        public string data;
    }
}