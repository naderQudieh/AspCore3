using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace AppZeroAPI.Setup
{
    public static partial class SetUp
    {
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            // c.IncludeXmlComments(string.Format(@"{0}\AppZeroAPI.AppZeroAPI.xml", System.AppDomain.CurrentDomain.BaseDirectory));
            services.AddSwaggerGen(c =>
            { 
                
                c.CustomSchemaIds(type => type.ToString());
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "eProdaja", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"//   "basic"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer", // Id = "basic" The name of the previously defined security scheme.
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            new string[] {}
                    }
                });

            });
        }

        public class AuthResponsesOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
                var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter)
                    .Any(filter => filter is AuthorizeFilter);
                var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter)
                    .Any(filter => filter is IAllowAnonymousFilter);

                if (!isAuthorized || allowAnonymous) return;
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            }
        }
        public static void UseConfiguredSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.EnableValidator(null);
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web Api");
                c.RoutePrefix = string.Empty;
                c.DefaultModelsExpandDepth(-1);
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });
        }
    }
}
