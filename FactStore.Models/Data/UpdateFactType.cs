using System.Collections.Generic;

namespace FactStore.Models
{
    public class UpdateFactType
    {
        public string PreviousName { get; set; }
        public string NewName { get; set; }
        public string Description { get; set; }
        public List<string> Roles { get; set; }
    }
}
