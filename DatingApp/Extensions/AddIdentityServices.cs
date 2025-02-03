using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DatingApp.Extensions
{
    public static class AddIdentityServices
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
            {
                var tokenKey = configuration["TokenKey"] ?? throw new Exception("tokenKey not found");
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false

                };


            });

            return services;
        }

    }
}
