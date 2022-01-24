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
    public class EnjinBalance
    {
        [JsonProperty("id")]
        public string? Id { get; private set; }

        [JsonProperty("index")]
        public string? Index { get; private set; }

        [JsonProperty("value")]
        public int? Value { get; private set; }

        [JsonProperty("app")]
        public EnjinApp? App { get; private set; }

        [JsonProperty("token")]
        public EnjinToken? Token { get; private set; }

        [JsonProperty("identity")]
        public EnjinIdentity? Identity { get; private set; }

        [JsonProperty("wallet")]
        public EnjinWallet? Wallet { get; private set; }

        [JsonProperty("items")]
        public List<EnjinBalance>? Items { get; private set; }

        [JsonProperty("cursor")]
        public PaginationCursor? Cursor { get; private set; }
    }
}