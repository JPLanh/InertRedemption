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
    public class EnjinTransaction
    {
		[JsonProperty("id")]
		public int? Id { get; private set; }

		[JsonProperty("transactionId")]
		public string? TransactionId { get; private set; }

		[JsonProperty("title")]
		public string? Title { get; private set; }

		[JsonProperty("contract")]
		public string? Contract { get; private set; }

		[JsonProperty("type")]
		public string? Type { get; private set; }

		[JsonProperty("icon")]
		public string? Icon { get; private set; }

		[JsonProperty("userId")]
		public int? Userid { get; private set; }

		[JsonProperty("value")]
		public string? Value { get; private set; }

		[JsonProperty("retryState")]
		public string? RetryState { get; private set; }

		[JsonProperty("state")]
		public string? State { get; private set; }

		[JsonProperty("accepted")]
		public bool? Accepted { get; private set; }

		[JsonProperty("appWallet")]
		public bool? Appwallet { get; private set; }

		[JsonProperty("app")]
		public EnjinApp? App { get; private set; }

		[JsonProperty("appId")]
		public int? Appid { get; private set; }

		[JsonProperty("events")]
		public List<EnjinTokenEvent>? Events { get; private set; }

		[JsonProperty("user")]
		public EnjinUser? User { get; private set; }

		[JsonProperty("token")]
		public EnjinToken? Token { get; private set; }

		[JsonProperty("tokenId")]
		public string? Tokenid { get; private set; }

		[JsonProperty("wallet")]
		public EnjinWallet? Wallet { get; private set; }

		[JsonProperty("encodedData")]
		public string? EncodedData { get; private set; }

		[JsonProperty("signedTransaction")]
		public string? SignedTransaction { get; private set; }

		[JsonProperty("signedBackupTransaction")]
		public string? SignedBackupTransaction { get; private set; }

		[JsonProperty("signedCancelTransaction")]
		public string? SignedCancelTransaction { get; private set; }

		[JsonProperty("receipt")]
		public object? Receipt { get; private set; }

		[JsonProperty("error")]
		public string? Error { get; private set; }

		[JsonProperty("nonce")]
		public string? Nonce { get; private set; }

		[JsonProperty("createdAt")]
		public string? CreatedAt { get; private set; }

		[JsonProperty("updatedAt")]
		public string? UpdatedAt { get; private set; }

		[JsonProperty("items")]
		public List<EnjinTransaction>? Items { get; private set; }

		[JsonProperty("cursor")]
		public PaginationCursor? Cursor { get; private set; }

	}
}