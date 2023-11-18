using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

namespace Adstruo.ApiResponse.Tests;

public class Server
{
    private readonly ILogger _log;
    private readonly RouteData _router;
    private readonly ActionDescriptor _action;

    public Server()
    {
        _log = Mock.Of<ILogger>();
        _router = Mock.Of<RouteData>();
        _action = Mock.Of<ActionDescriptor>();
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseApiExceptionMiddleware();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet(
                Routes.Void,
                async (context) =>
                {
                    Mock<HttpContext> mockContext = new();
                    mockContext.Setup(m => m.Response).Returns(context.Response);

                    await new ApiResponse(_log).ExecuteResultAsync(
                        new ActionContext(mockContext.Object, _router, _action)
                    );
                }
            );

            endpoints.MapPost(
                Routes.Error,
                async (context) =>
                {
                    Mock<HttpContext> mockContext = new();
                    mockContext.Setup(m => m.Response).Returns(context.Response);

                    IDictionary<string, string> data =
                        await GetBodyData(context) ?? throw new Exception("Couldn't get body data");

                    ApiException ex = new UnknownException();

                    switch (data["error"])
                    {
                        case ApiExceptionError.ALL_FIELDS_REQUIRED:
                            ex = new AllFieldsRequiredException();
                            break;

                        case ApiExceptionError.BAD_REQUEST:
                            ex = new BadRequestException();
                            break;

                        case ApiExceptionError.FAILED_TO_PARSE_REQUEST:
                            ex = new FailedToParseRequestException();
                            break;

                        case ApiExceptionError.FORBIDDEN:
                            ex = new ForbiddenException();
                            break;

                        case ApiExceptionError.INVALID_USER_CREDENTIALS:
                            ex = new InvalidUserCredentialsException();
                            break;

                        case ApiExceptionError.NETWORK_ERROR:
                            ex = new NetworkErrorException();
                            break;

                        case ApiExceptionError.NOT_FOUND:
                            ex = new NotFoundException();
                            break;

                        case ApiExceptionError.UNAUTHENTICATED:
                            ex = new UnauthenticatedException();
                            break;

                        case ApiExceptionError.UNAUTHORIZED:
                            ex = new UnauthorizedException();
                            break;

                        default:
                            break;
                    }

                    ApiResult result = new();
                    result.SetError(ex);

                    ActionContext res = new(mockContext.Object, _router, _action);

                    await res.HttpContext.Response.JsonSerializerAsync(result, result.Code);
                }
            );

            endpoints.MapGet(
                Routes.GenericError,
                async (context) =>
                {
                    Mock<HttpContext> mockContext = new();
                    mockContext.Setup(m => m.Response).Returns(context.Response);

                    ApiResponse<dynamic> response = new(() => throw new Exception(), _log);

                    await response.ExecuteResultAsync(
                        new ActionContext(mockContext.Object, _router, _action)
                    );
                }
            );

            endpoints.MapGet(
                Routes.VoidActionError,
                async (context) =>
                {
                    Mock<HttpContext> mockContext = new();
                    mockContext.Setup(m => m.Response).Returns(context.Response);

                    ApiResponse response = new(() => throw new Exception(), _log);

                    await response.ExecuteResultAsync(
                        new ActionContext(mockContext.Object, _router, _action)
                    );
                }
            );

            endpoints.MapGet(
                Routes.UnknownError,
                async (context) =>
                {
                    Mock<HttpContext> mockContext = new();
                    mockContext.Setup(m => m.Response).Returns(context.Response);

                    ApiResult result = new();
                    result.SetError();

                    ActionContext res = new(mockContext.Object, _router, _action);

                    await res.HttpContext.Response.JsonSerializerAsync(result, result.Code);
                }
            );

            endpoints.MapPost(
                Routes.Data,
                async (context) =>
                {
                    Mock<HttpContext> mockContext = new();
                    mockContext.Setup(m => m.Response).Returns(context.Response);

                    IDictionary<string, string>? data = await GetBodyData(context);

                    ApiResponse<dynamic> response =
                        new(
                            () =>
                                data == null || data.Count == 0
                                    ? throw new AllFieldsRequiredException()
                                    : (dynamic)data,
                            _log
                        );

                    await response.ExecuteResultAsync(
                        new ActionContext(mockContext.Object, _router, _action)
                    );
                }
            );

            endpoints.MapGet(
                Routes.InvalidModel,
                async (context) =>
                {
                    Mock<HttpContext> mockContext = new();
                    mockContext.Setup(m => m.Response).Returns(context.Response);

                    await new ApiInvalidModel().ExecuteResultAsync(
                        new ActionContext(mockContext.Object, _router, _action)
                    );
                }
            );

            endpoints.MapPost(
                Routes.PrivateVoid,
                async (context) =>
                {
                    Mock<HttpContext> mockContext = new();
                    mockContext.Setup(m => m.Response).Returns(context.Response);

                    IDictionary<string, string>? data = await GetBodyData(context);

                    await new ApiResponse(data, _log).ExecuteResultAsync(
                        new ActionContext(mockContext.Object, _router, _action)
                    );
                }
            );

            endpoints.MapPost(
                Routes.PrivateData,
                async (context) =>
                {
                    Mock<HttpContext> mockContext = new();
                    mockContext.Setup(m => m.Response).Returns(context.Response);

                    IDictionary<string, string>? data = await GetBodyData(context);

                    ApiResponse<dynamic> response = new(() => data!, data, _log);

                    await response.ExecuteResultAsync(
                        new ActionContext(mockContext.Object, _router, _action)
                    );
                }
            );

            endpoints.MapPost(
                Routes.PrivateDataAsync,
                async (context) =>
                {
                    Mock<HttpContext> mockContext = new();
                    mockContext.Setup(m => m.Response).Returns(context.Response);

                    IDictionary<string, string>? data = await GetBodyData(context);

                    ApiResponse<dynamic> response =
                        new(async () => await Task.FromResult(data!), data, _log);

                    await response.ExecuteResultAsync(
                        new ActionContext(mockContext.Object, _router, _action)
                    );
                }
            );

            endpoints.MapPost(
                Routes.PrivateVoidAction,
                async (context) =>
                {
                    Mock<HttpContext> mockContext = new();
                    mockContext.Setup(m => m.Response).Returns(context.Response);

                    IDictionary<string, string>? data = await GetBodyData(context);

                    ApiResponse response =
                        new(
                            () => {
                                /*does nothing*/
                            },
                            data,
                            _log
                        );

                    await response.ExecuteResultAsync(
                        new ActionContext(mockContext.Object, _router, _action)
                    );
                }
            );

            endpoints.MapPost(
                Routes.PrivateVoidActionAsync,
                async (context) =>
                {
                    Mock<HttpContext> mockContext = new();
                    mockContext.Setup(m => m.Response).Returns(context.Response);

                    IDictionary<string, string>? data = await GetBodyData(context);

                    ApiResponse response =
                        new(
                            async () =>
                                await Task.Run(() => {
                                    /*does nothing*/
                                }),
                            data,
                            _log
                        );

                    await response.ExecuteResultAsync(
                        new ActionContext(mockContext.Object, _router, _action)
                    );
                }
            );

            endpoints.MapGet(
                Routes.VoidAction,
                async (context) =>
                {
                    Mock<HttpContext> mockContext = new();
                    mockContext.Setup(m => m.Response).Returns(context.Response);

                    ApiResponse response =
                        new(
                            () => {
                                /*does nothing*/
                            },
                            _log
                        );

                    await response.ExecuteResultAsync(
                        new ActionContext(mockContext.Object, _router, _action)
                    );
                }
            );

            endpoints.MapGet(
                Routes.VoidActionAsync,
                async (context) =>
                {
                    Mock<HttpContext> mockContext = new();
                    mockContext.Setup(m => m.Response).Returns(context.Response);

                    ApiResponse response =
                        new(
                            async () =>
                                await Task.Run(() => {
                                    /*does nothing*/
                                }),
                            _log
                        );

                    await response.ExecuteResultAsync(
                        new ActionContext(mockContext.Object, _router, _action)
                    );
                }
            );
        });
    }

    private static async Task<IDictionary<string, string>?> GetBodyData(HttpContext context)
    {
        IDictionary<string, string>? data = null;

        if (context.Request.HasJsonContentType())
        {
            data = await context.Request.ReadFromJsonAsync<IDictionary<string, string>>();
        }

        return data;
    }
}
