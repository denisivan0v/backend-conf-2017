using Demo.Swashbuckle;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using Swashbuckle.AspNetCore.Swagger;

namespace Demo
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder();
            var configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.LiterateConsole(outputTemplate: "[{Timestamp:HH:mm:ss:fff} {Level:u3}] [{ThreadId}] {Message}{NewLine}{Exception}")
                .Enrich.WithThreadId()
                .CreateLogger();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore().AddVersionedApiExplorer();
            services.AddMvc();
            services.AddApiVersioning(options => options.ReportApiVersions = true);
            services.AddSwaggerGen(
                x =>
                    {
                        var provider = services.BuildServiceProvider()
                                               .GetRequiredService<IApiVersionDescriptionProvider>();
                        foreach (var description in provider.ApiVersionDescriptions)
                        {
                            x.SwaggerDoc(
                                description.GroupName,
                                new Info
                                    {
                                        Title = $"DotNext Demo API {description.ApiVersion}",
                                        Version = description.ApiVersion.ToString()
                                    });
                        }

                        x.TagActionsBy(d => d.ActionDescriptor.RouteValues["controller"]);
                        x.OperationFilter<ImplicitApiVersionParameter>();
                    });
            services.AddSingleton<RemoteService>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(
                c =>
                    {
                        var provider = app.ApplicationServices
                                          .GetRequiredService<IApiVersionDescriptionProvider>();
                        foreach (var description in provider.ApiVersionDescriptions)
                        {
                            c.SwaggerEndpoint(
                                $"/swagger/{description.GroupName}/swagger.json",
                                description.GroupName.ToUpperInvariant());
                        }

                        c.DocExpansion("full");
                        c.EnabledValidator();
                        c.ShowRequestHeaders();
                    });
        }
    }
}
