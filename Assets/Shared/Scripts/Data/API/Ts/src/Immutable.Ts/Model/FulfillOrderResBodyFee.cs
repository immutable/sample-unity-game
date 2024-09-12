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
    /// FulfillOrderResBodyFee
    /// </summary>
    [DataContract(Name = "fulfillOrderResBodyFee")]
    public partial class FulfillOrderResBodyFee
    {

        /// <summary>
        /// Gets or Sets Type
        /// </summary>
        [DataMember(Name = "type", EmitDefaultValue = false)]
        public FulfillOrderResBodyFeeType? Type { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="FulfillOrderResBodyFee" /> class.
        /// </summary>
        /// <param name="amount">amount.</param>
        /// <param name="recipientAddress">recipientAddress.</param>
        /// <param name="type">type.</param>
        public FulfillOrderResBodyFee(string amount = default(string), string recipientAddress = default(string), FulfillOrderResBodyFeeType? type = default(FulfillOrderResBodyFeeType?))
        {
            this.Amount = amount;
            this.RecipientAddress = recipientAddress;
            this.Type = type;
        }

        /// <summary>
        /// Gets or Sets Amount
        /// </summary>
        [DataMember(Name = "amount", EmitDefaultValue = false)]
        public string Amount { get; set; }

        /// <summary>
        /// Gets or Sets RecipientAddress
        /// </summary>
        [DataMember(Name = "recipientAddress", EmitDefaultValue = false)]
        public string RecipientAddress { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class FulfillOrderResBodyFee {\n");
            sb.Append("  Amount: ").Append(Amount).Append("\n");
            sb.Append("  RecipientAddress: ").Append(RecipientAddress).Append("\n");
            sb.Append("  Type: ").Append(Type).Append("\n");
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