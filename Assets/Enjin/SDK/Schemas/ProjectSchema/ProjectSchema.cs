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
using System.Threading.Tasks;
using Enjin.SDK.Graphql;
using Enjin.SDK.Models;
using Enjin.SDK.Models.v2;
using Enjin.SDK.Shared;
using Enjin.SDK.Utils;
using JetBrains.Annotations;
using Enjin.SDK.PlayerSchema;

namespace Enjin.SDK.ProjectSchema
{
    /// <summary>
    /// Class for sending requests in the project schema.
    /// </summary>
    [PublicAPI]
    public class ProjectSchema : SharedSchema, IProjectSchema
    {
        private const string SCHEMA = "project";

        internal readonly IPlayerService PlayerService;
        internal readonly IBalanceService PlayerBalanceService;
        internal readonly IWalletService WalletService;
//        internal readonly IAssetService AssetService;

        /// <summary>
        /// Sole constructor.
        /// </summary>
        /// <param name="middleware">The middleware.</param>
        /// <param name="loggerProvider">The logger provider.</param>
        public ProjectSchema(TrustedPlatformMiddleware middleware, LoggerProvider loggerProvider) :
            base(middleware, SCHEMA, loggerProvider)
        {
            PlayerService = CreateService<IPlayerService>();
            WalletService = CreateService<IWalletService>();
            PlayerBalanceService = CreateService<IBalanceService>();
  //          AssetService = CreateService<IAssetService>();
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> AdvancedSendAsset(AdvancedSendAsset request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> ApproveEnj(ApproveEnj request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> ApproveMaxEnj(ApproveMaxEnj request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<AccessToken>> AuthPlayer(AuthPlayer request)
        {
            return SendRequest(PlayerService.Auth(Schema, CreateRequestBody(request)));
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<AccessToken>> AuthProject(AuthProject request)
        {
            return SendRequest(ProjectService.Auth(Schema, CreateRequestBody(request)));
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinUser>> AuthUser(EnjinOauth request)
        {
            return SendRequest(ProjectService.EnjinOAuth(Schema, CreateRequestBody(request)));
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> BridgeAsset(BridgeAsset request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> BridgeAssets(BridgeAssets request)
        {
            return TransactionRequest(request);
        }

        ///// <inheritdoc/>
        //public Task<GraphqlResponse<List<EnjinToken>>> getAssets(FindAssetToken request)
        //{
        //    return SendRequest(AssetService.GetMany(Schema, CreateRequestBody(request)));
        //}

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> BridgeClaimAsset(BridgeClaimAsset request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> CompleteTrade(CompleteTrade request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> CreateAsset(CreateAsset request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<AccessToken>> CreatePlayer(CreatePlayer request)
        {
            return SendRequest(PlayerService.Auth(Schema, CreateRequestBody(request)));
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> CreateTrade(CreateTrade request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> DecreaseMaxMeltFee(DecreaseMaxMeltFee request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> DecreaseMaxTransferFee(DecreaseMaxTransferFee request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<bool>> DeletePlayer(DeletePlayer request)
        {
            return SendRequest(PlayerService.Delete(Schema, CreateRequestBody(request)));
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinUser>> GetPlayer(GetPlayer request)
        {
            return SendRequest(PlayerService.GetOne(Schema, CreateRequestBody(request)));
        }

        ///// <inheritdoc/>
        //public Task<GraphqlResponse<List<EnjinBalance>>> GetBalances(PlayerSchema.GetBalances request)
        //{
        //    return SendRequest(PlayerBalanceService.GetMany(Schema, CreateRequestBody(request)));
        //}

        /// <inheritdoc/>
        public Task<GraphqlResponse<List<EnjinUser>>> GetPlayers(GetPlayers request)
        {
            return SendRequest(PlayerService.GetMany(Schema, CreateRequestBody(request)));
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<Wallet>> GetWallet(GetWallet request)
        {
            return SendRequest(WalletService.GetOne(Schema, CreateRequestBody(request)));
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<List<Wallet>>> GetWallets(GetWallets request)
        {
            return SendRequest(WalletService.GetMany(Schema, CreateRequestBody(request)));
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<bool>> InvalidateAssetMetadata(InvalidateAssetMetadata request)
        {
            return SendRequest(AssetService.Delete(Schema, CreateRequestBody(request)));
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> MeltAsset(MeltAsset request)
        {
            return TransactionRequest(request);
        }

        public Task<GraphqlResponse<EnjinTransaction>> CreateEnjinRequest(CreateNewCharacter request)
        {
            return TransactionRequest(request);
        }

        public Task<GraphqlResponse<EnjinTransaction>> CreateNewCharacter(CreateNewCharacter request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> Message(Message request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> MintAsset(MintAsset request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> ReleaseReserve(ReleaseReserve request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> ResetEnjApproval(ResetEnjApproval request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> SendEnj(SendEnj request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> SendAsset(SendAsset request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> SetApprovalForAll(SetApprovalForAll request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> SetMeltFee(SetMeltFee request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> SetTransferable(SetTransferable request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> SetTransferFee(SetTransferFee request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> SetUri(SetUri request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<EnjinTransaction>> SetWhitelisted(SetWhitelisted request)
        {
            return TransactionRequest(request);
        }

        /// <inheritdoc/>
        public Task<GraphqlResponse<bool>> UnlinkWallet(UnlinkWallet request)
        {
            return SendRequest(PlayerService.Delete(Schema, CreateRequestBody(request)));
        }
    }
}