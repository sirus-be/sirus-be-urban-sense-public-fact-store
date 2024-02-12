using System.Collections.Generic;

namespace FactStore.Models
{
    public class UpdateRole
    {
        public string PreviousName { get; set; }
        public string NewName { get; set; }
        public string Description { get; set; }
    }
}
