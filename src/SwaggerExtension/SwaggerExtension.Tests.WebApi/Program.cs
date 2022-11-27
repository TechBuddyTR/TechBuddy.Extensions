using Microsoft.AspNetCore.Mvc.ApiExplorer;
using TechBuddy.Extensions.AspNetCore.ApiVersioning;
using TechBuddy.Extensions.OpenApi.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddApiVersioning();
builder.Services.AddTechBuddyApiVersioning(option =>
{
    option.AddQueryStringApiVersionReader("x-api-version");
    option.EnableVersionedApiExplorer = true;
});


//builder.Services.AddSwaggerGen(options =>
//{
//    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
//});

builder.Services.ConfigureTechBuddySwagger(config =>
{
    config.AddHeader("TenantId", "TB_Company");

    config.BearerConfig = new TechBuddy.Extensions.OpenApi.Infrastructure.ConfigModels.SwaggerBearerConfig()
    {
        AuthEnabled = true
    };

    config.XmlDocConfig = new TechBuddy.Extensions.OpenApi.Infrastructure.ConfigModels.SwaggerDocConfig()
    {
        XmlFilePath = "SwaggerExtension.Tests.WebApi.xml"
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.UseTechBuddySwagger();
    app.UseTechBuddySwaggerWithApiVersioning(app.Services.GetRequiredService<IApiVersionDescriptionProvider>());
}




app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

