# Immutable.Ts.Model.Order

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AccountAddress** | **string** |  | [optional] 
**Buy** | [**List&lt;V1TsSdkOrderbookPrepareListingPostRequestBuy&gt;**](V1TsSdkOrderbookPrepareListingPostRequestBuy.md) |  | [optional] 
**Chain** | [**OrderChain**](OrderChain.md) |  | [optional] 
**CreatedAt** | **string** |  | [optional] 
**EndAt** | **string** | Time after which the Order is expired | [optional] 
**Fees** | [**List&lt;Fee&gt;**](Fee.md) |  | [optional] 
**FillStatus** | [**OrderFillStatus**](OrderFillStatus.md) |  | [optional] 
**Id** | **string** |  | 
**OrderHash** | **string** |  | 
**ProtocolData** | [**OrderProtocolData**](OrderProtocolData.md) |  | [optional] 
**Salt** | **string** |  | [optional] 
**Sell** | [**List&lt;V1TsSdkOrderbookPrepareListingPostRequestSell&gt;**](V1TsSdkOrderbookPrepareListingPostRequestSell.md) |  | 
**Signature** | **string** |  | 
**StartAt** | **string** | Time after which the Order is considered active | 
**Status** | [**OrderStatus**](OrderStatus.md) |  | 
**Type** | **string** |  | 
**UpdatedAt** | **string** |  | 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

