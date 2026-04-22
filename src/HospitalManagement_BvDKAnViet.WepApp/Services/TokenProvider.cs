using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;

namespace HospitalManagement_BvDKAnViet.WepApp.Services

{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string TokenSessionKey = "AuthToken";

        public TokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Retrieves the stored authentication token from session.
        /// </summary>
        public string? GetToken()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.Session.GetString(TokenSessionKey);
        }

        /// <summary>
        /// Stores the authentication token in session.
        /// </summary>
        public void SetToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                RemoveToken();
                return;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            httpContext?.Session.SetString(TokenSessionKey, token);
        }

        /// <summary>
        /// Removes the authentication token from session.
        /// </summary>
        public void RemoveToken()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext?.Session.Remove(TokenSessionKey);
        }
    }
}
