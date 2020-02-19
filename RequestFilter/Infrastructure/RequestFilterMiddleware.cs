using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestFilter.Infrastructure
{
    public class RequestFilterMiddleware
    {
        public static IEnumerable<string> WhiteListIpCollection;
        public static IEnumerable<string> RestrictPathCollection;
        private readonly RequestDelegate _next;

        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestFilterMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public RequestFilterMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            this._next = next;
            this._httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public async Task Invoke(HttpContext context)
        {
            if (WhiteListIpCollection.Any().Equals(false) && RestrictPathCollection.Any().Equals(false))
            {
                await this._next.Invoke(context);
            }

            var requestPath = context.Request.Path.ToString();
            var isPathInRestricts = RestrictPathCollection.Any(x => requestPath.StartsWith($"/{x}", StringComparison.OrdinalIgnoreCase));

            if (isPathInRestricts.Equals(true))
            {
                var ipAddress = context.Connection.RemoteIpAddress.ToString();
                var clientIp = this._httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                ipAddress = string.IsNullOrWhiteSpace(clientIp)
                    ? ipAddress
                    : clientIp;

                var isIpInWhiteList = WhiteListIpCollection.Any(x => ipAddress.StartsWith(x));
                if (isIpInWhiteList.Equals(false))
                {
                    // 當 IP 不在白名單裡且瀏覽路徑在限制名單裡，跳轉到 /Home/Index
                    context.Response.Redirect("/");

                    // 或是直接給 403
                    // context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                    return;
                }
            }

            await this._next.Invoke(context);
        }
    }
}