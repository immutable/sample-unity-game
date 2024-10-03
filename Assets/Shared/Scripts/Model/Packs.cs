using System;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class Packs
    {
        public List<Pack> result;
    }

    [Serializable]
    public class Pack
    {
        public string name;
        public string description;
        public string image;
        public List<PackItem> items;
        public string collection;
        public string price;
        public string function;
    }

    [Serializable]
    public class PackItem
    {
        public int id;
        public string name;
        public int amount;
        public string image;
    }
}