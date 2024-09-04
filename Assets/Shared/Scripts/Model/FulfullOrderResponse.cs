using System;
using System.Collections;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class FulfullOrderResponse
    {
        public Transaction[] transactionsToSend;
    }
}