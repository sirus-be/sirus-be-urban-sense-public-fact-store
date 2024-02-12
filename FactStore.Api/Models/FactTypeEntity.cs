using Core.Models;
using FactStore.Models;
using System.Collections.Generic;
using System.Linq;

namespace FactStore.Api.Models
{
    public class FactTypeEntity : AuditableEntity
    {
        public string Name { get; set; }
        public List<RoleEntity> Roles { get; set; } = new List<RoleEntity>();
        public List<FactEntity> Facts { get; set; } = new List<FactEntity>();

        public FactType ToViewModel()
        {
            return new FactType
            {
                Name = Name,
                Roles = Roles.Select(r=> r.Name).ToList(),
                Description = Description
            };
        }
    }
}
