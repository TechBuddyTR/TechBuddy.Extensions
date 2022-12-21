using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;

namespace TechBuddy.Extensions.OpenApi.Infrastructure.OperationFilters
{
    internal class JsonIgnoreOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            IList<ApiParameterDescription> parameterDescriptions = context.ApiDescription.ParameterDescriptions;

            foreach (ApiParameterDescription parameterDescription in parameterDescriptions)
            {
                bool? ignore = parameterDescription.ModelMetadata is DefaultModelMetadata metaData
                    ? metaData.Attributes.PropertyAttributes?.Any(x => x is JsonIgnoreAttribute)
                    : false;

                if (ignore.HasValue && ignore.Value)
                {
                    OpenApiParameter apiParameter = operation.Parameters.FirstOrDefault(x => x.Name == parameterDescription.Name);

                    if (apiParameter is not null)
                    {
                        operation.Parameters.Remove(apiParameter);
                    }
                }
            }

        }
    }
}
