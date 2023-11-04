using Microsoft.AspNetCore.Http;
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
}
