using System;
using System.Collections;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
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