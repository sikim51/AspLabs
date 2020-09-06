// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNetCore.WebHooks
{
    /// <summary>
    /// Contains information sent in a WebHook notification from Crisp. Describes the API request that caused the
    /// event. See <see href="https://stripe.com/docs/api/curl#event_object-request"/> for details.
    /// </summary>
    public class CrispRequestData
    {
        /// <summary>
        /// Gets or sets the Web Site Id of the API request that caused the event.
        /// </summary>
        /// <value><see langword="null"/> if the event was automatic. Otherwise, the API request ID.</value>
        [JsonProperty("website_id")]
        public string WebSiteId { get; set; }

        /// <summary>
        /// Gets or sets the idempotency key transmitted in the API request.
        /// </summary>
        /// <remarks>This property is only populated for events on or after May 23, 2017.</remarks>
        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("data")]
        public JObject Data { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }
    }
}
