using System;
using System.Collections;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class FulfullOrderRequest
    {
        public string listingId;
        public string takerAddress;
        public FulfullOrderRequestFee[] takerFees;
    }

    [Serializable]
    public class FulfullOrderRequestFee
    {
        public string amount;
        public string recipientAddress;
    }

    [Serializable]
    public class FulfullOrderResponse
    {
        public Transaction[] transactions;
    }
}