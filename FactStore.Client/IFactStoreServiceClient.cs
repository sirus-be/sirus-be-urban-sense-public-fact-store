using Core.Collections;
using FactStore.Models;
using FactStore.Models.Parameters;
using System.Threading.Tasks;

namespace FactStore.Client
{
    public interface IFactStoreServiceClient
    {
        #region Facts
        Task<PaginatedList<Fact>> GetAllFactsAsync(FactParameters parameters);
        Task<PaginatedList<Fact>> GetFactsByFactTypeNameAsync(string factTypeName, FactParameters parameters);
        Task<Fact> GetFactAsync(string factTypeName, string key);
        Task<UpdateFact> PutFactAsync(UpdateFact fact);
        Task<Fact> PostFactAsync(Fact fact);
        Task DeleteFactAsync(string factTypeId, string key);
        #endregion

        #region Fact types
        Task<PaginatedList<FactType>> GetFactTypesAsync(FactTypeParameters parameters);
        Task<FactType> GetFactTypeAsync(string factTypeName);
        Task<UpdateFactType> PutFactTypeAsync(UpdateFactType factType);
        Task<FactType> PostFactTypeAsync(FactType factType);
        Task DeleteFactTypeAsync(string factTypeId);
        #endregion

        #region Roles
        Task<PaginatedList<Role>> GetRolesAsync(RoleParameters parameters);
        Task<PaginatedList<Role>> GetFactTypesRolesAsync(RoleParameters parameters, string factTypeName);
        Task<Role> PostRoleAsync(Role role);
        Task<UpdateRole> PutRoleAsync(UpdateRole role);
        Task<Role> GetRoleAsync(string roleName);
        #endregion

        #region External Facts
        Task<PaginatedList<ExternalFactConfig>> GetAllExternalFactsAsync(ExternalFactParameters parameters);
        Task<ExternalFactConfig> GetExternalFactAscyn(string factTypeName, string factKey);
        Task<ExternalFactConfig> PostExternalFactAsync(ExternalFactConfig externalFact);
        Task<UpdateExternalFactConfig> PutExternalFactAsync(UpdateExternalFactConfig updateExternalFact);
        Task<ExternalFactConfig> PutExternalFactUpdateFactValueAsync(ExternalFactConfig updateExternalFact);
        Task DeleteExternalFactAsync(string factTypeName, string factKey);
        #endregion

    }
}