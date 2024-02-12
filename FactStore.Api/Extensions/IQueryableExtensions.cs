using Core.Collections;
using FactStore.Api.Models;
using FactStore.Models.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FactStore.Api.Extensions
{
    public static class IQueryableExtensions
    {
        public static async Task<IPaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> collection, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }

            var totalItems = await collection.CountAsync(cancellationToken);
            if (pageSize > 0)
            {
                var items = await collection.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(cancellationToken);
                return new PaginatedList<T>(pageIndex, pageSize, totalItems, items);
            }
            else
            {
                var items = await collection.ToListAsync(cancellationToken);
                return new PaginatedList<T>(0, totalItems, totalItems, items);
            }
        }

        public static IQueryable<FactEntity> FilterOnRoles (this IQueryable<FactEntity> query, ClaimsPrincipal currentUser)
        {
            var userRoles = currentUser.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!userRoles.Contains(Roles.SuperAdmin))
            {
                query = query.Where(fact => fact.FactType.Roles.Any(factTypeRole => userRoles.Any(userRole => factTypeRole.Name == userRole)));
            }

            return query;
        }

        public static IQueryable<FactTypeEntity> FilterOnRoles(this IQueryable<FactTypeEntity> query, ClaimsPrincipal currentUser)
        {
            var userRoles = currentUser.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!userRoles.Contains(Roles.SuperAdmin))
            {
                query = query.Where(factType => factType.Roles.Any(factTypeRole => userRoles.Any(userRole => factTypeRole.Name == userRole)));
            }

            return query;
        }

        public static IQueryable<RoleEntity> FilterOnRoles(this IQueryable<RoleEntity> query, ClaimsPrincipal currentUser)
        {
            var userRoles = currentUser.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!userRoles.Contains(Roles.SuperAdmin))
            {
                query = query.Where(role => userRoles.Any(userRole => role.Name == userRole));
            }

            return query;
        }

        public static IQueryable<ExternalFactConfigEntity> FilterOnRoles(this IQueryable<ExternalFactConfigEntity> query, ClaimsPrincipal currentUser)
        {
            var userRoles = currentUser.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!userRoles.Contains(Roles.SuperAdmin))
            {
                query = query.Where(fact => fact.Fact.FactType.Roles.Any(factTypeRole => userRoles.Any(userRole => factTypeRole.Name == userRole)));
            }

            return query;
        }
    }
}
