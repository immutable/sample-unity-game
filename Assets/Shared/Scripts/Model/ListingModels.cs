using System;
using System.Collections;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    [Serializable]
    public class PrepareListingERC20Item
    {
        public string amount;
        public string contractAddress;
        public string type = "ERC20";
    }

    [Serializable]
    public class PrepareListingERC721Item
    {
        public string contractAddress;
        public string tokenId;
        public string type = "ERC721";
    }

    [Serializable]
    public class PrepareListingRequest
    {
        public PrepareListingERC20Item buy;
        public string makerAddress;
        public PrepareListingERC721Item sell;
    }

    [Serializable]
    public class CreateListingRequest
    {
        public List<CreateListingFeeValue> makerFees;
        public CreateListingOrderComponents orderComponents;
        public string orderHash;
        public string orderSignature;
    }

    [Serializable]
    public class PrepareListingResponse
    {
        public List<ListingAction> actions;
        public CreateListingOrderComponents orderComponents;
        public string orderHash;
    }

    [Serializable]
    public class ListingAction
    {
        public string type;
        public string purpose;
        public Transaction transaction;
        public SignableMessage message;
    }

    [Serializable]
    public class CreateListingFeeValue
    {
        public string amount;
        public string recipientAddress;
    }

    [Serializable]
    public class CreateListingOrderComponents
    {
        public string conduitKey;
        public List<CreateListingConsideration> consideration;
        public string endTime;
        public List<CreateListingOffer> offer;
        public string offerer;
        public string orderType;
        public string salt;
        public string startTime;
        public string totalOriginalConsiderationItems;
        public string zone;
        public string zoneHash;
        public string counter;
    }

    [Serializable]
    public class CreateListingConsideration
    {
        public string endAmount;
        public string identifierOrCriteria;
        public string itemType;
        public string recipient;
        public string startAmount;
        public string token;
    }

    [Serializable]
    public class CreateListingOffer
    {
        public string endAmount;
        public string identifierOrCriteria;
        public string itemType;
        public string startAmount;
        public string token;
    }
}