using Core.Collections;
using FactStore.Api.Models;
using FactStore.Models.Parameters;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FactStore.Api.Infrastructure.DataStores
{
    public interface IFactStore
    {
        #region Facts
        Task<IPaginatedList<FactEntity>> GetFactsAsync(FactParameters factParameters, CancellationToken cancellationToken = default);
        Task<IPaginatedList<FactEntity>> GetFactsAsync(string factType, FactParameters factParameters, CancellationToken cancellationToken = default);
        Task<FactEntity> GetFactAsync(string factTypeName, string key, CancellationToken cancellationToken = default);
        Task<FactEntity> AddFactAsync(FactEntity fact, CancellationToken cancellationToken = default);
        Task<FactEntity> UpdateFactAsync(FactEntity fact, CancellationToken cancellationToken = default);
        Task DeleteFactAsync(FactEntity fact, CancellationToken cancellationToken = default);
        #endregion

        #region Fact Types
        Task<IPaginatedList<FactTypeEntity>> GetFactTypesAsync(FactTypeParameters factTypeParameters, CancellationToken cancellationToken = default);
        Task<FactTypeEntity> GetFactTypeByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<FactTypeEntity> AddFactTypeAsync(FactTypeEntity factType, CancellationToken cancellationToken = default);
        Task<FactTypeEntity> UpdateFactTypeAsync(FactTypeEntity factType, CancellationToken cancellationToken = default);
        Task DeleteFactTypeAsync(FactTypeEntity factType, CancellationToken cancellationToken = default);
        #endregion

        #region Roles
        Task<IPaginatedList<RoleEntity>> GetRolesAsync(RoleParameters roleParameters, CancellationToken cancellationToken = default);
        Task<IPaginatedList<RoleEntity>> GetRolesAsync(string factTypeName, RoleParameters roleParameters, CancellationToken cancellationToken = default);
        Task<RoleEntity> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<RoleEntity> AddRoleAsync(RoleEntity role, CancellationToken cancellationToken = default);
        Task<RoleEntity> UpdateRoleAsync(RoleEntity role, CancellationToken cancellationToken = default);
        #endregion

        #region External Facts
        Task<IPaginatedList<ExternalFactConfigEntity>> GetExternalFactConfigsAsync(ExternalFactParameters externalFactParameters, CancellationToken cancellationToken = default);
        Task<ExternalFactConfigEntity> GetExternalFactConfigAsync(string factType, string key, CancellationToken cancellationToken = default);
        Task<ExternalFactConfigEntity> AddExternalFactConfigAsync(ExternalFactConfigEntity externalFactConfig, CancellationToken cancellationToken = default);
        Task<ExternalFactConfigEntity> UpdateExternalFactConfigAsync(ExternalFactConfigEntity externalFactConfig, CancellationToken cancellationToken = default);
        Task DeleteExternalFactConfigAsync(ExternalFactConfigEntity externalFactConfig, CancellationToken cancellationToken = default);
        #endregion
    }
}