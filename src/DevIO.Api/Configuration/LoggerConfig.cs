using Elmah.Io.AspNetCore;
using Elmah.Io.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services)
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

            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();
            return app;
        }
    }
}
