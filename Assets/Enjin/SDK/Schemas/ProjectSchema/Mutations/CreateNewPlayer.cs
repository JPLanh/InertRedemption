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
using JetBrains.Annotations;

namespace Enjin.SDK.ProjectSchema
{
    /// <summary>
    /// Request to set the URI for the metadata of an asset.
    /// </summary>
    /// <seealso cref="IProjectSchema"/>
    [PublicAPI]
    public class CreateNewPlayer : GraphqlRequest<CreateNewPlayer>, IProjectTransactionRequestArguments<CreateNewPlayer>
    {
        /// <summary>
        /// Sole constructor.
        /// </summary>
        public CreateNewPlayer() : base("enjin.sdk.project.CreateNewPlayer")
        {
        }

        public CreateNewPlayer IdentityId(int? identityId)
        {
            return SetVariable("identityId", identityId);
        }

        public CreateNewPlayer Name(string? name)
        {
            return SetVariable("name", name);
        }
    }
}