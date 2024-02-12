namespace FactStore.Models
{
    public class ExternalFactConfig
    {       
        public string FactTypeName { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public bool Authentication { get; set; }
        public string TokenAuthorizationHeader { get; set; }
        public string CronScheduleExpression { get; set; }
        public string AuthenticationUrl { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
    }
}
