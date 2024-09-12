# Immutable.Ts.Model.V1TsSdkV1OrderbookCancelOrdersPost200ResponseResult

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**FailedCancellations** | [**List&lt;V1TsSdkV1OrderbookCancelOrdersPost200ResponseResultFailedCancellationsInner&gt;**](V1TsSdkV1OrderbookCancelOrdersPost200ResponseResultFailedCancellationsInner.md) | Orders which failed to be cancelled | [optional] 
**PendingCancellations** | **List&lt;string&gt;** | Orders which are marked for cancellation but the cancellation cannot be guaranteed | [optional] 
**SuccessfulCancellations** | **List&lt;string&gt;** | Orders which were successfully cancelled | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
