using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechBuddy.Extensions.OpenApi.Infrastructure.ConfigModels;

/// <summary>
/// The config to set the Auth in Swagger implementation
/// </summary>
public class SwaggerBearerConfig
{
    /// <summary>
    /// true when JWT/Auth is enabled on Swagger UI
    /// </summary>
    public bool AuthEnabled { get; set; }

    /// <summary>
    /// The bearer key to use as a prefix with JWT in header
    /// </summary>
    public string HeaderKey { get; set; } = SwaggerConstants.BearerHeaderKey;

    /// <summary>
    /// The description will be shown on Swagger UI Auth
    /// </summary>
    public string BearerDescription => SwaggerConstants.DefaultBearerDescription;
}
