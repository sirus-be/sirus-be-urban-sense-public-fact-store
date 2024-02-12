using Core.Models;
using FactStore.Models;
using System.Collections.Generic;

namespace FactStore.Api.Models
{
    public class RoleEntity : AuditableEntity
    {
        public string Name { get; set; }
        public List<FactTypeEntity> FactTypes { get; set; } = new List<FactTypeEntity>();

        public Role ToViewModel()
        {
            return new Role
            {
                Name = Name,
                Description = Description
            };
        }
    }
}
