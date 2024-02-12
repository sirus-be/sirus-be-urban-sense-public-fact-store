using Core.Collections;
using FactStore.Models;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Authentication;
using System.Net.Http.Json;
using FactStore.Models.Parameters;
using Core.Authentication;
using System.Net.Http.Headers;

namespace FactStore.Client
{
    public class FactStoreServiceClient : IFactStoreServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly TokenProvider _tokenProvider;

        public FactStoreServiceClient(HttpClient httpClient, TokenProvider tokenProvider)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _tokenProvider = tokenProvider;
        }

        #region Facts

        public async Task<PaginatedList<Fact>> GetAllFactsAsync(FactParameters parameters)
        {
            return await GetAllAsync<PaginatedList<Fact>>(FactStoreServiceUrlBuilder.GetFactsUrl(parameters));
        }

        public async Task<Fact> GetFactAsync(string factTypename, string key)
        {
            return await GetAsync<Fact>(FactStoreServiceUrlBuilder.GetFactUrl(factTypename, key));
        }

        public async Task<PaginatedList<Fact>> GetFactsByFactTypeNameAsync(string factTypeName, FactParameters parameters)
        {
            return await GetAllAsync<PaginatedList<Fact>>(FactStoreServiceUrlBuilder.GetFactsUrl(factTypeName, parameters));
        }

        public async Task<Fact> PostFactAsync(Fact fact)
        {
            return await PostAsync<Fact>(FactStoreServiceUrlBuilder.GetFactsUrl(), fact);
        }

        public async Task<UpdateFact> PutFactAsync(UpdateFact fact)
        {
            return await PutAsync<UpdateFact>(FactStoreServiceUrlBuilder.GetFactsUrl(), fact);
        }

        public async Task DeleteFactAsync(string factTypeId, string key)
        {
            await DeleteAsync<Fact>(FactStoreServiceUrlBuilder.GetFactUrl(factTypeId, key));
        }

        #endregion

        #region Fact Types

        public async Task<PaginatedList<FactType>> GetFactTypesAsync(FactTypeParameters parameters)
        {
            return await GetAllAsync<PaginatedList<FactType>>(FactStoreServiceUrlBuilder.GetFactTypesUrl(parameters));
        }

        public async Task<FactType> GetFactTypeAsync(string factTypeName)
        {
            return await GetAsync<FactType>(FactStoreServiceUrlBuilder.GetFactTypeUrl(factTypeName));
        }

        public async Task<UpdateFactType> PutFactTypeAsync(UpdateFactType factType)
        {
            return await PutAsync<UpdateFactType>(FactStoreServiceUrlBuilder.GetFactTypesUrl(), factType);
        }

        public async Task<FactType> PostFactTypeAsync(FactType factType)
        {
            return await PostAsync<FactType>(FactStoreServiceUrlBuilder.GetFactTypesUrl(), factType);
        }

        public async Task DeleteFactTypeAsync(string factTypeId)
        {
            await DeleteAsync<FactType>(FactStoreServiceUrlBuilder.GetFactTypeUrl(factTypeId));
        }
        #endregion

        #region Roles

        public async Task<PaginatedList<Role>> GetRolesAsync(RoleParameters parameters)
        {
            return await GetAllAsync<PaginatedList<Role>>(FactStoreServiceUrlBuilder.GetRolesUrl(parameters));
        }

        public async Task<PaginatedList<Role>> GetFactTypesRolesAsync(RoleParameters parameters, string factTypeName)
        {
            return await GetAllAsync<PaginatedList<Role>>(FactStoreServiceUrlBuilder.GetFactTypeRolesUrl(parameters, factTypeName));
        }

        public async Task<Role> GetRoleAsync(string roleName)
        {
            return await GetAsync<Role>(FactStoreServiceUrlBuilder.GetRoleUrl(roleName));
        }

        public async Task<Role> PostRoleAsync(Role role)
        {
            return await PostAsync<Role>(FactStoreServiceUrlBuilder.GetRolesUrl(), role);
        }

        public async Task<UpdateRole> PutRoleAsync(UpdateRole role)
        {
            return await PutAsync<UpdateRole>(FactStoreServiceUrlBuilder.GetRolesUrl(), role);
        }
        #endregion

        #region External Facts

        public async Task<PaginatedList<ExternalFactConfig>> GetAllExternalFactsAsync(ExternalFactParameters parameters)
        {
            return await GetAllAsync<PaginatedList<ExternalFactConfig>>(FactStoreServiceUrlBuilder.GetExternalFactsUrl(parameters));
        }

        public async Task<ExternalFactConfig> GetExternalFactAscyn(string factTypeName, string factKey)
        {
            return await GetAsync<ExternalFactConfig>(FactStoreServiceUrlBuilder.GetExternalFactsUrl(factTypeName, factKey));
        }

        public async Task<ExternalFactConfig> PostExternalFactAsync(ExternalFactConfig externalFact)
        {
            return await PostAsync<ExternalFactConfig>(FactStoreServiceUrlBuilder.GetExternalFactsUrl(), externalFact);
        }

        public async Task<UpdateExternalFactConfig> PutExternalFactAsync(UpdateExternalFactConfig updateExternalFact)
        {
            return await PutAsync<UpdateExternalFactConfig>(FactStoreServiceUrlBuilder.GetExternalFactsUrl(), updateExternalFact);
        }

        public async Task<ExternalFactConfig> PutExternalFactUpdateFactValueAsync(ExternalFactConfig updateExternalFact)
        {
            return await PutAsync<ExternalFactConfig>(FactStoreServiceUrlBuilder.GetExternalFactUpdateFactValueUrl(), updateExternalFact);
        }

        public async Task DeleteExternalFactAsync(string factTypeName, string factKey)
        {
            await DeleteAsync<ExternalFactConfig>(FactStoreServiceUrlBuilder.GetExternalFactsUrl(factTypeName, factKey));
        }
        #endregion

        #region Private methods
        private async Task<TResult> PostAsync<TResult>(string uri, TResult entity) where TResult : class
        {
            SetAuthorizationHeader(_tokenProvider.GetAccessToken());

            var content = JsonContent.Create(entity);
            var response = await _httpClient.PostAsync(uri, content);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    var error = await response.Content.ReadAsAsync<System.Web.Http.HttpError>();
                    throw new Exception(error.Message);
                }
                throw new Exception(string.Empty);
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResult>(responseContent);
        }

        private async Task<TResult> PutAsync<TResult>(string uri, TResult entity) where TResult : class
        {
            SetAuthorizationHeader(_tokenProvider.GetAccessToken());

            var content = JsonContent.Create(entity);
            var response = await _httpClient.PutAsync(uri, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new AuthenticationException(((int)response.StatusCode).ToString());
            }
            return JsonSerializer.Deserialize<TResult>(responseContent);
        }

        private async Task<TResult> GetAllAsync<TResult>(string uri)
            where TResult : class
        {
            SetAuthorizationHeader(_tokenProvider.GetAccessToken());

            var response = await _httpClient.GetAsync(uri);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new AuthenticationException(((int)response.StatusCode).ToString()); //Create custom exception
            }
            return JsonSerializer.Deserialize<TResult>(responseContent);

        }

        private async Task<T> GetAsync<T>(string uri) where T : class
        {
            SetAuthorizationHeader(_tokenProvider.GetAccessToken());

            var response = await _httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                throw new AuthenticationException(((int)response.StatusCode).ToString()); //Create custom exception
            }
            return await response.Content.ReadFromJsonAsync<T>();
        }

        private async Task DeleteAsync<T>(string uri) where T : class
        {
            SetAuthorizationHeader(_tokenProvider.GetAccessToken());

            var response = await _httpClient.DeleteAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                throw new AuthenticationException(((int)response.StatusCode).ToString()); //Create custom exception
            }
        }

        private void SetAuthorizationHeader(string accessToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }
        #endregion
    }
}
