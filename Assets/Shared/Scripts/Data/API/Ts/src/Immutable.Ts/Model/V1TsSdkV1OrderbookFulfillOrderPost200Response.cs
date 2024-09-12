/*
 * TS SDK API
 *
 * running ts sdk as an api
 *
 * The version of the OpenAPI document: 1.0.0
 * Contact: contact@immutable.com
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using OpenAPIDateConverter = Immutable.Ts.Client.OpenAPIDateConverter;

namespace Immutable.Ts.Model
{
    /// <summary>
    /// Response schema for the fulfillOrder endpoint
    /// </summary>
    [DataContract(Name = "_v1_ts_sdk_v1_orderbook_fulfillOrder_post_200_response")]
    public partial class V1TsSdkV1OrderbookFulfillOrderPost200Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="V1TsSdkV1OrderbookFulfillOrderPost200Response" /> class.
        /// </summary>
        /// <param name="actions">actions.</param>
        /// <param name="expiration">User MUST submit the fulfillment transaction before the expiration Submitting after the expiration will result in a on chain revert.</param>
        /// <param name="order">order.</param>
        public V1TsSdkV1OrderbookFulfillOrderPost200Response(List<FulfillOrderResBodyAction> actions = default(List<FulfillOrderResBodyAction>), string expiration = default(string), FulfillOrderResBodyOrder order = default(FulfillOrderResBodyOrder))
        {
            this.Actions = actions;
            this.Expiration = expiration;
            this.Order = order;
        }

        /// <summary>
        /// Gets or Sets Actions
        /// </summary>
        [DataMember(Name = "actions", EmitDefaultValue = false)]
        public List<FulfillOrderResBodyAction> Actions { get; set; }

        /// <summary>
        /// User MUST submit the fulfillment transaction before the expiration Submitting after the expiration will result in a on chain revert
        /// </summary>
        /// <value>User MUST submit the fulfillment transaction before the expiration Submitting after the expiration will result in a on chain revert</value>
        [DataMember(Name = "expiration", EmitDefaultValue = false)]
        public string Expiration { get; set; }

        /// <summary>
        /// Gets or Sets Order
        /// </summary>
        [DataMember(Name = "order", EmitDefaultValue = false)]
        public FulfillOrderResBodyOrder Order { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class V1TsSdkV1OrderbookFulfillOrderPost200Response {\n");
            sb.Append("  Actions: ").Append(Actions).Append("\n");
            sb.Append("  Expiration: ").Append(Expiration).Append("\n");
            sb.Append("  Order: ").Append(Order).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

    }

}