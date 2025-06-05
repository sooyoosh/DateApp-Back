using DatingApp.Data;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.Services;
using DatingApp.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddControllers();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("myConnection"));
            });
            services.AddCors(
                options =>
                {
                    options.AddPolicy("CorsPolicy", policy =>
                    {
                        policy.WithOrigins("https://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
                }
                );
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository > ();
            services.AddScoped<ILikesRepository, LikesRepository> ();
            services.AddScoped<IMessageRepository, MessageRepository> ();
            services.AddScoped<IUnitOfWork,UnitOfWork> ();
            //cloudinary
            services.AddScoped<IPhotoService, PhotoService>();
            //cloudinary
            services.AddScoped<LogUserActivity>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<CloudinarySettings>(
            configuration.GetSection("CloudinarySettings"));
            services.AddSignalR();
            services.AddSingleton<PresenceTracker>();


            return services;
        }


    }
}
