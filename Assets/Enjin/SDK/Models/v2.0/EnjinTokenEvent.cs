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
    [PublicAPI]
    public class EnjinTokenEvent
    {
        [JsonProperty("id")]
        public int? Id { get; private set; }

        [JsonProperty("tokenId")]
        public string? Tokenid { get; private set; }

        [JsonProperty("event")]
        public string? Event { get; private set; }

        [JsonProperty("param1")]
        public string? Param1 { get; private set; }

        [JsonProperty("param2")]
        public string? Param2 { get; private set; }

        [JsonProperty("param3")]
        public string? Param3 { get; private set; }

        [JsonProperty("param4")]
        public string? Param4 { get; private set; }

        [JsonProperty("blockNumber")]
        public int? Blocknumber { get; private set; }

        [JsonProperty("token")]
        public EnjinToken? Token { get; private set; }

        [JsonProperty("transaction")]
        public EnjinTransaction? Transaction { get; private set; }

        [JsonProperty("createdAt")]
        public string? Createdat { get; private set; }

        [JsonProperty("updatedAt")]
        public string? Updatedat { get; private set; }

        [JsonProperty("items")]
        public List<EnjinTokenEvent>? Items { get; private set; }

        [JsonProperty("cursor")]
        public PaginationCursor? Cursor { get; private set; }
    }
}