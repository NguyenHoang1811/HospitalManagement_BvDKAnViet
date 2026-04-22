namespace HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces
{
    public interface IApiService
    {
        /// <summary>
        /// Retrieves data from the specified URL.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response into.</typeparam>
        /// <param name="url">The API endpoint URL.</param>
        /// <returns>Deserialized response data or null if not found.</returns>
        Task<T?> GetAsync<T>(string url);

        /// <summary>
        /// Posts data and retrieves a typed response.
        /// </summary>
        /// <typeparam name="TRequest">The request data type.</typeparam>
        /// <typeparam name="TResponse">The response data type.</typeparam>
        /// <param name="url">The API endpoint URL.</param>
        /// <param name="data">The request payload.</param>
        /// <returns>Deserialized response data or null if not found.</returns>
        Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data);

        /// <summary>
        /// Posts data without expecting a typed response.
        /// </summary>
        /// <typeparam name="TRequest">The request data type.</typeparam>
        /// <param name="url">The API endpoint URL.</param>
        /// <param name="data">The request payload.</param>
        /// <returns>A completed task.</returns>
        Task PostAsync<TRequest>(string url, TRequest data);

        /// <summary>
        /// Updates data at the specified URL.
        /// </summary>
        /// <typeparam name="TRequest">The request data type.</typeparam>
        /// <param name="url">The API endpoint URL.</param>
        /// <param name="data">The request payload.</param>
        /// <returns>A completed task.</returns>
        Task PutAsync<TRequest>(string url, TRequest data);

        /// <summary>
        /// Deletes data at the specified URL.
        /// </summary>
        /// <param name="url">The API endpoint URL.</param>
        /// <returns>A completed task.</returns>
        Task DeleteAsync(string url);

        /// <summary>
        /// Sets the authorization token for subsequent requests.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        void AddToken(string token);

        /// <summary>
        /// Removes the authorization token.
        /// </summary>
        void RemoveToken();
    }
}
