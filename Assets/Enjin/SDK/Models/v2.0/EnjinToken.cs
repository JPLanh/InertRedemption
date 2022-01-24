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
    public class EnjinToken
    {
        [JsonProperty("id")]
        public string? Id { get; private set; }

        [JsonProperty("name")]
        public string? Name { get; private set; }

        [JsonProperty("appId")]
        public int? Appid { get; private set; }

        [JsonProperty("blockHeight")]
        public int? Blockheight { get; private set; }

        [JsonProperty("creator")]
        public string? Creator { get; private set; }

        [JsonProperty("firstBlock")]
        public int? Firstblock { get; private set; }

        [JsonProperty("icon")]
        public string? Icon { get; private set; }

        [JsonProperty("meltFeeRatio")]
        public int? Meltfeeratio { get; private set; }

        [JsonProperty("meltFeeMaxRatio")]
        public int? Meltfeemaxratio { get; private set; }

        [JsonProperty("meltValue")]
        public long? Meltvalue { get; private set; }

        [JsonProperty("metadata")]
        public string? Metadata { get; private set; }

        [JsonProperty("metadataURI")]
        public string? Metadatauri { get; private set; }

        [JsonProperty("nonFungible")]
        public bool? Nonfungible { get; private set; }

        [JsonProperty("reserve")]
        public string? Reserve { get; private set; }

        [JsonProperty("supplyModel")]
        public string? Supplymodel { get; private set; }

        [JsonProperty("circulatingSupply")]
        public long? Circulatingsupply { get; private set; }

        [JsonProperty("mintableSupply")]
        public long? Mintablesupply { get; private set; }

        [JsonProperty("totalSupply")]
        public long? Totalsupply { get; private set; }

        //[JsonProperty("transferable")]
        //public string? Transferable { get; private set; }

        //[JsonProperty("transferFeeSettings")]
        //public EnjinTokenTransferFeeSettings? Transferfeesettings { get; private set; }

        //[JsonProperty("settingsMask")]
        //public int? Settingsmask { get; private set; }

        //[JsonProperty("app")]
        //public EnjinApp? App { get; private set; }

        //[JsonProperty("identities")]
        //public List<EnjinIdentity>? Identities { get; private set; }

        //[JsonProperty("tokenEvents")]
        //public List<EnjinTokenEvent>? Tokenevents { get; private set; }

        //[JsonProperty("transactions")]
        //public List<EnjinTransaction>? Transactions { get; private set; }

        //[JsonProperty("variantMode")]
        //public string? Variantmode { get; private set; }

        //[JsonProperty("variants")]
        //public List<EnjinTokenVariant>? Variants { get; private set; }

        //[JsonProperty("wallet")]
        //public EnjinWallet? Wallet { get; private set; }

        [JsonProperty("createdAt")]
        public string? Createdat { get; private set; }

        [JsonProperty("updatedAt")]
        public string? Updatedat { get; private set; }

        [JsonProperty("items")]
        public List<EnjinToken>? Items { get; private set; }

        [JsonProperty("cursor")]
        public PaginationCursor? Cursor { get; private set; }
    }

    [PublicAPI]
    public class EnjinTokenTransferFeeSettings
    {
        [JsonProperty("type")]
        public string Type { get; private set; }

        [JsonProperty("assetId")]
        public string AssetId { get; private set; }

        [JsonProperty("value")]
        public string Value { get; private set; }
    }

    [PublicAPI]
    public class EnjinTokenVariant
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("assetId")]
        public string AssetId { get; private set; }

        [JsonProperty("variantMetadata")]
        public string VariantMetadata { get; private set; }

        [JsonProperty("flags")]
        public int Flags { get; private set; }

        [JsonProperty("usageCount")]
        public int UsageCount { get; private set; }

        [JsonProperty("createdAt")]
        public string CreatedAt { get; private set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; private set; }

        [JsonProperty("items")]
        public List<EnjinTokenVariant> Items { get; private set; }

        [JsonProperty("cursor")]
        public PaginationCursor Cursor { get; private set; }
    }
}