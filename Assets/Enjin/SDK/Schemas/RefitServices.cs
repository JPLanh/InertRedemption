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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Enjin.SDK.Graphql;
using Enjin.SDK.Models;
using Enjin.SDK.Models.v2;
using Newtonsoft.Json.Linq;
using Refit;

[assembly: InternalsVisibleTo("TestSuite")]
namespace Enjin.SDK
{
    [Headers("Content-Type: application/json")]
    internal interface IPlayerService : IAuth, IGetOne<EnjinUser>, IGetMany<EnjinUser>, IDelete
    {
    }

    //[Headers("Content-Type: application/json")]
    //internal interface IBalancesService : IGetMany<EnjinBalance>
    //{
    //}

    [Headers("Content-Type: application/json")]
    internal interface IProjectService : IAuth, IGetOne<Project>
    {
    }

    [Headers("Content-Type: application/json")]
    internal interface IBalanceService : IGetMany<EnjinBalance>
    {
    }

    [Headers("Content-Type: application/json")]
    internal interface IPlatformService : IGetOne<Platform>
    {
        [Post("/graphql/{schema}")]
        Task<ApiResponse<GraphqlResponse<GasPrices>>> GetGasPrices(string schema,
            [Body(BodySerializationMethod.Serialized)] JObject body);
    }
    
    [Headers("Content-Type: application/json")]
    internal interface IRequestService : IGetOne<EnjinTransaction>, IGetMany<EnjinTransaction>, IDelete
    {
    }
    
    [Headers("Content-Type: application/json")]
    internal interface IAssetService : IGetOne<EnjinToken>, IGetMany<EnjinToken>, IDelete
    {
    }

    [Headers("Content-Type: application/json")]
    internal interface IWalletService : IGetOne<Wallet>, IGetMany<Wallet>
    {
    }

    [Headers("Content-Type: application/json")]
    internal interface IAuth
    {
        [Post("/graphql/default")]
        Task<ApiResponse<GraphqlResponse<AccessToken>>> Auth(string schema,
            [Body(BodySerializationMethod.Serialized)] JObject body);

        [Post("/graphql/default")]
        Task<ApiResponse<GraphqlResponse<EnjinUser>>> EnjinOAuth(string schema,
            [Body(BodySerializationMethod.Serialized)] JObject body);
    }

    [Headers("Content-Type: application/json")]
    internal interface IGetMany<T>
    {
        [Post("/graphql/default")]
        Task<ApiResponse<GraphqlResponse<List<T>>>> GetMany(string schema,
            [Body(BodySerializationMethod.Serialized)] JObject body);
    }
    
    [Headers("Content-Type: application/json")]
    internal interface IGetOne<T>
    {
        [Post("/graphql/default")]
        Task<ApiResponse<GraphqlResponse<T>>> GetOne(string schema,
            [Body(BodySerializationMethod.Serialized)] JObject body);
    }

    [Headers("Content-Type: application/json")]
    internal interface IDelete
    {
        [Post("/graphql/{schema}")]
        Task<ApiResponse<GraphqlResponse<bool>>> Delete(string schema,
            [Body(BodySerializationMethod.Serialized)] JObject body);
    }
}