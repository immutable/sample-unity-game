# Immutable.Search.Api.SearchApi

All URIs are relative to *https://api.immutable.com*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**Backfill**](SearchApi.md#backfill) | **POST** /v1/internal/chains/{chain_name}/backfill | Backfills or re-indexes a given entity |
| [**QuotesForNFTs**](SearchApi.md#quotesfornfts) | **GET** /experimental/chains/{chain_name}/quotes/{contract_address}/nfts | Get pricing data for a list of token ids |
| [**QuotesForStacks**](SearchApi.md#quotesforstacks) | **GET** /experimental/chains/{chain_name}/quotes/{contract_address}/stacks | Get pricing data for a list of stack ids |
| [**SearchStacks**](SearchApi.md#searchstacks) | **GET** /experimental/chains/{chain_name}/search/stacks | Search NFT stacks |

<a id="backfill"></a>
# **Backfill**
> void Backfill (string chainName, BackfillRequest backfillRequest)

Backfills or re-indexes a given entity

Backfills or re-indexes a given entity

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Search.Api;
using Immutable.Search.Client;
using Immutable.Search.Model;

namespace Example
{
    public class BackfillExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new SearchApi(config);
            var chainName = imtbl-zkevm-testnet;  // string | The name of chain
            var backfillRequest = new BackfillRequest(); // BackfillRequest | 

            try
            {
                // Backfills or re-indexes a given entity
                apiInstance.Backfill(chainName, backfillRequest);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SearchApi.Backfill: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the BackfillWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Backfills or re-indexes a given entity
    apiInstance.BackfillWithHttpInfo(chainName, backfillRequest);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SearchApi.BackfillWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **chainName** | **string** | The name of chain |  |
| **backfillRequest** | [**BackfillRequest**](BackfillRequest.md) |  |  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **202** | Accepted |  -  |
| **400** | Bad Request (400) |  -  |
| **404** | The specified resource was not found (404) |  -  |
| **500** | Internal Server Error (500) |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="quotesfornfts"></a>
# **QuotesForNFTs**
> QuotesForNFTsResult QuotesForNFTs (string chainName, string contractAddress, List<string> tokenId, string? pageCursor = null)

Get pricing data for a list of token ids

Get pricing data for a list of token ids

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Search.Api;
using Immutable.Search.Client;
using Immutable.Search.Model;

namespace Example
{
    public class QuotesForNFTsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new SearchApi(config);
            var chainName = imtbl-zkevm-testnet;  // string | The name of chain
            var contractAddress = "contractAddress_example";  // string | Contract address for collection that these token ids are on
            var tokenId = new List<string>(); // List<string> | List of token ids to get pricing data for
            var pageCursor = "pageCursor_example";  // string? | Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional) 

            try
            {
                // Get pricing data for a list of token ids
                QuotesForNFTsResult result = apiInstance.QuotesForNFTs(chainName, contractAddress, tokenId, pageCursor);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SearchApi.QuotesForNFTs: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the QuotesForNFTsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get pricing data for a list of token ids
    ApiResponse<QuotesForNFTsResult> response = apiInstance.QuotesForNFTsWithHttpInfo(chainName, contractAddress, tokenId, pageCursor);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SearchApi.QuotesForNFTsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **chainName** | **string** | The name of chain |  |
| **contractAddress** | **string** | Contract address for collection that these token ids are on |  |
| **tokenId** | [**List&lt;string&gt;**](string.md) | List of token ids to get pricing data for |  |
| **pageCursor** | **string?** | Encoded page cursor to retrieve previous or next page. Use the value returned in the response. | [optional]  |

### Return type

[**QuotesForNFTsResult**](QuotesForNFTsResult.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | 200 response |  -  |
| **400** | Bad Request (400) |  -  |
| **401** | Unauthorised Request (401) |  -  |
| **403** | Forbidden Request (403) |  -  |
| **404** | The specified resource was not found (404) |  -  |
| **500** | Internal Server Error (500) |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="quotesforstacks"></a>
# **QuotesForStacks**
> QuotesForStacksResult QuotesForStacks (string chainName, string contractAddress, List<string> stackId, string? pageCursor = null)

Get pricing data for a list of stack ids

Get pricing data for a list of stack ids

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Search.Api;
using Immutable.Search.Client;
using Immutable.Search.Model;

namespace Example
{
    public class QuotesForStacksExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new SearchApi(config);
            var chainName = imtbl-zkevm-testnet;  // string | The name of chain
            var contractAddress = "contractAddress_example";  // string | Contract address for collection that these stacks are on
            var stackId = new List<string>(); // List<string> | List of stack ids to get pricing data for
            var pageCursor = "pageCursor_example";  // string? | Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional) 

            try
            {
                // Get pricing data for a list of stack ids
                QuotesForStacksResult result = apiInstance.QuotesForStacks(chainName, contractAddress, stackId, pageCursor);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SearchApi.QuotesForStacks: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the QuotesForStacksWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get pricing data for a list of stack ids
    ApiResponse<QuotesForStacksResult> response = apiInstance.QuotesForStacksWithHttpInfo(chainName, contractAddress, stackId, pageCursor);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SearchApi.QuotesForStacksWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **chainName** | **string** | The name of chain |  |
| **contractAddress** | **string** | Contract address for collection that these stacks are on |  |
| **stackId** | [**List&lt;string&gt;**](string.md) | List of stack ids to get pricing data for |  |
| **pageCursor** | **string?** | Encoded page cursor to retrieve previous or next page. Use the value returned in the response. | [optional]  |

### Return type

[**QuotesForStacksResult**](QuotesForStacksResult.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | 200 response |  -  |
| **400** | Bad Request (400) |  -  |
| **401** | Unauthorised Request (401) |  -  |
| **403** | Forbidden Request (403) |  -  |
| **404** | The specified resource was not found (404) |  -  |
| **500** | Internal Server Error (500) |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="searchstacks"></a>
# **SearchStacks**
> SearchStacksResult SearchStacks (string chainName, List<string> contractAddress, string? accountAddress = null, bool? onlyIncludeOwnerListings = null, List<AttributeQuery>? trait = null, string? keyword = null, int? pageSize = null, string? pageCursor = null)

Search NFT stacks

Search NFT stacks

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Immutable.Search.Api;
using Immutable.Search.Client;
using Immutable.Search.Model;

namespace Example
{
    public class SearchStacksExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.immutable.com";
            var apiInstance = new SearchApi(config);
            var chainName = imtbl-zkevm-testnet;  // string | The name of chain
            var contractAddress = new List<string>(); // List<string> | List of contract addresses to filter by
            var accountAddress = 1334120697966828340666039427861105342297873844179;  // string? | Account address to filter by (optional) 
            var onlyIncludeOwnerListings = true;  // bool? | Whether to the listings should include only the owner created listings (optional) 
            var trait = new List<AttributeQuery>?(); // List<AttributeQuery>? | Traits to filter by (optional) 
            var keyword = sword;  // string? | Keyword to search NFT name and description. Alphanumeric characters only. (optional) 
            var pageSize = 100;  // int? | Number of results to return per page (optional)  (default to 100)
            var pageCursor = "pageCursor_example";  // string? | Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional) 

            try
            {
                // Search NFT stacks
                SearchStacksResult result = apiInstance.SearchStacks(chainName, contractAddress, accountAddress, onlyIncludeOwnerListings, trait, keyword, pageSize, pageCursor);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SearchApi.SearchStacks: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SearchStacksWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Search NFT stacks
    ApiResponse<SearchStacksResult> response = apiInstance.SearchStacksWithHttpInfo(chainName, contractAddress, accountAddress, onlyIncludeOwnerListings, trait, keyword, pageSize, pageCursor);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SearchApi.SearchStacksWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **chainName** | **string** | The name of chain |  |
| **contractAddress** | [**List&lt;string&gt;**](string.md) | List of contract addresses to filter by |  |
| **accountAddress** | **string?** | Account address to filter by | [optional]  |
| **onlyIncludeOwnerListings** | **bool?** | Whether to the listings should include only the owner created listings | [optional]  |
| **trait** | [**List&lt;AttributeQuery&gt;?**](AttributeQuery.md) | Traits to filter by | [optional]  |
| **keyword** | **string?** | Keyword to search NFT name and description. Alphanumeric characters only. | [optional]  |
| **pageSize** | **int?** | Number of results to return per page | [optional] [default to 100] |
| **pageCursor** | **string?** | Encoded page cursor to retrieve previous or next page. Use the value returned in the response. | [optional]  |

### Return type

[**SearchStacksResult**](SearchStacksResult.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | 200 response |  -  |
| **400** | Bad Request (400) |  -  |
| **401** | Unauthorised Request (401) |  -  |
| **403** | Forbidden Request (403) |  -  |
| **404** | The specified resource was not found (404) |  -  |
| **500** | Internal Server Error (500) |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

