using System;
using System.Collections;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class ListTokenResponse
    {
        public List<TokenModel> result;
    }

    [Serializable]
    public class TokenResponse
    {
        public TokenModel result;
    }

    [Serializable]
    public class TokenModel
    {
        public string token_id;
        public string image;
        public string name;
        public string contract_address;
    }

    [Serializable]
    public class ListOrderResponse
    {
        public List<OrderModel> result;
    }

    [Serializable]
    public class OrderModel
    {
        public string id;
        public string account_address;
        public Buy[] buy;
        public Sell[] sell;
        public Fee[] fees;
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

    [Serializable]
    public class PrepareListingResponse
    {
        public TransactionToSend transactionToSend;
        public string toSign;
        public string preparedListing;
    }

    [Serializable]
    public class FulfullOrderResponse
    {
        public TransactionToSend[] transactionsToSend;
    }

    [Serializable]
    public class TransactionToSend
    {
        public string to;
        public string data;
    }

    [Serializable]
    public class CreateListingResponse
    {
        public Listing result;
    }

    [Serializable]
    public class ListingResponse
    {
        public Listing[] result;
    }

    [Serializable]
    public class Listing
    {
        public string id;
    }
}