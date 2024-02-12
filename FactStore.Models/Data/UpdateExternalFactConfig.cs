namespace FactStore.Models
{
    public class UpdateExternalFactConfig
    {
        public string PreviousFactTypeName { get; set; }
        public string NewFactTypeName { get; set; }
        public string PreviousKey { get; set; }
        public string NewKey { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public bool Authentication { get; set; }
        public string TokenAuthorizationHeader { get; set; }
        public string AuthenticationUrl { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string CronScheduleExpression { get; set; }
    }
}
