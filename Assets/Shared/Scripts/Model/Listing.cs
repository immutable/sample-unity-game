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