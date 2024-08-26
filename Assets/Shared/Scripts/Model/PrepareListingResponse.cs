using System;
using System.Collections;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class PrepareListingResponse
    {
        public TransactionToSend transactionToSend;
        public string toSign;
        public string preparedListing;
    }
}