using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using System.Net.Http.Headers;

namespace HospitalManagement_BvDKAnViet.WepApp.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _http;
        private readonly ITokenProvider _tokenProvider;

        public ApiService(HttpClient http, ITokenProvider tokenProvider)
        {
            _http = http;
            _tokenProvider = tokenProvider;
        }

        /// <summary>
        /// Retrieves data from the specified URL.
        /// </summary>
        public async Task<T?> GetAsync<T>(string url)
        {
            using var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        /// <summary>
        /// Posts data and retrieves a typed response.
        /// </summary>
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            using var response = await _http.PostAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        /// <summary>
        /// Posts data without expecting a typed response.
        /// </summary>
        public async Task PostAsync<TRequest>(string url, TRequest data)
        {
            using var response = await _http.PostAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Updates data at the specified URL.
        /// </summary>
        public async Task PutAsync<TRequest>(string url, TRequest data)
        {
            using var response = await _http.PutAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Deletes data at the specified URL.
        /// </summary>
        public async Task DeleteAsync(string url)
        {
            using var response = await _http.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Sets the authorization token for subsequent requests.
        /// </summary>
        public void AddToken(string token)
        {
            _tokenProvider.SetToken(token);
        }

        /// <summary>
        /// Removes the authorization token.
        /// </summary>
        public void RemoveToken()
        {
            _tokenProvider.RemoveToken();
        }
    }
}
