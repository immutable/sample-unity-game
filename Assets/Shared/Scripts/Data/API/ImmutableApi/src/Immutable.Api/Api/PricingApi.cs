/*
 * Immutable zkEVM API
 *
 * Immutable Multi Rollup API
 *
 * The version of the OpenAPI document: 1.0.0
 * Contact: support@immutable.com
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mime;
using Immutable.Api.Client;
using Immutable.Api.Model;

namespace Immutable.Api.Api
{

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IPricingApiSync : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Experimental: Get pricing data for a list of token ids
        /// </summary>
        /// <remarks>
        /// ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of token ids
        /// </remarks>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these token ids are on</param>
        /// <param name="tokenId">List of token ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <returns>QuotesForNFTsResult</returns>
        QuotesForNFTsResult QuotesForNFTs(string chainName, string contractAddress, List<string> tokenId, string? pageCursor = default(string?));

        /// <summary>
        /// Experimental: Get pricing data for a list of token ids
        /// </summary>
        /// <remarks>
        /// ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of token ids
        /// </remarks>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these token ids are on</param>
        /// <param name="tokenId">List of token ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <returns>ApiResponse of QuotesForNFTsResult</returns>
        ApiResponse<QuotesForNFTsResult> QuotesForNFTsWithHttpInfo(string chainName, string contractAddress, List<string> tokenId, string? pageCursor = default(string?));
        /// <summary>
        /// Experimental: Get pricing data for a list of stack ids
        /// </summary>
        /// <remarks>
        /// ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of stack ids
        /// </remarks>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these stacks are on</param>
        /// <param name="stackId">List of stack ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <returns>QuotesForStacksResult</returns>
        QuotesForStacksResult QuotesForStacks(string chainName, string contractAddress, List<Guid> stackId, string? pageCursor = default(string?));

        /// <summary>
        /// Experimental: Get pricing data for a list of stack ids
        /// </summary>
        /// <remarks>
        /// ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of stack ids
        /// </remarks>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these stacks are on</param>
        /// <param name="stackId">List of stack ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <returns>ApiResponse of QuotesForStacksResult</returns>
        ApiResponse<QuotesForStacksResult> QuotesForStacksWithHttpInfo(string chainName, string contractAddress, List<Guid> stackId, string? pageCursor = default(string?));
        #endregion Synchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IPricingApiAsync : IApiAccessor
    {
        #region Asynchronous Operations
        /// <summary>
        /// Experimental: Get pricing data for a list of token ids
        /// </summary>
        /// <remarks>
        /// ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of token ids
        /// </remarks>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these token ids are on</param>
        /// <param name="tokenId">List of token ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of QuotesForNFTsResult</returns>
        System.Threading.Tasks.Task<QuotesForNFTsResult> QuotesForNFTsAsync(string chainName, string contractAddress, List<string> tokenId, string? pageCursor = default(string?), System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken));

        /// <summary>
        /// Experimental: Get pricing data for a list of token ids
        /// </summary>
        /// <remarks>
        /// ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of token ids
        /// </remarks>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these token ids are on</param>
        /// <param name="tokenId">List of token ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (QuotesForNFTsResult)</returns>
        System.Threading.Tasks.Task<ApiResponse<QuotesForNFTsResult>> QuotesForNFTsWithHttpInfoAsync(string chainName, string contractAddress, List<string> tokenId, string? pageCursor = default(string?), System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken));
        /// <summary>
        /// Experimental: Get pricing data for a list of stack ids
        /// </summary>
        /// <remarks>
        /// ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of stack ids
        /// </remarks>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these stacks are on</param>
        /// <param name="stackId">List of stack ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of QuotesForStacksResult</returns>
        System.Threading.Tasks.Task<QuotesForStacksResult> QuotesForStacksAsync(string chainName, string contractAddress, List<Guid> stackId, string? pageCursor = default(string?), System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken));

        /// <summary>
        /// Experimental: Get pricing data for a list of stack ids
        /// </summary>
        /// <remarks>
        /// ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of stack ids
        /// </remarks>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these stacks are on</param>
        /// <param name="stackId">List of stack ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (QuotesForStacksResult)</returns>
        System.Threading.Tasks.Task<ApiResponse<QuotesForStacksResult>> QuotesForStacksWithHttpInfoAsync(string chainName, string contractAddress, List<Guid> stackId, string? pageCursor = default(string?), System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken));
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IPricingApi : IPricingApiSync, IPricingApiAsync
    {

    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class PricingApi : IDisposable, IPricingApi
    {
        private Immutable.Api.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PricingApi"/> class.
        /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
        /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
        /// </summary>
        /// <returns></returns>
        public PricingApi() : this((string)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PricingApi"/> class.
        /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
        /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
        /// </summary>
        /// <param name="basePath">The target service's base path in URL format.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public PricingApi(string basePath)
        {
            this.Configuration = Immutable.Api.Client.Configuration.MergeConfigurations(
                Immutable.Api.Client.GlobalConfiguration.Instance,
                new Immutable.Api.Client.Configuration { BasePath = basePath }
            );
            this.ApiClient = new Immutable.Api.Client.ApiClient(this.Configuration.BasePath);
            this.Client =  this.ApiClient;
            this.AsynchronousClient = this.ApiClient;
            this.ExceptionFactory = Immutable.Api.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PricingApi"/> class using Configuration object.
        /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
        /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
        /// </summary>
        /// <param name="configuration">An instance of Configuration.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public PricingApi(Immutable.Api.Client.Configuration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Configuration = Immutable.Api.Client.Configuration.MergeConfigurations(
                Immutable.Api.Client.GlobalConfiguration.Instance,
                configuration
            );
            this.ApiClient = new Immutable.Api.Client.ApiClient(this.Configuration.BasePath);
            this.Client = this.ApiClient;
            this.AsynchronousClient = this.ApiClient;
            ExceptionFactory = Immutable.Api.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PricingApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public PricingApi(Immutable.Api.Client.ISynchronousClient client, Immutable.Api.Client.IAsynchronousClient asyncClient, Immutable.Api.Client.IReadableConfiguration configuration)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (asyncClient == null) throw new ArgumentNullException("asyncClient");
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Client = client;
            this.AsynchronousClient = asyncClient;
            this.Configuration = configuration;
            this.ExceptionFactory = Immutable.Api.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Disposes resources if they were created by us
        /// </summary>
        public void Dispose()
        {
            this.ApiClient?.Dispose();
        }

        /// <summary>
        /// Holds the ApiClient if created
        /// </summary>
        public Immutable.Api.Client.ApiClient ApiClient { get; set; } = null;

        /// <summary>
        /// The client for accessing this underlying API asynchronously.
        /// </summary>
        public Immutable.Api.Client.IAsynchronousClient AsynchronousClient { get; set; }

        /// <summary>
        /// The client for accessing this underlying API synchronously.
        /// </summary>
        public Immutable.Api.Client.ISynchronousClient Client { get; set; }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public string GetBasePath()
        {
            return this.Configuration.BasePath;
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public Immutable.Api.Client.IReadableConfiguration Configuration { get; set; }

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public Immutable.Api.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        /// Experimental: Get pricing data for a list of token ids ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of token ids
        /// </summary>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these token ids are on</param>
        /// <param name="tokenId">List of token ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <returns>QuotesForNFTsResult</returns>
        public QuotesForNFTsResult QuotesForNFTs(string chainName, string contractAddress, List<string> tokenId, string? pageCursor = default(string?))
        {
            Immutable.Api.Client.ApiResponse<QuotesForNFTsResult> localVarResponse = QuotesForNFTsWithHttpInfo(chainName, contractAddress, tokenId, pageCursor);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Experimental: Get pricing data for a list of token ids ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of token ids
        /// </summary>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these token ids are on</param>
        /// <param name="tokenId">List of token ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <returns>ApiResponse of QuotesForNFTsResult</returns>
        public Immutable.Api.Client.ApiResponse<QuotesForNFTsResult> QuotesForNFTsWithHttpInfo(string chainName, string contractAddress, List<string> tokenId, string? pageCursor = default(string?))
        {
            // verify the required parameter 'chainName' is set
            if (chainName == null)
                throw new Immutable.Api.Client.ApiException(400, "Missing required parameter 'chainName' when calling PricingApi->QuotesForNFTs");

            // verify the required parameter 'contractAddress' is set
            if (contractAddress == null)
                throw new Immutable.Api.Client.ApiException(400, "Missing required parameter 'contractAddress' when calling PricingApi->QuotesForNFTs");

            // verify the required parameter 'tokenId' is set
            if (tokenId == null)
                throw new Immutable.Api.Client.ApiException(400, "Missing required parameter 'tokenId' when calling PricingApi->QuotesForNFTs");

            Immutable.Api.Client.RequestOptions localVarRequestOptions = new Immutable.Api.Client.RequestOptions();

            string[] _contentTypes = new string[] {
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "application/json"
            };

            var localVarContentType = Immutable.Api.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null) localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = Immutable.Api.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null) localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            localVarRequestOptions.PathParameters.Add("chain_name", Immutable.Api.Client.ClientUtils.ParameterToString(chainName)); // path parameter
            localVarRequestOptions.PathParameters.Add("contract_address", Immutable.Api.Client.ClientUtils.ParameterToString(contractAddress)); // path parameter
            localVarRequestOptions.QueryParameters.Add(Immutable.Api.Client.ClientUtils.ParameterToMultiMap("multi", "token_id", tokenId));
            if (pageCursor != null)
            {
                localVarRequestOptions.QueryParameters.Add(Immutable.Api.Client.ClientUtils.ParameterToMultiMap("", "page_cursor", pageCursor));
            }


            // make the HTTP request
            var localVarResponse = this.Client.Get<QuotesForNFTsResult>("/experimental/chains/{chain_name}/quotes/{contract_address}/nfts", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("QuotesForNFTs", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// Experimental: Get pricing data for a list of token ids ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of token ids
        /// </summary>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these token ids are on</param>
        /// <param name="tokenId">List of token ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of QuotesForNFTsResult</returns>
        public async System.Threading.Tasks.Task<QuotesForNFTsResult> QuotesForNFTsAsync(string chainName, string contractAddress, List<string> tokenId, string? pageCursor = default(string?), System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
        {
            var task = QuotesForNFTsWithHttpInfoAsync(chainName, contractAddress, tokenId, pageCursor, cancellationToken);
#if UNITY_EDITOR || !UNITY_WEBGL
            Immutable.Api.Client.ApiResponse<QuotesForNFTsResult> localVarResponse = await task.ConfigureAwait(false);
#else
            Immutable.Api.Client.ApiResponse<QuotesForNFTsResult> localVarResponse = await task;
#endif
            return localVarResponse.Data;
        }

        /// <summary>
        /// Experimental: Get pricing data for a list of token ids ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of token ids
        /// </summary>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these token ids are on</param>
        /// <param name="tokenId">List of token ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (QuotesForNFTsResult)</returns>
        public async System.Threading.Tasks.Task<Immutable.Api.Client.ApiResponse<QuotesForNFTsResult>> QuotesForNFTsWithHttpInfoAsync(string chainName, string contractAddress, List<string> tokenId, string? pageCursor = default(string?), System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
        {
            // verify the required parameter 'chainName' is set
            if (chainName == null)
                throw new Immutable.Api.Client.ApiException(400, "Missing required parameter 'chainName' when calling PricingApi->QuotesForNFTs");

            // verify the required parameter 'contractAddress' is set
            if (contractAddress == null)
                throw new Immutable.Api.Client.ApiException(400, "Missing required parameter 'contractAddress' when calling PricingApi->QuotesForNFTs");

            // verify the required parameter 'tokenId' is set
            if (tokenId == null)
                throw new Immutable.Api.Client.ApiException(400, "Missing required parameter 'tokenId' when calling PricingApi->QuotesForNFTs");


            Immutable.Api.Client.RequestOptions localVarRequestOptions = new Immutable.Api.Client.RequestOptions();

            string[] _contentTypes = new string[] {
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "application/json"
            };


            var localVarContentType = Immutable.Api.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null) localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = Immutable.Api.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null) localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            localVarRequestOptions.PathParameters.Add("chain_name", Immutable.Api.Client.ClientUtils.ParameterToString(chainName)); // path parameter
            localVarRequestOptions.PathParameters.Add("contract_address", Immutable.Api.Client.ClientUtils.ParameterToString(contractAddress)); // path parameter
            localVarRequestOptions.QueryParameters.Add(Immutable.Api.Client.ClientUtils.ParameterToMultiMap("multi", "token_id", tokenId));
            if (pageCursor != null)
            {
                localVarRequestOptions.QueryParameters.Add(Immutable.Api.Client.ClientUtils.ParameterToMultiMap("", "page_cursor", pageCursor));
            }


            // make the HTTP request

            var task = this.AsynchronousClient.GetAsync<QuotesForNFTsResult>("/experimental/chains/{chain_name}/quotes/{contract_address}/nfts", localVarRequestOptions, this.Configuration, cancellationToken);

#if UNITY_EDITOR || !UNITY_WEBGL
            var localVarResponse = await task.ConfigureAwait(false);
#else
            var localVarResponse = await task;
#endif

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("QuotesForNFTs", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// Experimental: Get pricing data for a list of stack ids ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of stack ids
        /// </summary>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these stacks are on</param>
        /// <param name="stackId">List of stack ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <returns>QuotesForStacksResult</returns>
        public QuotesForStacksResult QuotesForStacks(string chainName, string contractAddress, List<Guid> stackId, string? pageCursor = default(string?))
        {
            Immutable.Api.Client.ApiResponse<QuotesForStacksResult> localVarResponse = QuotesForStacksWithHttpInfo(chainName, contractAddress, stackId, pageCursor);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Experimental: Get pricing data for a list of stack ids ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of stack ids
        /// </summary>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these stacks are on</param>
        /// <param name="stackId">List of stack ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <returns>ApiResponse of QuotesForStacksResult</returns>
        public Immutable.Api.Client.ApiResponse<QuotesForStacksResult> QuotesForStacksWithHttpInfo(string chainName, string contractAddress, List<Guid> stackId, string? pageCursor = default(string?))
        {
            // verify the required parameter 'chainName' is set
            if (chainName == null)
                throw new Immutable.Api.Client.ApiException(400, "Missing required parameter 'chainName' when calling PricingApi->QuotesForStacks");

            // verify the required parameter 'contractAddress' is set
            if (contractAddress == null)
                throw new Immutable.Api.Client.ApiException(400, "Missing required parameter 'contractAddress' when calling PricingApi->QuotesForStacks");

            // verify the required parameter 'stackId' is set
            if (stackId == null)
                throw new Immutable.Api.Client.ApiException(400, "Missing required parameter 'stackId' when calling PricingApi->QuotesForStacks");

            Immutable.Api.Client.RequestOptions localVarRequestOptions = new Immutable.Api.Client.RequestOptions();

            string[] _contentTypes = new string[] {
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "application/json"
            };

            var localVarContentType = Immutable.Api.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null) localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = Immutable.Api.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null) localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            localVarRequestOptions.PathParameters.Add("chain_name", Immutable.Api.Client.ClientUtils.ParameterToString(chainName)); // path parameter
            localVarRequestOptions.PathParameters.Add("contract_address", Immutable.Api.Client.ClientUtils.ParameterToString(contractAddress)); // path parameter
            localVarRequestOptions.QueryParameters.Add(Immutable.Api.Client.ClientUtils.ParameterToMultiMap("multi", "stack_id", stackId));
            if (pageCursor != null)
            {
                localVarRequestOptions.QueryParameters.Add(Immutable.Api.Client.ClientUtils.ParameterToMultiMap("", "page_cursor", pageCursor));
            }


            // make the HTTP request
            var localVarResponse = this.Client.Get<QuotesForStacksResult>("/experimental/chains/{chain_name}/quotes/{contract_address}/stacks", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("QuotesForStacks", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// Experimental: Get pricing data for a list of stack ids ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of stack ids
        /// </summary>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these stacks are on</param>
        /// <param name="stackId">List of stack ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of QuotesForStacksResult</returns>
        public async System.Threading.Tasks.Task<QuotesForStacksResult> QuotesForStacksAsync(string chainName, string contractAddress, List<Guid> stackId, string? pageCursor = default(string?), System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
        {
            var task = QuotesForStacksWithHttpInfoAsync(chainName, contractAddress, stackId, pageCursor, cancellationToken);
#if UNITY_EDITOR || !UNITY_WEBGL
            Immutable.Api.Client.ApiResponse<QuotesForStacksResult> localVarResponse = await task.ConfigureAwait(false);
#else
            Immutable.Api.Client.ApiResponse<QuotesForStacksResult> localVarResponse = await task;
#endif
            return localVarResponse.Data;
        }

        /// <summary>
        /// Experimental: Get pricing data for a list of stack ids ![Experimental](https://img.shields.io/badge/status-experimental-yellow) Get pricing data for a list of stack ids
        /// </summary>
        /// <exception cref="Immutable.Api.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="chainName">The name of chain</param>
        /// <param name="contractAddress">Contract address for collection that these stacks are on</param>
        /// <param name="stackId">List of stack ids to get pricing data for</param>
        /// <param name="pageCursor">Encoded page cursor to retrieve previous or next page. Use the value returned in the response. (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (QuotesForStacksResult)</returns>
        public async System.Threading.Tasks.Task<Immutable.Api.Client.ApiResponse<QuotesForStacksResult>> QuotesForStacksWithHttpInfoAsync(string chainName, string contractAddress, List<Guid> stackId, string? pageCursor = default(string?), System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
        {
            // verify the required parameter 'chainName' is set
            if (chainName == null)
                throw new Immutable.Api.Client.ApiException(400, "Missing required parameter 'chainName' when calling PricingApi->QuotesForStacks");

            // verify the required parameter 'contractAddress' is set
            if (contractAddress == null)
                throw new Immutable.Api.Client.ApiException(400, "Missing required parameter 'contractAddress' when calling PricingApi->QuotesForStacks");

            // verify the required parameter 'stackId' is set
            if (stackId == null)
                throw new Immutable.Api.Client.ApiException(400, "Missing required parameter 'stackId' when calling PricingApi->QuotesForStacks");


            Immutable.Api.Client.RequestOptions localVarRequestOptions = new Immutable.Api.Client.RequestOptions();

            string[] _contentTypes = new string[] {
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "application/json"
            };


            var localVarContentType = Immutable.Api.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null) localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = Immutable.Api.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null) localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            localVarRequestOptions.PathParameters.Add("chain_name", Immutable.Api.Client.ClientUtils.ParameterToString(chainName)); // path parameter
            localVarRequestOptions.PathParameters.Add("contract_address", Immutable.Api.Client.ClientUtils.ParameterToString(contractAddress)); // path parameter
            localVarRequestOptions.QueryParameters.Add(Immutable.Api.Client.ClientUtils.ParameterToMultiMap("multi", "stack_id", stackId));
            if (pageCursor != null)
            {
                localVarRequestOptions.QueryParameters.Add(Immutable.Api.Client.ClientUtils.ParameterToMultiMap("", "page_cursor", pageCursor));
            }


            // make the HTTP request

            var task = this.AsynchronousClient.GetAsync<QuotesForStacksResult>("/experimental/chains/{chain_name}/quotes/{contract_address}/stacks", localVarRequestOptions, this.Configuration, cancellationToken);

#if UNITY_EDITOR || !UNITY_WEBGL
            var localVarResponse = await task.ConfigureAwait(false);
#else
            var localVarResponse = await task;
#endif

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("QuotesForStacks", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

    }
}