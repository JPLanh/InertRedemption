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

using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Enjin.SDK.Models.v2
{
	/// <summary>
	/// Models a wallet on the platform.
	/// </summary>
	[PublicAPI]
	public class GasPrice
	{
		[JsonProperty("fast")]
		public long Fast { get; private set; }

		[JsonProperty("fastest")]
		public long Fastest { get; private set; }

		[JsonProperty("safeLow")]
		public long SafeLow { get; private set; }

		[JsonProperty("average")]
		public long Average { get; private set; }
	}
}