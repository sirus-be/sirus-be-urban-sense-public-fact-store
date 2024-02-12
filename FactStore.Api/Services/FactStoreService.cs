using Core.Collections;
using FactStore.Api.Infrastructure.DataStores;
using FactStore.Api.Models;
using FactStore.Models;
using FactStore.Models.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FactStore.Api.Services
{
    public class FactStoreService : IFactStoreService
    {
        private readonly IFactStore _factStore;

        public FactStoreService(IFactStore factStore)
        {
            _factStore = factStore ?? throw new ArgumentNullException(nameof(factStore));
        }

        #region Facts
        public async Task<IPaginatedList<Fact>> GetAllFactsAsync(FactParameters factParameters)
        {
            var items = await _factStore.GetFactsAsync(factParameters);
            if (items == null)
            {
                return null;
            }

            var itemsViewModel = items.Select(x => x.ToViewModel());

            return itemsViewModel;
        }

        public async Task<IPaginatedList<Fact>> GetAllFactsAsync(string factTypeName, FactParameters factParameters)
        {
            var factType = await _factStore.GetFactTypeByNameAsync(factTypeName);
            if (factType == null)
            {
                throw new ValidationException("FactType does not exists");
            }

            var items = await _factStore.GetFactsAsync(factTypeName, factParameters);
            var itemsViewModel = items.Select(x => x.ToViewModel());

            return itemsViewModel;
        }

        public async Task<Fact> GetFactAsync(string factTypeName, string key)
        {
            var factType = await _factStore.GetFactTypeByNameAsync(factTypeName);
            if (factType == null)
            {
                throw new ValidationException("FactType does not exists");
            }

            var item = await _factStore.GetFactAsync(factTypeName, key);
            if (item == null)
            {
                return null;
            }

            return item.ToViewModel();
        }

        public async Task<Fact> CreateFactAsync(Fact fact)
        {
            var factType = await _factStore.GetFactTypeByNameAsync(fact.FactTypeName);
            if(factType == null)
            {
                throw new ValidationException($"FactType with name {fact.FactTypeName} does not exists");
            }

            var entity = new FactEntity();
            entity.Key = fact.Key;
            entity.Value = fact.Value;
            entity.Description = fact.Description;

            entity.FactTypeId = factType.Id;

            var savedentity = await _factStore.AddFactAsync(entity);

            return savedentity.ToViewModel();
        }

        public async Task<Fact> UpdateFactAsync(UpdateFact fact)
        {
            var factType = await _factStore.GetFactTypeByNameAsync(fact.NewFactTypeName);
            if (factType == null)
            {
                throw new ValidationException($"FactType with name {fact.NewFactTypeName} does not exists");
            }

            var entity = await _factStore.GetFactAsync(fact.PreviousFactTypeName, fact.PreviousKey);
            if (entity == null)
            {
                throw new ValidationException($"Fact with factType {fact.PreviousFactTypeName} and key {fact.PreviousKey} does not exists");
            }

            if (fact.NewKey != fact.PreviousKey)
            {
                var newEntity = await _factStore.GetFactAsync(fact.NewFactTypeName, fact.NewKey);
                if (newEntity != null)
                {
                    throw new ValidationException($"Fact with factType {fact.NewFactTypeName} and key {fact.NewKey} already exists");
                }
            }
            
            entity.Key = fact.NewKey;
            entity.Value = fact.Value;
            entity.Description = fact.Description;

            entity.FactTypeId = factType.Id;

            var savedentity = await _factStore.UpdateFactAsync(entity);

            return savedentity.ToViewModel();
        }

        public async Task DeleteFactAsync(string factTypeName, string key)
        {
            var entity = await _factStore.GetFactAsync(factTypeName, key);
            if (entity != null)
            {
                await _factStore.DeleteFactAsync(entity);
            }
        }
        #endregion

        #region FactTypes
        public async Task<IPaginatedList<FactType>> GetAllFactTypesAsync(FactTypeParameters factTypeParameters)
        {
            var items = await _factStore.GetFactTypesAsync(factTypeParameters);
            var itemsViewModel = items.Select(x => x.ToViewModel());

            return itemsViewModel;
        }

        public async Task<FactType> GetFactTypeByNameAsync(string name)
        {
            var item = await _factStore.GetFactTypeByNameAsync(name);
            if (item == null)
            {
                return null;
            }

            return item.ToViewModel();
        }

        public async Task<FactType> CreateFactTypeAsync(FactType factType)
        {
            var entity = new FactTypeEntity();
            entity.Name = factType.Name;
            entity.Description = factType.Description;

            foreach (var roleName in factType.Roles)
            {
                var role = await _factStore.GetRoleByNameAsync(roleName);
                if (role == null)
                {
                    throw new ValidationException($"Role {roleName} does not exists");
                }

                entity.Roles.Add(role);
            }

            var savedentity = await _factStore.AddFactTypeAsync(entity);

            return savedentity.ToViewModel();
        }

        public async Task<FactType> UpdateFactTypeAsync(UpdateFactType factType)
        {
            var entity = await _factStore.GetFactTypeByNameAsync(factType.PreviousName);
            if (entity == null)
            {
                throw new ValidationException($"FactType with name {factType.PreviousName} does not exists");
            }

            if (factType.NewName != factType.PreviousName)
            {
                var newEntity = await _factStore.GetFactTypeByNameAsync(factType.NewName);
                if (newEntity != null)
                {
                    throw new ValidationException($"FactType with new name {factType.NewName} already exists");
                }
            }
                
            entity.Roles = new List<RoleEntity>();
            
            foreach (var roleName in factType.Roles)
            {
                var role = await _factStore.GetRoleByNameAsync(roleName);
                if (role == null)
                {
                    throw new ValidationException($"Role {roleName} does not exists");
                }

                entity.Roles.Add(role);
            }

            entity.Name = factType.NewName;
            entity.Description = factType.Description;

            var savedentity = await _factStore.UpdateFactTypeAsync(entity);

            return savedentity.ToViewModel();
        }

        #endregion

        #region Roles
        public async Task<IPaginatedList<Role>> GetAllRolesAsync(RoleParameters roleParameters)
        {
            var items = await _factStore.GetRolesAsync(roleParameters);
            var itemsViewModel = items.Select(x => x.ToViewModel());

            return itemsViewModel;
        }

        public async Task<IPaginatedList<Role>> GetAllRolesAsync(string factTypeName, RoleParameters roleParameters)
        {
            var factType = await _factStore.GetFactTypeByNameAsync(factTypeName);
            if (factType == null)
            {
                throw new ValidationException("FactType does not exists");
            }

            var items = await _factStore.GetRolesAsync(factTypeName, roleParameters);
            var itemsViewModel = items.Select(x => x.ToViewModel());

            return itemsViewModel;
        }

        public async Task<Role> GetRoleByNameAsync(string name)
        {
            var item = await _factStore.GetRoleByNameAsync(name);
            if (item == null)
            {
                return null;
            }

            return item.ToViewModel();
        }

        public async Task<Role> CreateRoleAsync(Role role)
        {
            var entity = new RoleEntity();
            entity.Name = role.Name;
            entity.Description = role.Description;

            var savedentity = await _factStore.AddRoleAsync(entity);

            return savedentity.ToViewModel();
        }

        public async Task<Role> UpdateRoleAsync(UpdateRole role)
        {
            var entity = await _factStore.GetRoleByNameAsync(role.PreviousName);
            if (entity == null)
            {
                throw new ValidationException($"Role {role.PreviousName} does not exists");
            }

            if (role.PreviousName != role.NewName)
            {
                var newEntity = await _factStore.GetRoleByNameAsync(role.NewName);
                if (newEntity != null)
                {
                    throw new ValidationException($"Role with new name {role.NewName} already exists");
                }
            }
            
            entity.Name = role.NewName;
            entity.Description = role.Description;

            var savedentity = await _factStore.UpdateRoleAsync(entity);

            return savedentity.ToViewModel();
        }

        public async Task DeleteFactTypeAsync(string factTypeName)
        {
            var facts = await GetAllFactsAsync(factTypeName, new FactParameters());
            foreach (var fact in facts)
            {
                var factEntity = await _factStore.GetFactAsync(factTypeName, fact.Key);
                if (factEntity != null)
                {
                    await _factStore.DeleteFactAsync(factEntity);
                }
            }

            var entity = await _factStore.GetFactTypeByNameAsync(factTypeName);
            if (entity != null)
            {
                await _factStore.DeleteFactTypeAsync(entity);
            }
        }
        #endregion

        #region External Facts
        public async Task<IPaginatedList<ExternalFactConfig>> GetAllExternalFactConfigsAsync(ExternalFactParameters externalFactParameters)
        {
            var items = await _factStore.GetExternalFactConfigsAsync(externalFactParameters);
            var itemsViewModel = items.Select(x => x.ToViewModel());

            return itemsViewModel;
        }

        public async Task<ExternalFactConfig> GetExternalFactConfigAsync(string factTypeName, string key)
        {
            var factType = await _factStore.GetFactTypeByNameAsync(factTypeName);
            if (factType == null)
            {
                throw new ValidationException("FactType does not exists");
            }

            var item = await _factStore.GetExternalFactConfigAsync(factTypeName, key);
            if (item == null)
            {
                return null;
            }

            return item.ToViewModel();
        }

        public async Task<ExternalFactConfig> CreateExternalFactConfigAsync(ExternalFactConfig externalFactConfig)
        {
            var fact = await _factStore.GetFactAsync(externalFactConfig.FactTypeName, externalFactConfig.Key);
            if (fact == null)
            {
                throw new ValidationException($"Fact with key {externalFactConfig.Key} and FactType {externalFactConfig.FactTypeName} does not exists");
            }

            var entity = new ExternalFactConfigEntity();

            entity.FactId = fact.Id; 
            entity.Url = externalFactConfig.Url;
            entity.Authentication = externalFactConfig.Authentication;
            entity.AuthenitcationUrl = externalFactConfig.AuthenticationUrl;
            entity.ClientId = externalFactConfig.ClientId;
            entity.Secret = externalFactConfig.Secret;
            entity.CronScheduleExpression = externalFactConfig.CronScheduleExpression;
            entity.TokenAuthorizationHeader = externalFactConfig.TokenAuthorizationHeader;
            entity.Description = externalFactConfig.Description;

            var savedentity = await _factStore.AddExternalFactConfigAsync(entity);

            return savedentity.ToViewModel();
        }

        public async Task<ExternalFactConfig> UpdateExternalFactConfigAsync(UpdateExternalFactConfig externalFactConfig)
        {
            var fact = await _factStore.GetFactAsync(externalFactConfig.NewFactTypeName, externalFactConfig.NewKey);
            if (fact == null)
            {
                throw new ValidationException($"Fact with key {externalFactConfig.NewKey} and FactType {externalFactConfig.NewFactTypeName} does not exists");
            }


            var entity = await _factStore.GetExternalFactConfigAsync(externalFactConfig.PreviousFactTypeName, externalFactConfig.PreviousKey);
            if (entity == null)
            {
                throw new ValidationException($"ExternalFactConfig does not exists");
            }

            if (externalFactConfig.NewKey != externalFactConfig.PreviousKey)
            {
                var newEntity = await _factStore.GetExternalFactConfigAsync(externalFactConfig.NewFactTypeName, externalFactConfig.NewKey);
                if (newEntity != null)
                {
                    throw new ValidationException($"Fact with factType {externalFactConfig.NewFactTypeName} and key {externalFactConfig.NewKey} already exists");
                }
            }

            entity.FactId = fact.Id;
            entity.Url = externalFactConfig.Url;
            entity.Authentication = externalFactConfig.Authentication;
            entity.AuthenitcationUrl = externalFactConfig.AuthenticationUrl;
            entity.ClientId = externalFactConfig.ClientId;
            entity.Secret = externalFactConfig.Secret;
            entity.CronScheduleExpression = externalFactConfig.CronScheduleExpression;
            entity.TokenAuthorizationHeader = externalFactConfig.TokenAuthorizationHeader;
            entity.Description = externalFactConfig.Description;

            var savedentity = await _factStore.UpdateExternalFactConfigAsync(entity);

            return savedentity.ToViewModel();
        }

        public async Task DeleteExternalFactConfigAsync(string factType, string key)
        {
            var entity = await _factStore.GetExternalFactConfigAsync(factType, key);
            if (entity != null)
            {
                await _factStore.DeleteExternalFactConfigAsync(entity);
            }
        }
        #endregion

    }
}
