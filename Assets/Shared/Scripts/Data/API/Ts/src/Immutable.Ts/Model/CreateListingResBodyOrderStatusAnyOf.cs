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
    /// CreateListingResBodyOrderStatusAnyOf
    /// </summary>
    [DataContract(Name = "createListingResBodyOrderStatus_anyOf")]
    public partial class CreateListingResBodyOrderStatusAnyOf
    {
        /// <summary>
        /// The order status that indicates an order can be fulfilled.
        /// </summary>
        /// <value>The order status that indicates an order can be fulfilled.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum NameEnum
        {
            /// <summary>
            /// Enum ACTIVE for value: ACTIVE
            /// </summary>
            [EnumMember(Value = "ACTIVE")]
            ACTIVE = 1
        }


        /// <summary>
        /// The order status that indicates an order can be fulfilled.
        /// </summary>
        /// <value>The order status that indicates an order can be fulfilled.</value>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public NameEnum? Name { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateListingResBodyOrderStatusAnyOf" /> class.
        /// </summary>
        /// <param name="name">The order status that indicates an order can be fulfilled..</param>
        public CreateListingResBodyOrderStatusAnyOf(NameEnum? name = default(NameEnum?))
        {
            this.Name = name;
        }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class CreateListingResBodyOrderStatusAnyOf {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
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