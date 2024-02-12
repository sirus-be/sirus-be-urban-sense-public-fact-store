using System.Collections.Generic;

namespace FactStore.Models
{
    public class FactType
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Roles { get; set; }
    }
}
