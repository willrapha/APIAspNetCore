using DevIO.Api.Data;
using DevIO.Api.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.Api.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Adicionar identity na aplicação
            // IdentityUser - classe usuario padrao
            // AddRoles - trabalhar com Roles - passamos o 'IdentityRole' para podermos customizar a politica de roles
            // AddEntityFrameworkStores - dizemos que estamos trabalhando com entity e qual seu contexto
            // AddDefaultTokenProviders - recurso para gerar tokens
            // AddErrorDescriber - erros identity
            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddErrorDescriber<IdentityMensagensPortugues>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
