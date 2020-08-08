using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SterlingCommunityAPI.Services.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.Keys.Contains("NounceVal"))
            {
                context.Response.StatusCode = 401; //Unauthorized
                return;
            }

            if (context.Request.Headers.Keys.Contains("NounceVal"))
            {
                var keyName = (context.Request.Headers.FirstOrDefault(x => x.Key == "NounceVal").Value.ToString());
                if (keyName== "AAECAwQFBgcICQoLDA0ODw")
                {

                }
                else
                {
                    context.Response.StatusCode = 401; //Unauthorized
                    return;
                }
              
            }
            await _next.Invoke(context);
        }
    }
}
