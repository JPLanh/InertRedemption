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
    public class EnjinApp
    {
        [JsonProperty("id")]
        public int? Id { get; private set; }

        [JsonProperty("uuid")]
        public string? Uuid { get; private set; }

        [JsonProperty("secret")]
        public string? Secret { get; private set; }

        [JsonProperty("description")]
        public string? Description { get; private set; }

        [JsonProperty("image")]
        public string? Image { get; private set; }

        [JsonProperty("owner")]
        public EnjinUser? Owner { get; private set; }

        [JsonProperty("identities")]
        public List<EnjinIdentity>? Identities { get; private set; }

        [JsonProperty("identity")]
        public EnjinIdentity? Identity { get; private set; }

        [JsonProperty("transaction")]
        public List<EnjinTransaction>? Transaction { get; private set; }

        [JsonProperty("tokens")]
        public List<EnjinToken>? Tokens { get; private set; }

        [JsonProperty("wallets")]
        public List<EnjinWallet>? wallets { get; private set; }

        [JsonProperty("tokenCount")]
        public int? TokenCount { get; private set; }

        [JsonProperty("createdAt")]
        public string? CreatedAt { get; private set; }

        [JsonProperty("updatedAt")]
        public string? UpdatedAt { get; private set; }

        [JsonProperty("items")]
        public List<EnjinApp>? Items { get; private set; }

        [JsonProperty("cursor")]
        public PaginationCursor? Cursor { get; private set; }
    }
}