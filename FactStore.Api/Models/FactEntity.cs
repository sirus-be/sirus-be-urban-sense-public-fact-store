using Core.Models;
using FactStore.Models;
using System.Collections.Generic;

namespace FactStore.Api.Models
{
    public class FactEntity : AuditableEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int FactTypeId { get; set; }
        public FactTypeEntity FactType { get; set; }
        public List<ExternalFactConfigEntity> ExternalFactConfigs { get; set; } = new List<ExternalFactConfigEntity>();

        public Fact ToViewModel()
        {
            return new Fact
            {
                Key = Key,
                Value = Value,
                FactTypeName = FactType.Name,
                Description = Description
            };
        }
    }
}
