using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BankAccountManager.ExtensionMethods;

public static class ServicesExtensions
{
    public static void AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var audience = configuration["Jwt:Audience"];
                var issuer = configuration["Jwt:Issuer"];
                var secret = configuration["Jwt:Secret"];

                if (audience == null || issuer == null || secret == null)
                {
                    throw new ApplicationException("Jwt settings are missing");
                }

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                };
            });
    }
}
