using Microsoft.AspNetCore.Mvc;
using SwaggerExtension.Tests.WebApi.Controllers.v2;
using SwaggerExtension.Tests.WebApi.Infrastructure.Models;
using TechBuddy.Extensions.AspNetCore.ApiVersioning;
using TechBuddy.Extensions.OpenApi;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddApiVersioning();
builder.Services.AddTechBuddyApiVersioning(option =>
{
    option.AddQueryStringApiVersionReader("x-api-version");
    option.EnableVersionedApiExplorer = true;
});

builder.Services.ConfigureTechBuddySwagger(config =>
{
    config.AddHeader("TenantId", "TB_Company");

    config.BearerConfig = new SwaggerBearerConfig()
    {
        AuthEnabled = true
    };

    config.XmlDocConfig = new SwaggerDocConfig()
    {
        XmlFilePath = "SwaggerExtension.Tests.WebApi.xml"
    };


    var provider = ResponseTypeModelProviderConfig.CreateDefault();
    provider.ClearDefaultTypes();
    provider.ClearDefaultResponseHttpStatusCodes();

    provider
        .AddDefaultType(typeof(Task), typeof(ActionResult), typeof(IActionResult))
        .AddDefaultResponseHttpStatusCodeForAll(System.Net.HttpStatusCode.OK)
        .AddSpecificTypeForSpecificHttpStatusCode(System.Net.HttpStatusCode.OK, typeof(string))

        .AddDefaultResponseHttpStatusCodeForHttpMethods(HttpMethod.Post, System.Net.HttpStatusCode.BadRequest)
        .AddSpecificTypeForSpecificHttpStatusCode(System.Net.HttpStatusCode.BadRequest, typeof(BadRequestResponseModel))
        .AddDefaultResponseHttpStatusCodeForHttpMethods(HttpMethod.Get, System.Net.HttpStatusCode.NoContent)

        .ExcludeController(nameof(TestControllerV2));

    config.ResponseTypeModelProviderConfig = provider;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseTechBuddySwagger();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

