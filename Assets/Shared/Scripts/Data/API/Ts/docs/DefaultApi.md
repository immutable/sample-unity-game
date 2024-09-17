# Immutable.Ts.Api.DefaultApi

All URIs are relative to *https://api.immutable.com*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**V1HeartbeatGet**](DefaultApi.md#v1heartbeatget) | **GET** /v1/heartbeat |  |
| [**V1HeartbeatHead**](DefaultApi.md#v1heartbeathead) | **HEAD** /v1/heartbeat |  |
| [**V1TsSdkOrderbookCancelOrdersOnChainPost**](DefaultApi.md#v1tssdkorderbookcancelordersonchainpost) | **POST** /v1/ts-sdk/orderbook/cancelOrdersOnChain |  |
| [**V1TsSdkOrderbookCancelOrdersPost**](DefaultApi.md#v1tssdkorderbookcancelorderspost) | **POST** /v1/ts-sdk/orderbook/cancelOrders |  |
| [**V1TsSdkOrderbookCreateListingPost**](DefaultApi.md#v1tssdkorderbookcreatelistingpost) | **POST** /v1/ts-sdk/orderbook/createListing |  |
| [**V1TsSdkOrderbookFulfillOrderPost**](DefaultApi.md#v1tssdkorderbookfulfillorderpost) | **POST** /v1/ts-sdk/orderbook/fulfillOrder |  |
| [**V1TsSdkOrderbookPrepareListingPost**](DefaultApi.md#v1tssdkorderbookpreparelistingpost) | **POST** /v1/ts-sdk/orderbook/prepareListing |  |
| [**V1TsSdkOrderbookPrepareOrderCancellationsPost**](DefaultApi.md#v1tssdkorderbookprepareordercancellationspost) | **POST** /v1/ts-sdk/orderbook/prepareOrderCancellations |  |

<a id="v1heartbeatget"></a>
# **V1HeartbeatGet**
> V1HeartbeatGet200Response V1HeartbeatGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;

namespace Example
{
    public class V1HeartbeatGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new DefaultApi(config);

            try
            {
                V1HeartbeatGet200Response result = apiInstance.V1HeartbeatGet();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.V1HeartbeatGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the V1HeartbeatGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<V1HeartbeatGet200Response> response = apiInstance.V1HeartbeatGetWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.V1HeartbeatGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**V1HeartbeatGet200Response**](V1HeartbeatGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | heartbeat |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="v1heartbeathead"></a>
# **V1HeartbeatHead**
> V1HeartbeatGet200Response V1HeartbeatHead ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;

namespace Example
{
    public class V1HeartbeatHeadExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new DefaultApi(config);

            try
            {
                V1HeartbeatGet200Response result = apiInstance.V1HeartbeatHead();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.V1HeartbeatHead: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the V1HeartbeatHeadWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<V1HeartbeatGet200Response> response = apiInstance.V1HeartbeatHeadWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.V1HeartbeatHeadWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**V1HeartbeatGet200Response**](V1HeartbeatGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | heartbeat |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="v1tssdkorderbookcancelordersonchainpost"></a>
# **V1TsSdkOrderbookCancelOrdersOnChainPost**
> V1TsSdkOrderbookCancelOrdersOnChainPost200Response V1TsSdkOrderbookCancelOrdersOnChainPost (V1TsSdkOrderbookCancelOrdersOnChainPostRequest? v1TsSdkOrderbookCancelOrdersOnChainPostRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;

namespace Example
{
    public class V1TsSdkOrderbookCancelOrdersOnChainPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new DefaultApi(config);
            var v1TsSdkOrderbookCancelOrdersOnChainPostRequest = new V1TsSdkOrderbookCancelOrdersOnChainPostRequest?(); // V1TsSdkOrderbookCancelOrdersOnChainPostRequest? |  (optional) 

            try
            {
                V1TsSdkOrderbookCancelOrdersOnChainPost200Response result = apiInstance.V1TsSdkOrderbookCancelOrdersOnChainPost(v1TsSdkOrderbookCancelOrdersOnChainPostRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.V1TsSdkOrderbookCancelOrdersOnChainPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the V1TsSdkOrderbookCancelOrdersOnChainPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<V1TsSdkOrderbookCancelOrdersOnChainPost200Response> response = apiInstance.V1TsSdkOrderbookCancelOrdersOnChainPostWithHttpInfo(v1TsSdkOrderbookCancelOrdersOnChainPostRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.V1TsSdkOrderbookCancelOrdersOnChainPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **v1TsSdkOrderbookCancelOrdersOnChainPostRequest** | [**V1TsSdkOrderbookCancelOrdersOnChainPostRequest?**](V1TsSdkOrderbookCancelOrdersOnChainPostRequest?.md) |  | [optional]  |

### Return type

[**V1TsSdkOrderbookCancelOrdersOnChainPost200Response**](V1TsSdkOrderbookCancelOrdersOnChainPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Response schema for the cancelOrder endpoint |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="v1tssdkorderbookcancelorderspost"></a>
# **V1TsSdkOrderbookCancelOrdersPost**
> V1TsSdkOrderbookCancelOrdersPost200Response V1TsSdkOrderbookCancelOrdersPost (V1TsSdkOrderbookCancelOrdersPostRequest? v1TsSdkOrderbookCancelOrdersPostRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;

namespace Example
{
    public class V1TsSdkOrderbookCancelOrdersPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new DefaultApi(config);
            var v1TsSdkOrderbookCancelOrdersPostRequest = new V1TsSdkOrderbookCancelOrdersPostRequest?(); // V1TsSdkOrderbookCancelOrdersPostRequest? |  (optional) 

            try
            {
                V1TsSdkOrderbookCancelOrdersPost200Response result = apiInstance.V1TsSdkOrderbookCancelOrdersPost(v1TsSdkOrderbookCancelOrdersPostRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.V1TsSdkOrderbookCancelOrdersPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the V1TsSdkOrderbookCancelOrdersPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<V1TsSdkOrderbookCancelOrdersPost200Response> response = apiInstance.V1TsSdkOrderbookCancelOrdersPostWithHttpInfo(v1TsSdkOrderbookCancelOrdersPostRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.V1TsSdkOrderbookCancelOrdersPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **v1TsSdkOrderbookCancelOrdersPostRequest** | [**V1TsSdkOrderbookCancelOrdersPostRequest?**](V1TsSdkOrderbookCancelOrdersPostRequest?.md) |  | [optional]  |

### Return type

[**V1TsSdkOrderbookCancelOrdersPost200Response**](V1TsSdkOrderbookCancelOrdersPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Response schema for the cancelOrder endpoint |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="v1tssdkorderbookcreatelistingpost"></a>
# **V1TsSdkOrderbookCreateListingPost**
> V1TsSdkOrderbookCreateListingPost200Response V1TsSdkOrderbookCreateListingPost (V1TsSdkOrderbookCreateListingPostRequest? v1TsSdkOrderbookCreateListingPostRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;

namespace Example
{
    public class V1TsSdkOrderbookCreateListingPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new DefaultApi(config);
            var v1TsSdkOrderbookCreateListingPostRequest = new V1TsSdkOrderbookCreateListingPostRequest?(); // V1TsSdkOrderbookCreateListingPostRequest? |  (optional) 

            try
            {
                V1TsSdkOrderbookCreateListingPost200Response result = apiInstance.V1TsSdkOrderbookCreateListingPost(v1TsSdkOrderbookCreateListingPostRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.V1TsSdkOrderbookCreateListingPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the V1TsSdkOrderbookCreateListingPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<V1TsSdkOrderbookCreateListingPost200Response> response = apiInstance.V1TsSdkOrderbookCreateListingPostWithHttpInfo(v1TsSdkOrderbookCreateListingPostRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.V1TsSdkOrderbookCreateListingPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **v1TsSdkOrderbookCreateListingPostRequest** | [**V1TsSdkOrderbookCreateListingPostRequest?**](V1TsSdkOrderbookCreateListingPostRequest?.md) |  | [optional]  |

### Return type

[**V1TsSdkOrderbookCreateListingPost200Response**](V1TsSdkOrderbookCreateListingPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | The response schema for the create listing endpoint |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="v1tssdkorderbookfulfillorderpost"></a>
# **V1TsSdkOrderbookFulfillOrderPost**
> V1TsSdkOrderbookFulfillOrderPost200Response V1TsSdkOrderbookFulfillOrderPost (V1TsSdkOrderbookFulfillOrderPostRequest? v1TsSdkOrderbookFulfillOrderPostRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;

namespace Example
{
    public class V1TsSdkOrderbookFulfillOrderPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new DefaultApi(config);
            var v1TsSdkOrderbookFulfillOrderPostRequest = new V1TsSdkOrderbookFulfillOrderPostRequest?(); // V1TsSdkOrderbookFulfillOrderPostRequest? |  (optional) 

            try
            {
                V1TsSdkOrderbookFulfillOrderPost200Response result = apiInstance.V1TsSdkOrderbookFulfillOrderPost(v1TsSdkOrderbookFulfillOrderPostRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.V1TsSdkOrderbookFulfillOrderPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the V1TsSdkOrderbookFulfillOrderPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<V1TsSdkOrderbookFulfillOrderPost200Response> response = apiInstance.V1TsSdkOrderbookFulfillOrderPostWithHttpInfo(v1TsSdkOrderbookFulfillOrderPostRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.V1TsSdkOrderbookFulfillOrderPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **v1TsSdkOrderbookFulfillOrderPostRequest** | [**V1TsSdkOrderbookFulfillOrderPostRequest?**](V1TsSdkOrderbookFulfillOrderPostRequest?.md) |  | [optional]  |

### Return type

[**V1TsSdkOrderbookFulfillOrderPost200Response**](V1TsSdkOrderbookFulfillOrderPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Response schema for the fulfillOrder endpoint |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="v1tssdkorderbookpreparelistingpost"></a>
# **V1TsSdkOrderbookPrepareListingPost**
> V1TsSdkOrderbookPrepareListingPost200Response V1TsSdkOrderbookPrepareListingPost (V1TsSdkOrderbookPrepareListingPostRequest? v1TsSdkOrderbookPrepareListingPostRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;

namespace Example
{
    public class V1TsSdkOrderbookPrepareListingPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new DefaultApi(config);
            var v1TsSdkOrderbookPrepareListingPostRequest = new V1TsSdkOrderbookPrepareListingPostRequest?(); // V1TsSdkOrderbookPrepareListingPostRequest? |  (optional) 

            try
            {
                V1TsSdkOrderbookPrepareListingPost200Response result = apiInstance.V1TsSdkOrderbookPrepareListingPost(v1TsSdkOrderbookPrepareListingPostRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.V1TsSdkOrderbookPrepareListingPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the V1TsSdkOrderbookPrepareListingPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<V1TsSdkOrderbookPrepareListingPost200Response> response = apiInstance.V1TsSdkOrderbookPrepareListingPostWithHttpInfo(v1TsSdkOrderbookPrepareListingPostRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.V1TsSdkOrderbookPrepareListingPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **v1TsSdkOrderbookPrepareListingPostRequest** | [**V1TsSdkOrderbookPrepareListingPostRequest?**](V1TsSdkOrderbookPrepareListingPostRequest?.md) |  | [optional]  |

### Return type

[**V1TsSdkOrderbookPrepareListingPost200Response**](V1TsSdkOrderbookPrepareListingPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Response schema for the prepareListing endpoint |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="v1tssdkorderbookprepareordercancellationspost"></a>
# **V1TsSdkOrderbookPrepareOrderCancellationsPost**
> V1TsSdkOrderbookPrepareOrderCancellationsPost200Response V1TsSdkOrderbookPrepareOrderCancellationsPost (V1TsSdkOrderbookPrepareOrderCancellationsPostRequest? v1TsSdkOrderbookPrepareOrderCancellationsPostRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;

namespace Example
{
    public class V1TsSdkOrderbookPrepareOrderCancellationsPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new DefaultApi(config);
            var v1TsSdkOrderbookPrepareOrderCancellationsPostRequest = new V1TsSdkOrderbookPrepareOrderCancellationsPostRequest?(); // V1TsSdkOrderbookPrepareOrderCancellationsPostRequest? |  (optional) 

            try
            {
                V1TsSdkOrderbookPrepareOrderCancellationsPost200Response result = apiInstance.V1TsSdkOrderbookPrepareOrderCancellationsPost(v1TsSdkOrderbookPrepareOrderCancellationsPostRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.V1TsSdkOrderbookPrepareOrderCancellationsPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the V1TsSdkOrderbookPrepareOrderCancellationsPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<V1TsSdkOrderbookPrepareOrderCancellationsPost200Response> response = apiInstance.V1TsSdkOrderbookPrepareOrderCancellationsPostWithHttpInfo(v1TsSdkOrderbookPrepareOrderCancellationsPostRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.V1TsSdkOrderbookPrepareOrderCancellationsPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **v1TsSdkOrderbookPrepareOrderCancellationsPostRequest** | [**V1TsSdkOrderbookPrepareOrderCancellationsPostRequest?**](V1TsSdkOrderbookPrepareOrderCancellationsPostRequest?.md) |  | [optional]  |

### Return type

[**V1TsSdkOrderbookPrepareOrderCancellationsPost200Response**](V1TsSdkOrderbookPrepareOrderCancellationsPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Response schema for the prepareOrderCancellations endpoint |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

