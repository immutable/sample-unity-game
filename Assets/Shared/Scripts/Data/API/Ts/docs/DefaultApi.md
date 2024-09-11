# Immutable.Ts.Api.DefaultApi

All URIs are relative to *https://api.immutable.com*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**V1HeartbeatGet**](DefaultApi.md#v1heartbeatget) | **GET** /v1/heartbeat |  |
| [**V1HeartbeatHead**](DefaultApi.md#v1heartbeathead) | **HEAD** /v1/heartbeat |  |
| [**V1TsSdkV1OrderbookCancelOrdersOnChainPost**](DefaultApi.md#v1tssdkv1orderbookcancelordersonchainpost) | **POST** /v1/ts-sdk/v1/orderbook/cancelOrdersOnChain |  |
| [**V1TsSdkV1OrderbookCancelOrdersPost**](DefaultApi.md#v1tssdkv1orderbookcancelorderspost) | **POST** /v1/ts-sdk/v1/orderbook/cancelOrders |  |
| [**V1TsSdkV1OrderbookCreateListingPost**](DefaultApi.md#v1tssdkv1orderbookcreatelistingpost) | **POST** /v1/ts-sdk/v1/orderbook/createListing |  |
| [**V1TsSdkV1OrderbookFulfillOrderPost**](DefaultApi.md#v1tssdkv1orderbookfulfillorderpost) | **POST** /v1/ts-sdk/v1/orderbook/fulfillOrder |  |
| [**V1TsSdkV1OrderbookPrepareListingPost**](DefaultApi.md#v1tssdkv1orderbookpreparelistingpost) | **POST** /v1/ts-sdk/v1/orderbook/prepareListing |  |
| [**V1TsSdkV1OrderbookPrepareOrderCancellationsPost**](DefaultApi.md#v1tssdkv1orderbookprepareordercancellationspost) | **POST** /v1/ts-sdk/v1/orderbook/prepareOrderCancellations |  |

<a id="v1heartbeatget"></a>
# **V1HeartbeatGet**
> void V1HeartbeatGet ()



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
                apiInstance.V1HeartbeatGet();
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
    apiInstance.V1HeartbeatGetWithHttpInfo();
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

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="v1heartbeathead"></a>
# **V1HeartbeatHead**
> void V1HeartbeatHead ()



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
                apiInstance.V1HeartbeatHead();
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
    apiInstance.V1HeartbeatHeadWithHttpInfo();
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

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="v1tssdkv1orderbookcancelordersonchainpost"></a>
# **V1TsSdkV1OrderbookCancelOrdersOnChainPost**
> V1TsSdkV1OrderbookCancelOrdersOnChainPost200Response V1TsSdkV1OrderbookCancelOrdersOnChainPost (V1TsSdkV1OrderbookCancelOrdersOnChainPostRequest? v1TsSdkV1OrderbookCancelOrdersOnChainPostRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;

namespace Example
{
    public class V1TsSdkV1OrderbookCancelOrdersOnChainPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new DefaultApi(config);
            var v1TsSdkV1OrderbookCancelOrdersOnChainPostRequest = new V1TsSdkV1OrderbookCancelOrdersOnChainPostRequest?(); // V1TsSdkV1OrderbookCancelOrdersOnChainPostRequest? |  (optional) 

            try
            {
                V1TsSdkV1OrderbookCancelOrdersOnChainPost200Response result = apiInstance.V1TsSdkV1OrderbookCancelOrdersOnChainPost(v1TsSdkV1OrderbookCancelOrdersOnChainPostRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.V1TsSdkV1OrderbookCancelOrdersOnChainPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the V1TsSdkV1OrderbookCancelOrdersOnChainPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<V1TsSdkV1OrderbookCancelOrdersOnChainPost200Response> response = apiInstance.V1TsSdkV1OrderbookCancelOrdersOnChainPostWithHttpInfo(v1TsSdkV1OrderbookCancelOrdersOnChainPostRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.V1TsSdkV1OrderbookCancelOrdersOnChainPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **v1TsSdkV1OrderbookCancelOrdersOnChainPostRequest** | [**V1TsSdkV1OrderbookCancelOrdersOnChainPostRequest?**](V1TsSdkV1OrderbookCancelOrdersOnChainPostRequest?.md) |  | [optional]  |

### Return type

[**V1TsSdkV1OrderbookCancelOrdersOnChainPost200Response**](V1TsSdkV1OrderbookCancelOrdersOnChainPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="v1tssdkv1orderbookcancelorderspost"></a>
# **V1TsSdkV1OrderbookCancelOrdersPost**
> V1TsSdkV1OrderbookCancelOrdersPost200Response V1TsSdkV1OrderbookCancelOrdersPost (V1TsSdkV1OrderbookCancelOrdersPostRequest? v1TsSdkV1OrderbookCancelOrdersPostRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;

namespace Example
{
    public class V1TsSdkV1OrderbookCancelOrdersPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new DefaultApi(config);
            var v1TsSdkV1OrderbookCancelOrdersPostRequest = new V1TsSdkV1OrderbookCancelOrdersPostRequest?(); // V1TsSdkV1OrderbookCancelOrdersPostRequest? |  (optional) 

            try
            {
                V1TsSdkV1OrderbookCancelOrdersPost200Response result = apiInstance.V1TsSdkV1OrderbookCancelOrdersPost(v1TsSdkV1OrderbookCancelOrdersPostRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.V1TsSdkV1OrderbookCancelOrdersPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the V1TsSdkV1OrderbookCancelOrdersPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<V1TsSdkV1OrderbookCancelOrdersPost200Response> response = apiInstance.V1TsSdkV1OrderbookCancelOrdersPostWithHttpInfo(v1TsSdkV1OrderbookCancelOrdersPostRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.V1TsSdkV1OrderbookCancelOrdersPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **v1TsSdkV1OrderbookCancelOrdersPostRequest** | [**V1TsSdkV1OrderbookCancelOrdersPostRequest?**](V1TsSdkV1OrderbookCancelOrdersPostRequest?.md) |  | [optional]  |

### Return type

[**V1TsSdkV1OrderbookCancelOrdersPost200Response**](V1TsSdkV1OrderbookCancelOrdersPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="v1tssdkv1orderbookcreatelistingpost"></a>
# **V1TsSdkV1OrderbookCreateListingPost**
> V1TsSdkV1OrderbookCreateListingPost200Response V1TsSdkV1OrderbookCreateListingPost (V1TsSdkV1OrderbookCreateListingPostRequest? v1TsSdkV1OrderbookCreateListingPostRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;

namespace Example
{
    public class V1TsSdkV1OrderbookCreateListingPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new DefaultApi(config);
            var v1TsSdkV1OrderbookCreateListingPostRequest = new V1TsSdkV1OrderbookCreateListingPostRequest?(); // V1TsSdkV1OrderbookCreateListingPostRequest? |  (optional) 

            try
            {
                V1TsSdkV1OrderbookCreateListingPost200Response result = apiInstance.V1TsSdkV1OrderbookCreateListingPost(v1TsSdkV1OrderbookCreateListingPostRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.V1TsSdkV1OrderbookCreateListingPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the V1TsSdkV1OrderbookCreateListingPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<V1TsSdkV1OrderbookCreateListingPost200Response> response = apiInstance.V1TsSdkV1OrderbookCreateListingPostWithHttpInfo(v1TsSdkV1OrderbookCreateListingPostRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.V1TsSdkV1OrderbookCreateListingPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **v1TsSdkV1OrderbookCreateListingPostRequest** | [**V1TsSdkV1OrderbookCreateListingPostRequest?**](V1TsSdkV1OrderbookCreateListingPostRequest?.md) |  | [optional]  |

### Return type

[**V1TsSdkV1OrderbookCreateListingPost200Response**](V1TsSdkV1OrderbookCreateListingPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="v1tssdkv1orderbookfulfillorderpost"></a>
# **V1TsSdkV1OrderbookFulfillOrderPost**
> V1TsSdkV1OrderbookFulfillOrderPost200Response V1TsSdkV1OrderbookFulfillOrderPost (V1TsSdkV1OrderbookFulfillOrderPostRequest? v1TsSdkV1OrderbookFulfillOrderPostRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;

namespace Example
{
    public class V1TsSdkV1OrderbookFulfillOrderPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new DefaultApi(config);
            var v1TsSdkV1OrderbookFulfillOrderPostRequest = new V1TsSdkV1OrderbookFulfillOrderPostRequest?(); // V1TsSdkV1OrderbookFulfillOrderPostRequest? |  (optional) 

            try
            {
                V1TsSdkV1OrderbookFulfillOrderPost200Response result = apiInstance.V1TsSdkV1OrderbookFulfillOrderPost(v1TsSdkV1OrderbookFulfillOrderPostRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.V1TsSdkV1OrderbookFulfillOrderPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the V1TsSdkV1OrderbookFulfillOrderPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<V1TsSdkV1OrderbookFulfillOrderPost200Response> response = apiInstance.V1TsSdkV1OrderbookFulfillOrderPostWithHttpInfo(v1TsSdkV1OrderbookFulfillOrderPostRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.V1TsSdkV1OrderbookFulfillOrderPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **v1TsSdkV1OrderbookFulfillOrderPostRequest** | [**V1TsSdkV1OrderbookFulfillOrderPostRequest?**](V1TsSdkV1OrderbookFulfillOrderPostRequest?.md) |  | [optional]  |

### Return type

[**V1TsSdkV1OrderbookFulfillOrderPost200Response**](V1TsSdkV1OrderbookFulfillOrderPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="v1tssdkv1orderbookpreparelistingpost"></a>
# **V1TsSdkV1OrderbookPrepareListingPost**
> V1TsSdkV1OrderbookPrepareListingPost200Response V1TsSdkV1OrderbookPrepareListingPost (V1TsSdkV1OrderbookPrepareListingPostRequest? v1TsSdkV1OrderbookPrepareListingPostRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;

namespace Example
{
    public class V1TsSdkV1OrderbookPrepareListingPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new DefaultApi(config);
            var v1TsSdkV1OrderbookPrepareListingPostRequest = new V1TsSdkV1OrderbookPrepareListingPostRequest?(); // V1TsSdkV1OrderbookPrepareListingPostRequest? |  (optional) 

            try
            {
                V1TsSdkV1OrderbookPrepareListingPost200Response result = apiInstance.V1TsSdkV1OrderbookPrepareListingPost(v1TsSdkV1OrderbookPrepareListingPostRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.V1TsSdkV1OrderbookPrepareListingPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the V1TsSdkV1OrderbookPrepareListingPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<V1TsSdkV1OrderbookPrepareListingPost200Response> response = apiInstance.V1TsSdkV1OrderbookPrepareListingPostWithHttpInfo(v1TsSdkV1OrderbookPrepareListingPostRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.V1TsSdkV1OrderbookPrepareListingPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **v1TsSdkV1OrderbookPrepareListingPostRequest** | [**V1TsSdkV1OrderbookPrepareListingPostRequest?**](V1TsSdkV1OrderbookPrepareListingPostRequest?.md) |  | [optional]  |

### Return type

[**V1TsSdkV1OrderbookPrepareListingPost200Response**](V1TsSdkV1OrderbookPrepareListingPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="v1tssdkv1orderbookprepareordercancellationspost"></a>
# **V1TsSdkV1OrderbookPrepareOrderCancellationsPost**
> V1TsSdkV1OrderbookPrepareOrderCancellationsPost200Response V1TsSdkV1OrderbookPrepareOrderCancellationsPost (V1TsSdkV1OrderbookPrepareOrderCancellationsPostRequest? v1TsSdkV1OrderbookPrepareOrderCancellationsPostRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;

namespace Example
{
    public class V1TsSdkV1OrderbookPrepareOrderCancellationsPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new DefaultApi(config);
            var v1TsSdkV1OrderbookPrepareOrderCancellationsPostRequest = new V1TsSdkV1OrderbookPrepareOrderCancellationsPostRequest?(); // V1TsSdkV1OrderbookPrepareOrderCancellationsPostRequest? |  (optional) 

            try
            {
                V1TsSdkV1OrderbookPrepareOrderCancellationsPost200Response result = apiInstance.V1TsSdkV1OrderbookPrepareOrderCancellationsPost(v1TsSdkV1OrderbookPrepareOrderCancellationsPostRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.V1TsSdkV1OrderbookPrepareOrderCancellationsPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the V1TsSdkV1OrderbookPrepareOrderCancellationsPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<V1TsSdkV1OrderbookPrepareOrderCancellationsPost200Response> response = apiInstance.V1TsSdkV1OrderbookPrepareOrderCancellationsPostWithHttpInfo(v1TsSdkV1OrderbookPrepareOrderCancellationsPostRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.V1TsSdkV1OrderbookPrepareOrderCancellationsPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **v1TsSdkV1OrderbookPrepareOrderCancellationsPostRequest** | [**V1TsSdkV1OrderbookPrepareOrderCancellationsPostRequest?**](V1TsSdkV1OrderbookPrepareOrderCancellationsPostRequest?.md) |  | [optional]  |

### Return type

[**V1TsSdkV1OrderbookPrepareOrderCancellationsPost200Response**](V1TsSdkV1OrderbookPrepareOrderCancellationsPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

