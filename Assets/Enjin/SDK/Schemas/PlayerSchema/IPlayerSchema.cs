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

using System.Threading.Tasks;
using Enjin.SDK.Graphql;
using Enjin.SDK.Models;
using Enjin.SDK.Models.v2;
using Enjin.SDK.Shared;
using JetBrains.Annotations;

namespace Enjin.SDK.PlayerSchema
{
    /// <summary>
    /// Interface for player schema implementation.
    /// </summary>
    [PublicAPI]
    public interface IPlayerSchema : ISharedSchema
    {
        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.AdvancedSendAsset"/> request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The task.</returns>
        Task<GraphqlResponse<EnjinTransaction>> AdvancedSendAsset(AdvancedSendAsset request);

        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.ApproveEnj"/> request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The task.</returns>
        Task<GraphqlResponse<EnjinTransaction>> ApproveEnj(ApproveEnj request);

        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.ApproveMaxEnj"/> request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The task.</returns>
        Task<GraphqlResponse<EnjinTransaction>> ApproveMaxEnj(ApproveMaxEnj request);

        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.BridgeAsset"/> request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The task.</returns>
        Task<GraphqlResponse<EnjinTransaction>> BridgeAsset(BridgeAsset request);

        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.BridgeAssets"/> request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The task.</returns>
        Task<GraphqlResponse<EnjinTransaction>> BridgeAssets(BridgeAssets request);

        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.BridgeClaimAsset"/> request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The task.</returns>
        Task<GraphqlResponse<EnjinTransaction>> BridgeClaimAsset(BridgeClaimAsset request);

        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.GetPlayer"/> request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The task.</returns>
        Task<GraphqlResponse<EnjinUser>> GetPlayer(GetPlayer request);

        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.GetWallet"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<GraphqlResponse<Wallet>> GetWallet(GetWallet request);

        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.MeltAsset"/> request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The task.</returns>
        Task<GraphqlResponse<EnjinTransaction>> MeltAsset(MeltAsset request);

        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.Message"/> request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The task.</returns>
        Task<GraphqlResponse<EnjinTransaction>> Message(Message request);

        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.ResetEnjApproval"/> request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The task.</returns>
        Task<GraphqlResponse<EnjinTransaction>> ResetEnjApproval(ResetEnjApproval request);

        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.SendAsset"/> request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The task.</returns>
        Task<GraphqlResponse<EnjinTransaction>> SendAsset(SendAsset request);

        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.SendEnj"/> request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The task.</returns>
        Task<GraphqlResponse<EnjinTransaction>> SendEnj(SendEnj request);

        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.SetApprovalForAll"/> request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The task.</returns>
        Task<GraphqlResponse<EnjinTransaction>> SetApprovalForAll(SetApprovalForAll request);

        /// <summary>
        /// Creates a task and sends the <see cref="Enjin.SDK.PlayerSchema.UnlinkWallet"/> request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The task.</returns>
        Task<GraphqlResponse<bool>> UnlinkWallet(UnlinkWallet request);
    }
}