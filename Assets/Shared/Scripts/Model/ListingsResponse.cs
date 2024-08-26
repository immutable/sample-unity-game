using System;
using System.Collections;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class ListingsResponse
    {
        public Listing[] result;
    }

    [Serializable]
    public class ListingResponse
    {
        public Listing result;
    }

    [Serializable]
    public class Listing
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