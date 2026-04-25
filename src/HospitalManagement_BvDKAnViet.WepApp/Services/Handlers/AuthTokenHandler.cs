using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace HospitalManagement_BvDKAnViet.WepApp.Services.Handlers
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenHandler(ITokenProvider tokenProvider,
                                IHttpContextAccessor httpContextAccessor)
        {
            _tokenProvider = tokenProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _tokenProvider.GetToken();

            // Fallback: đọc từ Cookie claim nếu Session không có
            if (string.IsNullOrWhiteSpace(token))
            {
                token = _httpContextAccessor.HttpContext?
                    .User?.FindFirst("AccessToken")?.Value;
            }

            

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}