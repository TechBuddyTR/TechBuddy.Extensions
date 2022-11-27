using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechBuddy.Extensions.OpenApi.Infrastructure.ConfigModels;

namespace TechBuddy.Extensions.OpenApi.Infrastructure.OperationFilters;

internal class AuthenticationRequirements : IOperationFilter
{
    private readonly SwaggerBearerConfig config;

    public AuthenticationRequirements(SwaggerBearerConfig config)
    {
        this.config = config;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Security ??= new List<OpenApiSecurityRequirement>();

        var scheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = config.HeaderKey
            }
        };

        var req = new OpenApiSecurityRequirement
        {
            { scheme, new List<string>() }
        };

        operation.Security.Add(req);
    }
}
