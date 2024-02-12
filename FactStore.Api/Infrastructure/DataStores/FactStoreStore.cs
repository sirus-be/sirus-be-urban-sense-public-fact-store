using Core.Collections;
using FactStore.Api.Extensions;
using FactStore.Api.Infrastructure.Database;
using FactStore.Api.Models;
using FactStore.Models.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace FactStore.Api.Infrastructure.DataStores
{
    public class FactStoreStore : IFactStore
    {
        private readonly ClaimsPrincipal _currentUser;
        private readonly string _userName;
        private readonly FactStoreDbContext _dbContext;
        private readonly IAuthorizationService _authorizationService;

        public FactStoreStore(FactStoreDbContext dbContext, IHttpContextAccessor httpContextAccessor, IAuthorizationService authorizationService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _currentUser = httpContextAccessor.HttpContext.User;
            _userName = httpContextAccessor.HttpContext.User.Claims
                        .Where(c => c.Type.Equals("preferred_username"))
                        .Select(c => c.Value)
                        .FirstOrDefault() ?? string.Empty;
        }

        #region Facts
        public async Task<IPaginatedList<FactEntity>> GetFactsAsync(FactParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Facts.Include(f => f.FactType).Where(i => !i.IsDeleted).FilterOnRoles(_currentUser);

            if (parameters != null)
            {
                query = FilterQuery(query, parameters);
            }

            return await query.ToPaginatedListAsync(parameters.PageIndex, parameters.PageSize, cancellationToken);
        }

        public async Task<IPaginatedList<FactEntity>> GetFactsAsync(string factType, FactParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Facts.Include(f => f.FactType).Where(i => i.FactType.Name.ToLower() == factType.ToLower() && !i.IsDeleted);

            if (parameters != null)
            {
                query = FilterQuery(query, parameters);
            }

            return await query.ToPaginatedListAsync(parameters.PageIndex, parameters.PageSize, cancellationToken);
        }       

        public async Task<FactEntity> GetFactAsync(string factTypeName, string key, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Facts.Include(f => f.FactType).AsQueryable();

            return await query.FirstOrDefaultAsync(i => i.Key.ToLower() == key.ToLower() && i.FactType.Name.ToLower() == factTypeName.ToLower() && !i.IsDeleted, cancellationToken);
        }

        public async Task<FactEntity> AddFactAsync(FactEntity fact, CancellationToken cancellationToken = default)
        {
            fact.CreatedOn = DateTime.Now;
            fact.CreatedBy = _userName;
            fact.LastModifiedBy = null;
            fact.LastModifiedOn = null;

            var entityEntry = _dbContext.Facts.Add(fact);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entityEntry.Entity;
        }

        public async Task<FactEntity> UpdateFactAsync(FactEntity fact, CancellationToken cancellationToken = default)
        {
            var entityEntry = GetChangeTrackedEntity(fact);
            entityEntry.Entity.LastModifiedBy = _userName;
            entityEntry.Entity.LastModifiedOn = DateTime.UtcNow;

            entityEntry.State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entityEntry.Entity;
        }

        public async Task DeleteFactAsync(FactEntity fact, CancellationToken cancellationToken = default)
        {
            var entityEntry = GetChangeTrackedEntity(fact);
            entityEntry.Entity.IsDeleted = true;
            entityEntry.Entity.LastModifiedBy = _userName;
            entityEntry.Entity.LastModifiedOn = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private IQueryable<FactEntity> FilterQuery(IQueryable<FactEntity> query, FactParameters parameters)
        {
            if (parameters.Search != null)
            {
                query = query.Where(i => i.Key.ToLower().Contains(parameters.Search.ToLower()) || i.Value.ToLower().Contains(parameters.Search.ToLower()) || i.Description.ToLower().Contains(parameters.Search.ToLower()) || i.FactType.Name.ToLower().Contains(parameters.Search.ToLower()));
            }

            if (!string.IsNullOrEmpty(parameters.Sorting))
            {
                switch (parameters.Sorting)
                {
                    case FactOrder.FactTypeNameAscending:
                        query = query.OrderBy(x => x.FactType.Name);
                        break;
                    case FactOrder.FactTypeNameDescending:
                        query = query.OrderByDescending(x => x.FactType.Name);
                        break;                  
                    default:
                        query = query.OrderBy(parameters.Sorting);
                        break;
                }
            }

            return query;
        }

        #endregion

        #region Fact Types
        public async Task<IPaginatedList<FactTypeEntity>> GetFactTypesAsync(FactTypeParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.FactTypes.Where(i => !i.IsDeleted).FilterOnRoles(_currentUser);

            if (parameters != null)
            {
                query = FilterQuery(query, parameters);
            }

            return await query.ToPaginatedListAsync(parameters.PageIndex, parameters.PageSize, cancellationToken);
        }

        public async Task<FactTypeEntity> GetFactTypeByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.FactTypes.Include(f=>f.Roles).AsQueryable();
            var factType = await query.FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower() && !i.IsDeleted, cancellationToken);

            return await AuthorizeRoles(factType);
        }

        public async Task<FactTypeEntity> AddFactTypeAsync(FactTypeEntity factType, CancellationToken cancellationToken = default)
        {
            factType.CreatedOn = DateTime.Now;
            factType.CreatedBy = _userName;
            factType.LastModifiedBy = null;
            factType.LastModifiedOn = null;

            var entityEntry = _dbContext.FactTypes.Add(factType);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entityEntry.Entity;
        }

        public async Task<FactTypeEntity> UpdateFactTypeAsync(FactTypeEntity factType, CancellationToken cancellationToken = default)
        {
            var entityEntry = GetChangeTrackedEntity(factType);
            entityEntry.Entity.LastModifiedBy = _userName;
            entityEntry.Entity.LastModifiedOn = DateTime.UtcNow;

            entityEntry.State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entityEntry.Entity;
        }

        public async Task DeleteFactTypeAsync(FactTypeEntity factType, CancellationToken cancellationToken = default)
        {
            var entityEntry = GetChangeTrackedEntity(factType);
            entityEntry.Entity.IsDeleted = true;
            entityEntry.Entity.LastModifiedBy = _userName;
            entityEntry.Entity.LastModifiedOn = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private IQueryable<FactTypeEntity> FilterQuery(IQueryable<FactTypeEntity> query, FactTypeParameters parameters)
        {
            if (parameters.Search != null)
            {
                query = query.Where(i => i.Name.ToLower().Contains(parameters.Search.ToLower()) || i.Description.ToLower().Contains(parameters.Search.ToLower()));
            }

            if (!string.IsNullOrEmpty(parameters.Sorting))
            {
                query = query.OrderBy(parameters.Sorting);
            }

            return query;
        }
        #endregion

        #region Roles
        public async Task<IPaginatedList<RoleEntity>> GetRolesAsync(RoleParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Roles.Where(i => !i.IsDeleted).FilterOnRoles(_currentUser);
            
            if (parameters != null)
            {
                query = FilterQuery(query, parameters);
            }

            return await query.ToPaginatedListAsync(parameters.PageIndex, parameters.PageSize, cancellationToken);
        }

        public async Task<IPaginatedList<RoleEntity>> GetRolesAsync(string factTypeName, RoleParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Roles.Include(f => f.FactTypes).Where(i => i.FactTypes.Any(factType => factType.Name.ToLower() == factTypeName.ToLower()) && !i.IsDeleted);

            if (parameters != null)
            {
                query = FilterQuery(query, parameters);
            }

            return await query.ToPaginatedListAsync(parameters.PageIndex, parameters.PageSize, cancellationToken);
        }

        public async Task<RoleEntity> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            await AuthorizeRoles(new List<string> { name});

            var query = _dbContext.Roles.AsQueryable();

            return await query.FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower() && !i.IsDeleted, cancellationToken);
        }

        public async Task<RoleEntity> AddRoleAsync(RoleEntity role, CancellationToken cancellationToken = default)
        {
            role.CreatedOn = DateTime.Now;
            role.CreatedBy = _userName;
            role.LastModifiedBy = null;
            role.LastModifiedOn = null;

            var entityEntry = _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entityEntry.Entity;
        }

        public async Task<RoleEntity> UpdateRoleAsync(RoleEntity role, CancellationToken cancellationToken = default)
        {
            var entityEntry = GetChangeTrackedEntity(role);
            entityEntry.Entity.LastModifiedBy = _userName;
            entityEntry.Entity.LastModifiedOn = DateTime.UtcNow;

            entityEntry.State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entityEntry.Entity;
        }

        private IQueryable<RoleEntity> FilterQuery(IQueryable<RoleEntity> query, RoleParameters parameters)
        {
            if (parameters.Search != null)
            {
                query = query.Where(i => i.Name.ToLower().Contains(parameters.Search.ToLower()) || i.Description.ToLower().Contains(parameters.Search.ToLower()));
            }

            if (!string.IsNullOrEmpty(parameters.Sorting))
            {
                query = query.OrderBy(parameters.Sorting);
            }

            return query;
        }
        #endregion

        #region External Facts
        public async Task<IPaginatedList<ExternalFactConfigEntity>> GetExternalFactConfigsAsync(ExternalFactParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ExternalFactConfigs.Include(f => f.Fact).ThenInclude(f=>f.FactType).ThenInclude(ft=>ft.Roles).Where(i => !i.IsDeleted).FilterOnRoles(_currentUser);

            if (parameters != null)
            {
                query = FilterQuery(query, parameters);
            }

            return await query.ToPaginatedListAsync(parameters.PageIndex, parameters.PageSize, cancellationToken);
        }

        public async Task<ExternalFactConfigEntity> GetExternalFactConfigAsync(string factType, string key, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ExternalFactConfigs.Include(f => f.Fact).ThenInclude(f => f.FactType).AsQueryable();

            return await query.FirstOrDefaultAsync(i => i.Fact.Key.ToLower() == key.ToLower() && i.Fact.FactType.Name.ToLower() == factType.ToLower() && !i.IsDeleted, cancellationToken);
        }

        public async Task<ExternalFactConfigEntity> AddExternalFactConfigAsync(ExternalFactConfigEntity externalFactConfig, CancellationToken cancellationToken = default)
        {
            externalFactConfig.CreatedOn = DateTime.Now;
            externalFactConfig.CreatedBy = _userName;
            externalFactConfig.LastModifiedBy = null;
            externalFactConfig.LastModifiedOn = null;

            var entityEntry = _dbContext.ExternalFactConfigs.Add(externalFactConfig);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entityEntry.Entity;
        }

        public async Task<ExternalFactConfigEntity> UpdateExternalFactConfigAsync(ExternalFactConfigEntity externalFactConfig, CancellationToken cancellationToken = default)
        {
            var entityEntry = GetChangeTrackedEntity(externalFactConfig);
            entityEntry.Entity.LastModifiedBy = _userName;
            entityEntry.Entity.LastModifiedOn = DateTime.UtcNow;

            entityEntry.State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entityEntry.Entity;
        }

        public async Task DeleteExternalFactConfigAsync(ExternalFactConfigEntity externalFactConfig, CancellationToken cancellationToken = default)
        {
            var entityEntry = GetChangeTrackedEntity(externalFactConfig);
            entityEntry.Entity.IsDeleted = true;
            entityEntry.Entity.LastModifiedBy = _userName;
            entityEntry.Entity.LastModifiedOn = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private IQueryable<ExternalFactConfigEntity> FilterQuery(IQueryable<ExternalFactConfigEntity> query, ExternalFactParameters parameters)
        {
            if (parameters.Search != null)
            {
                query = query.Where(i => i.Fact.FactType.Name.ToLower().Contains(parameters.Search.ToLower()) || i.Fact.Key.ToLower().Contains(parameters.Search.ToLower()) || i.Description.ToLower().Contains(parameters.Search.ToLower()));
            }

            if (!string.IsNullOrEmpty(parameters.Sorting))
            {
                switch (parameters.Sorting)
                {
                    case ExternalFactOrder.FactTypeNameAscending:
                        query = query.OrderBy(x => x.Fact.FactType.Name);
                        break;
                    case ExternalFactOrder.FactTypeNameDescending:
                        query = query.OrderByDescending(x => x.Fact.FactType.Name);
                        break;
                    case ExternalFactOrder.KeyAscending:
                        query = query.OrderBy(x => x.Fact.Key);
                        break;
                    case ExternalFactOrder.KeyDescending:
                        query = query.OrderByDescending(x => x.Fact.Key);
                        break;
                    default:
                        query = query.OrderBy(parameters.Sorting);
                        break;
                }
            }

            return query;
        }
        #endregion

        private async Task<FactTypeEntity> AuthorizeRoles(FactTypeEntity factType)
        {
            if (factType != null)
            {
                await AuthorizeRoles(factType.Roles.Select(r => r.Name));
            }          

            return factType;
        }

        private async Task AuthorizeRoles(IEnumerable<string> roles)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(_currentUser, roles, "ValidRoleRolesPolicy");
            if (!authorizationResult.Succeeded)
            {
                throw new AuthenticationException($"You don't have the right permissions.");
            }
        }

        private EntityEntry<TEntity> GetChangeTrackedEntity<TEntity>(TEntity item)
                where TEntity : class
        {
            var entityEntry = _dbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(i => i.Entity == item);
            if (entityEntry == null)
            {
                entityEntry = _dbContext.Set<TEntity>().Attach(item);
            }
            return entityEntry;
        }
    }
}
