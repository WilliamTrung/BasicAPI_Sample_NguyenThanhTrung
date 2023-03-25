using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace eStoreAPI.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CustomAuthorize : ActionFilterAttribute, IActionFilter 
    {
        private readonly string[]? _roles;
        public CustomAuthorize(params string[]? roles)
        {
            _roles = roles;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var isAuthorized = false;
            AuthenticationHeaderValue.TryParse(context.HttpContext.Request.Headers.Authorization, out var bearer);
            if (bearer != null)
            {
                string? token = bearer.Parameter;
                //token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImFkbWluQGVzdG9yZS5jb20iLCJyb2xlIjoiQWRtaW5pc3RyYXRvciIsIm5iZiI6MTY3NDU1MjYzOSwiZXhwIjoxNjc0NTUyNjk5LCJpYXQiOjE2NzQ1NTI2MzksImlzcyI6Imlzc3VlciIsImF1ZCI6ImF1ZGllbmNlIn0.mnZAj8Zkeut8hPtmkGfJZB5NjrG2pTdw2T1JtemjBKM";
                if (token != null)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadJwtToken(token);
                    string email = jwtToken.Payload.Claims.First(claim => claim.Type == "email").Value;
                    string role = jwtToken.Payload.Claims.First(claim => claim.Type == "role").Value;
                    var id = jwtToken.Payload.Claims.First(claim => claim.Type == "id").Value;
                    if (_roles != null && _roles.Length > 0)
                    {
                        if (_roles.Contains(role))
                        {
                            if (role == "Member")
                            {
                                var uri = context.HttpContext.Request.GetEncodedUrl();
                                if (uri.Contains(id))
                                {
                                    isAuthorized = true;
                                }
                            }
                            else
                                isAuthorized = true;
                        }
                    }
                    else if (role != null)
                    {
                        isAuthorized = true;
                    }
                }
            }
            if (!isAuthorized)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
            base.OnActionExecuting(context);
        }
    }
}
