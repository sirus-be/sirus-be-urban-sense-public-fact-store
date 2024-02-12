using Core.Collections;
using FactStore.Models;
using FactStore.Models.Parameters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FactStore.Api.Services
{
    public interface IFactStoreService
    {
        #region Fact
        Task<IPaginatedList<Fact>> GetAllFactsAsync(FactParameters factParameters);
        Task<IPaginatedList<Fact>> GetAllFactsAsync(string factType, FactParameters factParameters);
        Task<Fact> GetFactAsync(string factType, string key);
        Task<Fact> CreateFactAsync(Fact fact);
        Task<Fact> UpdateFactAsync(UpdateFact fact);
        Task DeleteFactAsync(string factTypeName, string key);
        #endregion

        #region Fact Type
        Task<IPaginatedList<FactType>> GetAllFactTypesAsync(FactTypeParameters factTypeParameters);
        Task<FactType> GetFactTypeByNameAsync(string name);
        Task<FactType> CreateFactTypeAsync(FactType factType);
        Task<FactType> UpdateFactTypeAsync(UpdateFactType factType);
        Task DeleteFactTypeAsync(string factTypeName);
        #endregion

        #region Role
        Task<IPaginatedList<Role>> GetAllRolesAsync(RoleParameters roleParameters);
        Task<IPaginatedList<Role>> GetAllRolesAsync(string factTypeName, RoleParameters roleParameters);
        Task<Role> GetRoleByNameAsync(string name);
        Task<Role> CreateRoleAsync(Role role);
        Task<Role> UpdateRoleAsync(UpdateRole role);
        #endregion

        #region External Fact
        Task<IPaginatedList<ExternalFactConfig>> GetAllExternalFactConfigsAsync(ExternalFactParameters externalFactParameters);
        Task<ExternalFactConfig> GetExternalFactConfigAsync(string factTypeName, string key);
        Task<ExternalFactConfig> CreateExternalFactConfigAsync(ExternalFactConfig externalFactConfig);
        Task<ExternalFactConfig> UpdateExternalFactConfigAsync(UpdateExternalFactConfig externalFactConfig);
        Task DeleteExternalFactConfigAsync(string factType, string key);
        #endregion
    }
}