using System;
using DevIO.Api.Extensions;
using Elmah.Io.AspNetCore;
using Elmah.Io.AspNetCore.HealthChecks;
using Elmah.Io.Extensions.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddElmahIo(o =>
            {
                o.ApiKey = "65f342039a4c4f8b9c685ba90b8a467d";
                o.LogId = new Guid("d4c37be9-927d-4972-8b94-9c53ddae452a");
            });

            // Logs pelo ElmahIo
            //services.AddLogging(builder =>
            //{
            //    builder.AddElmahIo(o =>
            //    {
            //        o.ApiKey = "65f342039a4c4f8b9c685ba90b8a467d";
            //        o.LogId = new Guid("d4c37be9-927d-4972-8b94-9c53ddae452a");
            //    });

            //    // Sem Categoria e tipo de log de Warning para cima
            //    builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);
            //});

            services.AddHealthChecks() // Verifica se a aplicação esta OK ou não
                .AddElmahIoPublisher("65f342039a4c4f8b9c685ba90b8a467d", new Guid("d4c37be9-927d-4972-8b94-9c53ddae452a"), "API Fornecedores") // Config para o Elmah
                .AddCheck("Produtos", new SqlServerHealthCheck(configuration.GetConnectionString("DefaultConnection"))) // HealthChecks que criamos
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"), name: "BancoSQL"); // Verifica Banco
            services.AddHealthChecksUI(); // Interface para o HealthChecks

            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();

            app.UseHealthChecks("/api/hc", new HealthCheckOptions() // Com o HealthCheckOptions - o HealthChecks retornara um json para que a UI possa apresenta-lo melhor
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse

            }); // Verifica se a aplicação esta OK ou não

            app.UseHealthChecksUI(options => // Interface
            {
                options.UIPath = "/api/hc-ui";
            });

            return app;
        }
    }
}
