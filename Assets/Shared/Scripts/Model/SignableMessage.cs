using System;
using System.Collections;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class EIP712TypedData
    {
        public SignableDomain domain;
        public SignableTypes types;
        public SignableValue message;
        public string primaryType;
    }

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
        public List<NameType> OrderComponents;
        public List<NameType> OfferItem;
        public List<SignableTypeConsiderationItem> ConsiderationItem;
        public List<NameType> EIP712Domain;
    }

    [Serializable]
    public class NameType
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