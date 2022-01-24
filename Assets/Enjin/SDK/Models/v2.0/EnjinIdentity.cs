/* Copyright 2021 Enjin Pte. Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Enjin.SDK.Models.v2
{
    /// <summary>
    /// Models a player on a project.
    /// </summary>
    [PublicAPI]
    public class EnjinIdentity
    {

        [JsonProperty("id")]
        public string? Id { get; private set; }

        [JsonProperty("appId")]
        public string? AppId { get; private set; }

        [JsonProperty("linkingCode")]
        public string? LinkingCode { get; private set; }

        [JsonProperty("linkingCodeQr")]
        public string? LinkingCodeQr { get; private set; }

        [JsonProperty("app")]
        public EnjinApp? App { get; private set; }

        [JsonProperty("user")]
        public EnjinUser? User { get; private set; }

        [JsonProperty("transaction")]
        public List<EnjinTransaction>? Transaction { get; private set; }

        [JsonProperty("tokens")]
        public List<EnjinToken>? Tokens { get; private set; }

        [JsonProperty("wallet")]
        public Wallet? Wallet { get; private set; }

        /// <summary>
        /// Represents the datetime when this player was created.
        /// </summary>
        /// <value>The datetime.</value>
        /// <remarks>
        /// The datetime is formatted using the ISO 8601 date format.
        /// </remarks>
        [JsonProperty("createdAt")]
        public string? CreatedAt { get; private set; }

        /// <summary>
        /// Represents the datetime when this player was last updated.
        /// </summary>
        /// <value>The datetime.</value>
        /// <remarks>
        /// The datetime is formatted using the ISO 8601 date format.
        /// </remarks>
        [JsonProperty("updatedAt")]
        public string? UpdatedAt { get; private set; }

        [JsonProperty("items")]
        public List<EnjinIdentity>? Items { get; private set; }

        [JsonProperty("cursor")]
        public PaginationCursor? Cursor { get; private set; }
    }
}