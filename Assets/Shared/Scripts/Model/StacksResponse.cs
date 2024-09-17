using System;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class StacksResponse
    {
        public List<StacksResult> result;
        public PageModel page;
    }

    [Serializable]
    public class StacksResult
    {
        public Stack stack;
        public Market market;
        public List<StackListing> listings;
        public List<StackListing> notListed; // Added myself, this will actually be another API call
    }

    [Serializable]
    public class Stack
    {
        public string stack_id;
        public string chain;
        public string contract_address;
        public string name;
        public string description;
        public string image;
        public string external_url;
        public string animation_url;
        public string youtube_url;
        public List<AssetAttribute> attributes;
        public int total_count;
        public DateTime created_at;
        public DateTime updated_at;
    }

    [Serializable]
    public class AssetAttribute
    {
        public string display_type;
        public string trait_type;
        public string value;
    }

    [Serializable]
    public class Market
    {
        public FloorListing floor_listing;
        public TopBid top_bid;
        public LastTrade last_trade;
    }

    [Serializable]
    public class FloorListing
    {
        public string listing_id;
        public Price price;
        public int quantity;
        public DateTime created_at;
    }

    [Serializable]
    public class TopBid
    {
        public string bid_id;
        public Price price;
        public int quantity;
        public DateTime created_at;
    }

    [Serializable]
    public class LastTrade
    {
        public string trade_id;
        public Price price;
        public int quantity;
        public DateTime created_at;
    }

    [Serializable]
    public class Price
    {
        public Token token;
        public Amount amount;
    }

    [Serializable]
    public class Token
    {
        public string type;
        public string symbol;
    }

    [Serializable]
    public class Amount
    {
        public string value;
        public string value_in_eth;
    }

    [Serializable]
    public class StackListing
    {
        public string listing_id;
        public Price price;
        public string token_id;
        public int quantity;
        public string account_address;
        public Fee[] fees;
    }
}