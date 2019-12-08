using DevIO.Api.Data;
using DevIO.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

            // JWT
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection); // Configurando no aspnet core nosso token

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret); // Chave precisa estar em ASCII

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Aqui setamos para toda autenticacao gerar um token
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // E a verificação de autenticidade tambem será com base no token
            }).AddJwtBearer(x => {
                x.RequireHttpsMetadata = false; // Se estivermos so trabalhando com HTTPS podemos deixar true, requer que a pessoa que esta chamando ele esteja vindo de https 
                x.SaveToken = true; // O token é guardado no AuthenticationProperties, facilitando a validacao do token
                x.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true, // Valida quem esta emitindo o token
                    IssuerSigningKey = new SymmetricSecurityKey(key), // Configuracao da chave
                    ValidateIssuer = true, // Valida o Issuer setado no appsettings.json
                    ValidateAudience = true, // Valida o Audience setado no appsettings.json
                    ValidAudience = appSettings.ValidoEm, // Valida o Audience setado no appsettings.json
                    ValidIssuer = appSettings.Emissor // Valida o Issuer setado no appsettings.json
                };
            });

            return services;
        }
    }
}
