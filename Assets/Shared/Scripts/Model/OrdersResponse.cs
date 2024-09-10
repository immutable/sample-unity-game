using System;
using System.Collections;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class OrderResponse
    {
        public OrderModel result;
    }

    [Serializable]
    public class OrderModel
    {
        public string id;
        public string account_address;
        public Buy[] buy;
        public Sell[] sell;
        public Fee[] fees;
        public AssetModel asset; // TODO remove
        public ListingStatus status;
    }

    [Serializable]
    public class Buy
    {
        public string amount;
    }

    [Serializable]
    public class Sell
    {
        public string contract_address;
        public string token_id;
    }

    [Serializable]
    public class Fee
    {
        public string amount;
        public string recipient_address;
    }

}