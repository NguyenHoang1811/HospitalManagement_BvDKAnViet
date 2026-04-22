using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using System.Net;
using System.Net.Http.Headers;

namespace HospitalManagement_BvDKAnViet.WepApp.Services.Handlers
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly ITokenProvider _tokenProvider;

        public AuthTokenHandler(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Get token from session via ITokenProvider
            var token = _tokenProvider.GetToken();

            // Add Bearer token to request if available
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Send the request
            var response = await base.SendAsync(request, cancellationToken);

            // Clear token from session if API returns 401 Unauthorized
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _tokenProvider.RemoveToken();
            }

            return response;
        }
    }
}
