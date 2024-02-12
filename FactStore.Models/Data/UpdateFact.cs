namespace FactStore.Models
{
    public class UpdateFact
    {
        public string PreviousKey { get; set; }
        public string NewKey { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string PreviousFactTypeName { get; set; }
        public string NewFactTypeName { get; set; }
    }
}
