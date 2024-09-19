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
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Immutable.Ts.Model
{
    /// <summary>
    ///     OrderComponentsOfferInner
    /// </summary>
    [DataContract(Name = "OrderComponents_offer_inner")]
    public class OrderComponentsOfferInner
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OrderComponentsOfferInner" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected OrderComponentsOfferInner()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrderComponentsOfferInner" /> class.
        /// </summary>
        /// <param name="endAmount">endAmount (required).</param>
        /// <param name="identifierOrCriteria">identifierOrCriteria (required).</param>
        /// <param name="itemType">itemType (required).</param>
        /// <param name="startAmount">startAmount (required).</param>
        /// <param name="token">token (required).</param>
        public OrderComponentsOfferInner(string endAmount = default, string identifierOrCriteria = default,
            ItemType itemType = default, string startAmount = default, string token = default)
        {
            // to ensure "endAmount" is required (not null)
            if (endAmount == null)
                throw new ArgumentNullException(
                    "endAmount is a required property for OrderComponentsOfferInner and cannot be null");
            EndAmount = endAmount;
            // to ensure "identifierOrCriteria" is required (not null)
            if (identifierOrCriteria == null)
                throw new ArgumentNullException(
                    "identifierOrCriteria is a required property for OrderComponentsOfferInner and cannot be null");
            IdentifierOrCriteria = identifierOrCriteria;
            ItemType = itemType;
            // to ensure "startAmount" is required (not null)
            if (startAmount == null)
                throw new ArgumentNullException(
                    "startAmount is a required property for OrderComponentsOfferInner and cannot be null");
            StartAmount = startAmount;
            // to ensure "token" is required (not null)
            if (token == null)
                throw new ArgumentNullException(
                    "token is a required property for OrderComponentsOfferInner and cannot be null");
            Token = token;
        }

        /// <summary>
        ///     Gets or Sets ItemType
        /// </summary>
        [DataMember(Name = "itemType", IsRequired = true, EmitDefaultValue = true)]
        public ItemType ItemType { get; set; }

        /// <summary>
        ///     Gets or Sets EndAmount
        /// </summary>
        [DataMember(Name = "endAmount", IsRequired = true, EmitDefaultValue = true)]
        public string EndAmount { get; set; }

        /// <summary>
        ///     Gets or Sets IdentifierOrCriteria
        /// </summary>
        [DataMember(Name = "identifierOrCriteria", IsRequired = true, EmitDefaultValue = true)]
        public string IdentifierOrCriteria { get; set; }

        /// <summary>
        ///     Gets or Sets StartAmount
        /// </summary>
        [DataMember(Name = "startAmount", IsRequired = true, EmitDefaultValue = true)]
        public string StartAmount { get; set; }

        /// <summary>
        ///     Gets or Sets Token
        /// </summary>
        [DataMember(Name = "token", IsRequired = true, EmitDefaultValue = true)]
        public string Token { get; set; }

        /// <summary>
        ///     Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class OrderComponentsOfferInner {\n");
            sb.Append("  EndAmount: ").Append(EndAmount).Append("\n");
            sb.Append("  IdentifierOrCriteria: ").Append(IdentifierOrCriteria).Append("\n");
            sb.Append("  ItemType: ").Append(ItemType).Append("\n");
            sb.Append("  StartAmount: ").Append(StartAmount).Append("\n");
            sb.Append("  Token: ").Append(Token).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        ///     Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}