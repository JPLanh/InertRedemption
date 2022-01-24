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

using Enjin.SDK.Graphql;
using Enjin.SDK.Models;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace Enjin.SDK.ProjectSchema
{
    /// <summary>
    /// Request for minting a asset.
    /// </summary>
    /// <seealso cref="IProjectSchema"/>
    [PublicAPI]
    public class MintAsset : GraphqlRequest<MintAsset>, IProjectTransactionRequestArguments<MintAsset>
    {
        /// <summary>
        /// Sole constructor.
        /// </summary>
        public MintAsset() : base("enjin.sdk.project.MintAsset")
        {
        }

        public MintAsset IdentityID(string? identityID)
        {
            return SetVariable("identityID", identityID);
        }

        public MintAsset Token_id(string? token_id)
        {
            return SetVariable("token_id", token_id);
        }

        public MintAsset Recipients(List<string>? recipients)
        {
            return SetVariable("recipients", recipients);
        }

    }
}