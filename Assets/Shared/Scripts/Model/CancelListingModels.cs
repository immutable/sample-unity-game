using System;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class CancelListingRequest
    {
        public string accountAddress;
        public List<string> orderIds;
    }

    [Serializable]
    public class CancelListingResponse
    {
        public CancellationAction cancellationAction;
    }

    [Serializable]
    public class CancellationAction
    {
        public Transaction populatedTransaction;
    }
}