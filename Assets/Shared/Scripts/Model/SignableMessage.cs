using System;
using System.Collections;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class SignableMessage
    {
        public SignableDomain domain;
        public SignableTypes types;
        public SignableValue value;
    }

    [Serializable]
    public class SignableDomain
    {
        public string name;
        public string version;
        public int chainId;
        public string verifyingContract;
    }

    [Serializable]
    public class SignableTypes
    {
        public List<SignableTypeOrderComponent> OrderComponents;
        public List<SignableTypeOfferItem> OfferItem;
        public List<SignableTypeConsiderationItem> ConsiderationItem;
    }

    [Serializable]
    public class SignableTypeOrderComponent
    {
        public string name;
        public string type;
    }

    [Serializable]
    public class SignableTypeOfferItem
    {
        public string name;
        public string type;
    }

    [Serializable]
    public class SignableTypeConsiderationItem
    {
        public string name;
        public string type;
    }

    [Serializable]
    public class SignableValue
    {
        public string offerer;
        public string zone;
        public List<SignableOfferItem> offer;
        public List<ConsiderationItem> consideration;
        public string orderType;
        public string startTime;
        public string endTime;
        public string zoneHash;
        public string salt;
        public string conduitKey;
        public string counter;
    }

    [Serializable]
    public class SignableOfferItem
    {
        public string itemType;
        public string token;
        public string identifierOrCriteria;
        public string startAmount;
        public string endAmount;
    }

    [Serializable]
    public class ConsiderationItem
    {
        public string itemType;
        public string token;
        public string identifierOrCriteria;
        public string startAmount;
        public string endAmount;
        public string recipient;
    }
}