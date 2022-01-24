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
using Enjin.SDK.Shared;
using JetBrains.Annotations;

namespace Enjin.SDK.ProjectSchema
{
    /// <summary>
    /// Request for getting information about the player.
    /// </summary>
    /// <seealso cref="Enjin.SDK.Models.v2.EnjinUser"/>
    /// <seealso cref="IPlayerSchema"/>
    [PublicAPI]
    public class EnjinOauth : GraphqlRequest<EnjinOauth>, IPlayerFragmentArguments<EnjinOauth>
    {
        /// <summary>
        /// Sole constructor.
        /// </summary>
        public EnjinOauth() : base("enjin.sdk.project.EnjinOauth")
        {
        }

        /// <summary>
        /// Sets the player ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>This request for chaining.</returns>
        public EnjinOauth Email(string? email)
        {
            return SetVariable("email", email);
        }

        public EnjinOauth Password(string? password)
        {
            return SetVariable("password", password);
        }

        public EnjinOauth Name(string? name)
        {
            return SetVariable("name", name);
        }
    }
}