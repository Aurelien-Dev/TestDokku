﻿using Client.Services;
using Client.Services.Authentication;
using Repository;
using System.Security.Claims;

namespace Client.Utils.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AuthenticateMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, ApplicationDbContext dbContext, SessionInfo currentSession, AuthenticationService authenticationService)
        {
            ClaimsPrincipal claimsP = httpContext.User;
            if (claimsP.Identity.IsAuthenticated && !authenticationService.AuthanticateInitialized)
            {
                int idWorkshop = Convert.ToInt32(claimsP.Claims.FirstOrDefault(d => d.Type == "IdWorkshop").Value);

                currentSession.ClaimsPrincipal = httpContext.User;
                currentSession.Workshop = dbContext.Workshops.FirstOrDefault(w => w.Id == idWorkshop);
                currentSession.IsAuthenticate = true;
                return _next(httpContext);
            }

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticateMiddleware>();
        }
    }
}
