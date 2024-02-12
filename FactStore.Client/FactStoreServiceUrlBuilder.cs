using FactStore.Models.Parameters;
using System.Text;

namespace FactStore.Client
{
    public static class FactStoreServiceUrlBuilder
    {
        //public const string FactTypeBaseUri = "factstore/api/v1/FactType"; Uri for SirusCityStore AKS cluster
        //public const string RoleBaseUri = "factstore/api/v1/Role"; Uri for SirusCityStore AKS cluster
        public const string FactTypeBaseUri = "api/v1/FactType";
        public const string RoleBaseUri = "api/v1/Role";
        public const string FactBaseUri = "api/v1/Fact";
        public const string ExternalFactConfigBaseUri = "api/v1/ExternalFactConfig";

        #region Fact

        public static string GetFactsUrl()
        {
            return FactBaseUri;
        }

        public static string GetFactsUrl(FactParameters parameters)
        {
            return BuildUrl(GetFactsUrl()+"?", parameters);
        }

        public static string GetFactUrl(string factTypeName,string key)
        {
            var sbUrl = new StringBuilder();
            return sbUrl.Append($"{GetFactsUrl()}/FactType/{factTypeName}/Facts/{key}").ToString();
        }

        public static string GetFactsUrl(string factTypeName, FactParameters parameters)
        {
            var url = $"{GetFactsUrl()}/FactType/{factTypeName}?";
            return BuildUrl(url, parameters);
        }

        private static string BuildUrl(string url, FactParameters parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                url += $"search={parameters.Search}&";
            }
            if (!string.IsNullOrWhiteSpace(parameters.Sorting))
            {
                url += $"sorting={parameters.Sorting}&";
            }
            url += $"pageIndex={parameters.PageIndex}&pageSize={parameters.PageSize}";
            return url;
        }

        #endregion

        #region Fact Types
        public static string GetFactTypesUrl()
        {
            return FactTypeBaseUri;
        }

        public static string GetFactTypesUrl(FactTypeParameters parameters)
        {
            return BuildUrl(GetFactTypesUrl()+"?", parameters);
        }

        public static string GetFactTypeUrl(string factTypeName)
        {
            var sbUrl = new StringBuilder();
            return sbUrl.Append($"{GetFactTypesUrl()}/{factTypeName}").ToString();
        }

        private static string BuildUrl(string url, FactTypeParameters parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                url += $"search={parameters.Search}&";
            }
            if (!string.IsNullOrWhiteSpace(parameters.Sorting))
            {
                url += $"sorting={parameters.Sorting}&";
            }
            url += $"pageIndex={parameters.PageIndex}&pageSize={parameters.PageSize}";
            return url;
        }
        #endregion

        #region Roles
        public static string GetRolesUrl()
        {
            return RoleBaseUri;
        }

        public static string GetRolesUrl(RoleParameters parameters)
        {
            return BuildUrl(GetRolesUrl()+"?", parameters);
        }

        public static string GetFactTypeRolesUrl(RoleParameters parameters, string factTypeName)
        {
            var url = $"{GetRolesUrl()}/FactType/{factTypeName}?";
            return BuildUrl(url, parameters);
        }

        public static string GetRoleUrl(string roleName)
        {
            var sbUrl = new StringBuilder();
            return sbUrl.Append($"{GetRolesUrl()}/{roleName}").ToString();
        }

        private static string BuildUrl(string url, RoleParameters parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                url += $"search={parameters.Search}&";
            }
            if (!string.IsNullOrWhiteSpace(parameters.Sorting))
            {
                url += $"sorting={parameters.Sorting}&";
            }
            url += $"pageIndex={parameters.PageIndex}&pageSize={parameters.PageSize}";
            return url;
        }

        #endregion

        #region External Facts

        public static string GetExternalFactsUrl()
        {
            return ExternalFactConfigBaseUri;
        }

        public static string GetExternalFactUpdateFactValueUrl()
        {
            var sbUrl = new StringBuilder();
            return sbUrl.Append($"{GetExternalFactsUrl()}/UpdateFactValue").ToString();
        }

        public static string GetExternalFactsUrl(ExternalFactParameters parameters)
        {
            return BuildUrl(GetExternalFactsUrl()+"?", parameters);
        }

        public static string GetExternalFactsUrl(string factTypeName, string factKey)
        {
            var sbUrl = new StringBuilder();
            return sbUrl.Append($"{GetExternalFactsUrl()}/FactType/{factTypeName}/Facts/{factKey}").ToString();
        }

        private static string BuildUrl(string url, ExternalFactParameters parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                url += $"search={parameters.Search}&";
            }
            if (!string.IsNullOrWhiteSpace(parameters.Sorting))
            {
                url += $"sorting={parameters.Sorting}&";
            }
            url += $"pageIndex={parameters.PageIndex}&pageSize={parameters.PageSize}";
            return url;
        }
        #endregion

    }
}
