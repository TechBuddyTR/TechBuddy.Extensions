using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechBuddy.Extensions.OpenApi.Infrastructure;
internal class SwaggerConstants
{
    internal const string DefaultProjectName = "TechBuddy.Extensions.ProjectName";

    internal const string DefaultSwaggerEndpointFileName = "swagger.json";
    internal const string DefaultSwaggerEndpointFileDirectory = "swagger";
    internal const string DefaultSwaggerEndpoint = $"/{DefaultSwaggerEndpointFileDirectory}/v1/{DefaultSwaggerEndpointFileName}";
    internal const string DefaultSwaggerApiVersion = "1";


    internal const string BearerFormat = "JWT";
    internal const string BearerHeaderKey = "bearer";
    internal static string DefaultBearerDescription
        => "JWT Authorization header using the {HeaderKey} scheme. \r\n\r\n Enter '{HeaderKey}' [space] and then your token in the text input below.\r\n\r\nExample: \"{HeaderKey} {TOKEN}\"".Replace("{HeaderKey}", BearerHeaderKey);
}
