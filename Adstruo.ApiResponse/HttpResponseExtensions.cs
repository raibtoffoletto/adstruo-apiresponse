using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Adstruo.ApiResponse;

/// <summary>Class containing helpful methods</summary>
public static class HttpResponseExtensions
{
    /// <summary>Serializes the HttpResponse using Newtonsoft.Json</summary>
    public static async Task<HttpResponse> JsonSerializerAsync(
        this HttpResponse res,
        object? value,
        int statusCode = 200
    )
    {
        res.StatusCode = statusCode;
        res.ContentType = "application/json; charset=utf-8";

        await res.WriteAsync(
            JsonConvert.SerializeObject(
                value,
                Formatting.None,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    },
                }
            )
        );

        return res;
    }

    /// <summary>Adds the invalid model to WebApplicationBuilder services</summary>
    [ExcludeFromCodeCoverage]
    public static IServiceCollection AddControllersWithInvalidModel(
        this IServiceCollection services
    )
    {
        services
            .AddControllers(x => x.AllowEmptyInputInBodyModelBinding = true)
            .ConfigureApiBehaviorOptions(
                options =>
                    options.InvalidModelStateResponseFactory = context =>
                        new ApiInvalidModel(context.ModelState)
            );

        return services;
    }

    /// <summary>Adds the catch all exceptions middleware</summary>
    public static IApplicationBuilder UseApiExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiExceptionMiddleware>();
    }
}
