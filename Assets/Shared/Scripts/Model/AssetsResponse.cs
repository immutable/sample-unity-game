using System;
using System.Collections;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class AssetsResponse
    {
        public List<AssetModel> result;
        public PageModel page;
    }

    [Serializable]
    public class AssetModel
    {
        public string token_id;
        public string image;
        public string name;
        public string contract_address;
    }
}