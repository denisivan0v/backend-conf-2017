using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.ApiExplorer;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Demo.Swashbuckle
{
    public sealed class ImplicitApiVersionParameter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var apiVersion = context.ApiDescription.GetApiVersion();
            if (apiVersion == null)
            {
                return;
            }

            var parameters = operation.Parameters;
            if (parameters == null)
            {
                operation.Parameters = parameters = new List<IParameter>();
            }

            var parameter = parameters.SingleOrDefault(p => p.Name == "api-version");
            if (parameter == null)
            {
                parameter = new NonBodyParameter
                                {
                                    Name = "api-version",
                                    Required = true,
                                    Default = apiVersion.ToString(),
                                    In = "query",
                                    Type = "string"
                                };

                parameters.Add(parameter);
            }

            var pathParameter = parameter as NonBodyParameter;
            if (pathParameter != null)
            {
                pathParameter.Default = apiVersion.ToString();
            }

            parameter.Description = "The requested API version";
        }
    }
}
