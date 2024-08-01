using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


public class JwtAuthenticationAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _role;

    public JwtAuthenticationAttribute(string role = null)
    {
        _role = role;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var request = context.HttpContext.Request;
        var token = request.Cookies["jwt"];

        if (token == null)
        {
            context.HttpContext.Response.Redirect("/Users/Unauthorized");
            context.Result = new EmptyResult();
            return;
        }

        try
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
           
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var principal = handler.ValidateToken(token, validationParameters, out var securityToken);
            var claimsIdentity = principal.Identity as ClaimsIdentity;

            if (claimsIdentity == null || (_role != null && !claimsIdentity.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == _role)))
            {
                context.HttpContext.Response.Redirect("/Users/Unauthorized");
                context.Result = new EmptyResult();
                return;
            }
            var jwtToken = securityToken as JwtSecurityToken;
            if (jwtToken != null && jwtToken.ValidTo < DateTime.UtcNow)
            {
                context.HttpContext.Response.Redirect("/Users/Unauthorized");
                context.Result = new EmptyResult();
                return;
            }
        }
        catch
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
