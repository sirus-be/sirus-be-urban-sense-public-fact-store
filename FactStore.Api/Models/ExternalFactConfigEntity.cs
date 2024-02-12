using Core.Models;
using FactStore.Models;

namespace FactStore.Api.Models
{
    public class ExternalFactConfigEntity : AuditableEntity
    {
        public int FactId { get; set; }
        public FactEntity Fact { get; set; }
        public string Url { get; set; }
        public bool Authentication { get; set; }
        public string TokenAuthorizationHeader { get; set; }
        public string CronScheduleExpression { get; set; }
        public string AuthenitcationUrl { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }

        public ExternalFactConfig ToViewModel()
        {
            return new ExternalFactConfig
            {
                FactTypeName = Fact.FactType.Name,
                Key = Fact.Key,
                Url = Url,
                Authentication = Authentication,
                AuthenticationUrl = AuthenitcationUrl,
                ClientId = ClientId,
                Secret = Secret,
                TokenAuthorizationHeader = TokenAuthorizationHeader,
                CronScheduleExpression = CronScheduleExpression,
                Description = Description
            };
        }
    }
}
