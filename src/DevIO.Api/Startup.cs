using AutoMapper;
using DevIO.Api.Configuration;
using DevIO.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configuração entity
            services.AddDbContext<MeuDbContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentityConfiguration(Configuration);

            services.AddAutoMapper(typeof(Startup)); // Busca tudo que foi implementado pelo 'Profile'

            services.ResolveDependencies(); // extension metodo de dependencias

            services.WebApiConfig();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication(); // Identity - é necessario ele sempre vir antes do 'UseMvcConfiguration' senão não vai funcionar

            app.UseMvcConfiguration();
        }
    }
}
