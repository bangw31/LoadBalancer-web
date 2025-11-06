using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace ECommerceSystem.GUI.Services
{
    public class AuthRetryHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthRetryHandler> _logger;

        public AuthRetryHandler(IHttpContextAccessor httpContextAccessor, ILogger<AuthRetryHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                // Nếu header đã có thì không ghi đè
                if (request.Headers.Authorization == null)
                {
                    var context = _httpContextAccessor.HttpContext;

                    // 1) Ưu tiên lấy Authorization header từ request gốc (nếu client đã gửi)
                    string incomingAuth = context?.Request?.Headers["Authorization"].FirstOrDefault();
                    string token = null;

                    if (!string.IsNullOrWhiteSpace(incomingAuth))
                    {
                        // incomingAuth có thể ở dạng "Bearer <token>"
                        if (incomingAuth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            token = incomingAuth.Substring("Bearer ".Length).Trim();
                        else
                            token = incomingAuth.Trim();

                        _logger.LogDebug("AuthRetryHandler: found incoming Authorization header (copied to outgoing).");
                    }

                    // 2) Nếu không có, lấy token từ cookie (AuthToken)
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        token = context?.Request?.Cookies["AuthToken"];
                        if (!string.IsNullOrWhiteSpace(token))
                            _logger.LogDebug("AuthRetryHandler: token read from cookie.");
                    }

                    // 3) Nếu có token, gắn vào header outgoing
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        _logger.LogDebug("AuthRetryHandler: Authorization header set for outgoing request.");
                    }
                    else
                    {
                        _logger.LogDebug("AuthRetryHandler: no token found (cookie or Authorization header). Outgoing request will be unauthenticated.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthRetryHandler: error while attaching token.");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
