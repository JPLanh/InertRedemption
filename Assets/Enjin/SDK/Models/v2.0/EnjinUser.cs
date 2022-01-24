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
    public class EnjinUser
    {
        /// <summary>
        /// Represents the ID of this player.
        /// </summary>
        /// <value>The player ID.</value>
        [JsonProperty("id")]
        public string? Id { get; private set; }

        [JsonProperty("name")]
        public string? Name { get; private set; }

        [JsonProperty("accessTokens")]
        public List<AccessToken>? AccessTokens { get; private set; }

        [JsonProperty("isPlayer")]
        public bool? IsPlayer { get; private set; }

        [JsonProperty("app")]
        public EnjinApp? App { get; private set; }

        [JsonProperty("apps")]
        public List<EnjinApp>? Apps { get; private set; }

        [JsonProperty("identities")]
        public List<EnjinIdentity>? Identities { get; private set; }

        [JsonProperty("createdAt")]
        public string? CreatedAt { get; private set; }

        [JsonProperty("updatedAt")]
        public string? UpdatedAt { get; private set; }

        [JsonProperty("items")]
        public List<EnjinUser>? Items { get; private set; }

        [JsonProperty("cursor")]
        public PaginationCursor? Cursor { get; private set; }
    }

    public class accessToken
    {
        [JsonProperty("appId")]
        public string? AppId { get; private set; }

        [JsonProperty("accessToken")]
        public string? AcessToken { get; private set; }

        [JsonProperty("refreshToken")]
        public string? RefreshToken { get; private set; }
    }
}