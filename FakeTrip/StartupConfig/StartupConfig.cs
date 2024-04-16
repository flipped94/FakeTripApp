using FakeTrip.Databases;
using FakeTrip.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

namespace FakeTrip.StartupConfig;

public static class StartupConfig
{
    public static void AddDatabaseContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(ops =>
        {
            ops.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
        });
    }

    public static void AddCustomService(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    public static void AddStandard(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(opts =>
        {
            opts.ReturnHttpNotAcceptable = true;
        })
            .AddNewtonsoftJson(opts =>
            {
                opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetail = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "请求参数验证",
                        Title = "请求参数验证失败",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "请看详细说明",
                        Instance = context.HttpContext.Request.Path
                    };
                    problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                    return new UnprocessableEntityObjectResult(problemDetail)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });
    }
}
