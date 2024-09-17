using System;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class ListingResponse
    {
        public OldListing result;
    }

    [Serializable]
    public class ListingsResponse
    {
        public List<OldListing> result;
    }

    [Serializable]
    public class OldListing
    {
        public string id;
        public ListingStatus status;
        public List<ListingBuy> buy;
    }

    [Serializable]
    public class ListingStatus
    {
        public string name;
    }

    [Serializable]
    public class ListingBuy
    {
        public string amount;
    }
}