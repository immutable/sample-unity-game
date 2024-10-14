using System;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class ListingResponse
    {
        public ListingResult result;
    }

    [Serializable]
    public class ListingResult
    {
        public ListingStatus status;
    }

    [Serializable]
    public class ListingStatus
    {
        public string name;
    }
}