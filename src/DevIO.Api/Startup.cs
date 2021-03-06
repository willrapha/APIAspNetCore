﻿using AutoMapper;
using DevIO.Api.Configuration;
using DevIO.Api.Extensions;
using DevIO.Data.Context;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment hostingEnvironment)
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(hostingEnvironment.ContentRootPath)
                 .AddJsonFile("appsettings.json", true, true)
                 .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true, true)
                 .AddEnvironmentVariables();

            if (hostingEnvironment.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

       

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configuração entity
            services.AddDbContext<MeuDbContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentityConfiguration(Configuration);

            services.AddAutoMapper(typeof(Startup)); // Busca tudo que foi implementado pelo 'Profile'

            services.WebApiConfig();

            services.AddSwaggerConfig();

            services.AddLoggingConfiguration(Configuration);
              
            services.ResolveDependencies(); // extension metodo de dependencias
                

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // IApiVersionDescriptionProvider provider - do proprio AspNet Core
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            // é necessario ele sempre vir antes do 'UseMvcConfiguration' senão não vai funcionar
            if (env.IsDevelopment())
            {
                app.UseCors("Development"); // Permite qualquer origem qualquer metodo qualquer credencial
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseCors("Production");
                app.UseHsts();
            }

            app.UseAuthentication(); // Identity - é necessario ele sempre vir antes do 'UseMvcConfiguration' senão não vai funcionar

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseMvcConfiguration();

            app.UseSwaggerConfig(provider);

            app.UseLoggingConfiguration();  
        }
    }
}
