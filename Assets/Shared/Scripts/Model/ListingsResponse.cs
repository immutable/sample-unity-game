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
    public class OldListing
    {
        public string id;
        public ListingStatus status;
    }

    [Serializable]
    public class ListingStatus
    {
        public string name;
    }
}