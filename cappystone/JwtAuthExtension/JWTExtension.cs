
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JwtAuthExtenstion
{
    public static class JWTExtension
    {
        public static void AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(
                 options =>
                 {
                     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                     options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                 }
            ).AddJwtBearer(op =>
            {
                op.SaveToken = true;
                op.RequireHttpsMetadata = true;
                op.IncludeErrorDetails = true;
                op.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "cwms-app-issuer",
                    ValidAudience = "cwms-app-audience",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("cwmsapp_9kQ2!r8F7zT4eB1pV6mN0yX3aC5dG")
                        )
                };

            });

        }
    }
}

