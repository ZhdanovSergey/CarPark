using Microsoft.AspNetCore.Antiforgery;
using System.Globalization;

namespace CarPark.Middleware
{
    public class AntiForgeryTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryTokenMiddleware(RequestDelegate next, IAntiforgery antiforgery)
        {
            _next = next;
            _antiforgery = antiforgery;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var tokens = _antiforgery.GetAndStoreTokens(context);
            context.Response.Headers.Add("X-CSRF-TOKEN", tokens.RequestToken);

            await _next(context);
        }
    }
}
