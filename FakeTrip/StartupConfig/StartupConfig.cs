using FakeTrip.Constants;
using FakeTrip.Databases;
using FakeTrip.Models;
using FakeTrip.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Text;

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

    public static void AddCustomServices(this WebApplicationBuilder builder)
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

    public static void AddAuthServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        builder.Services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration.GetValue<string>(AuthenticationConstant.Issuer),
                    ValidAudience = builder.Configuration.GetValue<string>(AuthenticationConstant.Audience),
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(
                        builder.Configuration.GetValue<string>(AuthenticationConstant.SecretKey)!))
                };
            });

        builder.Services.AddAuthorization(opts =>
        {
            opts.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });        
    }
}
