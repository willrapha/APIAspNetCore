using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace DevIO.Api.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection WebApiConfig(this IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Desabilitar modelState automatico - para personalizarmos os erros
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            
            // Cors - não é um recurso de segurança, nem tudo usa Cors, ele server para relaxar um pouco a forma de outras origem acessarem a aplicação
            services.AddCors(options =>
            {
                // Permite qualquer origem qualquer metodo qualquer credencial
                options.AddPolicy("Development",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());

                /* Politica padrao
                options.AddDefaultPolicy(
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials());*/

                options.AddPolicy("Production",
                    builder =>
                        builder
                            .WithMethods("GET", "POST") // Permitido metodos apenas com verbo GET e POST
                            .WithOrigins("http://desenvolvedor.io") // Apenas para origem deste site
                            .SetIsOriginAllowedToAllowWildcardSubdomains() // E sub dominios
                            //.WithHeaders(HeaderNames.ContentType, "x-custom-header") // Apenas para esse tipo de header
                            .AllowAnyHeader());
            });

            return services;
        }

        public static IApplicationBuilder UseMvcConfiguration(this IApplicationBuilder app)
        { 
            app.UseHttpsRedirection();
            app.UseMvc();

            return app;
        }
    }
}