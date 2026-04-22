namespace HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces
{
    public interface ITokenProvider
    {
        /// <summary>
        /// Retrieves the stored authentication token from session.
        /// </summary>
        /// <returns>The authentication token if available; otherwise null or empty string.</returns>
        string? GetToken();

        /// <summary>
        /// Stores the authentication token in session.
        /// </summary>
        /// <param name="token">The token to store.</param>
        void SetToken(string token);

        /// <summary>
        /// Removes the authentication token from session.
        /// </summary>
        void RemoveToken();
    }
}
