using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Xunit;

namespace Adstruo.ApiResponse.Tests;

public class Integration
{
    private readonly TestServer _server;
    private readonly HttpClient _client;

    public Integration()
    {
        _server = new TestServer(new WebHostBuilder().UseStartup<Server>());
        _client = _server.CreateClient();
    }

    [Theory]
    [InlineData(Routes.Void)]
    [InlineData(Routes.VoidAction)]
    [InlineData(Routes.VoidActionAsync)]
    public async Task VoidRoute_IsSuccessful(string route)
    {
        HttpResponseMessage response = await _client.GetAsync(route);

        VerifyResponse(response);

        ApiResult result = await ParseContent(response);

        VerifyResult(result, response, ApiResultStatus.Ok);
    }

    [Theory]
    [InlineData(Routes.GenericError)]
    [InlineData(Routes.VoidActionError)]
    public async Task GenericErrorRoute_ReturnsError(string route)
    {
        HttpResponseMessage response = await _client.GetAsync(route);

        VerifyResponse(response, 400);

        ApiResult result = await ParseContent(response);

        VerifyResult(result, response, ApiResultStatus.Error);
        VerifyContent(
            result,
            ApiExceptionError.UNKNOWN,
            "Exception of type 'System.Exception' was thrown."
        );
    }

    [Fact]
    public async Task UnknownErrorRoute_ReturnsError()
    {
        HttpResponseMessage response = await _client.GetAsync(Routes.UnknownError);

        VerifyResponse(response, 400);

        ApiResult result = await ParseContent(response);

        VerifyResult(result, response, ApiResultStatus.Error);
        VerifyContent(result, ApiExceptionError.UNKNOWN, UnknownException.Description);
    }

    [Fact]
    public async Task DataRoute_IsSuccessful()
    {
        IDictionary<string, string> data = new Dictionary<string, string>
        {
            { "id", Guid.NewGuid().ToString() },
            { "timestamp", DateTime.UtcNow.ToString() }
        };

        HttpResponseMessage response = await _client.PostAsync(
            Routes.Data,
            JsonContent.Create(data)
        );

        VerifyResponse(response);

        ApiResult<IDictionary<string, string>> result = await ParseContent(response);

        VerifyResult(result, response, ApiResultStatus.Ok);
        Assert.Equivalent(result.Data, data, true);
    }

    [Fact]
    public async Task DataRoute_FailsWithoutData()
    {
        HttpResponseMessage response = await _client.PostAsync(Routes.Data, null);

        VerifyResponse(response, 406);

        ApiResult<IDictionary<string, string>> result = await ParseContent(response);

        VerifyResult(result, response, ApiResultStatus.Error);
        VerifyContent(
            result,
            ApiExceptionError.ALL_FIELDS_REQUIRED,
            AllFieldsRequiredException.Description
        );
    }

    [Theory]
    [MemberData(nameof(ErrorData))]
    public async Task ErrorRoute_Fails(ApiException exception, string key)
    {
        HttpResponseMessage response = await _client.PostAsync(
            Routes.Error,
            JsonContent.Create(new { error = key })
        );

        VerifyResponse(response, exception.Code);

        ApiResult<IDictionary<string, string>> result = await ParseContent(response);

        VerifyResult(result, response, ApiResultStatus.Error);
        VerifyContent(result, key, exception.Message);
    }

    [Fact]
    public async Task InvalidModelRoute_Fails()
    {
        HttpResponseMessage response = await _client.GetAsync(Routes.InvalidModel);

        VerifyResponse(response, 422);

        ApiResult result = await ParseContent(response);

        VerifyResult(result, response, ApiResultStatus.Error);
        VerifyContent(
            result,
            ApiExceptionError.FAILED_TO_PARSE_REQUEST,
            FailedToParseRequestException.Description
        );
    }

    [Fact]
    public async Task PrivateVoidRoute_Succeeds()
    {
        HttpResponseMessage response = await _client.PostAsync(
            Routes.PrivateVoid,
            JsonContent.Create(new { authorized = "yes" })
        );

        VerifyResponse(response);

        ApiResult result = await ParseContent(response);

        VerifyResult(result, response, ApiResultStatus.Ok);
    }

    [Fact]
    public async Task PrivateVoidRoute_FailsWhenUnauthorized()
    {
        HttpResponseMessage response = await _client.PostAsync(Routes.PrivateVoid, null);

        VerifyResponse(response, 401);

        ApiResult result = await ParseContent(response);

        VerifyResult(result, response, ApiResultStatus.Error);
        VerifyContent(
            result,
            ApiExceptionError.UNAUTHENTICATED,
            UnauthenticatedException.Description
        );
    }

    [Theory]
    [InlineData(Routes.PrivateData)]
    [InlineData(Routes.PrivateDataAsync)]
    [InlineData(Routes.PrivateVoidAction, false)]
    [InlineData(Routes.PrivateVoidActionAsync, false)]
    public async Task PrivateDataRoute_Succeeds(string route, bool hasResponseData = true)
    {
        IDictionary<string, string> data = new Dictionary<string, string>
        {
            { "id", Guid.NewGuid().ToString() },
            { "timestamp", DateTime.UtcNow.ToString() }
        };

        HttpResponseMessage response = await _client.PostAsync(route, JsonContent.Create(data));

        VerifyResponse(response);

        ApiResult<IDictionary<string, string>> result = await ParseContent(response);

        VerifyResult(result, response, ApiResultStatus.Ok);

        if (hasResponseData)
        {
            Assert.Equivalent(result.Data, data, true);
        }
    }

    [Fact]
    public async Task PrivateDataRoute_FailsWhenUnauthorized()
    {
        HttpResponseMessage response = await _client.PostAsync(Routes.PrivateData, null);

        VerifyResponse(response, 401);

        ApiResult result = await ParseContent(response);

        VerifyResult(result, response, ApiResultStatus.Error);
        VerifyContent(
            result,
            ApiExceptionError.UNAUTHENTICATED,
            UnauthenticatedException.Description
        );
    }

    private static async Task<ApiResult<IDictionary<string, string>>> ParseContent(
        HttpResponseMessage response
    )
    {
        return JsonConvert.DeserializeObject<ApiResult<IDictionary<string, string>>>(
                await response.Content.ReadAsStringAsync()
            ) ?? throw new Exception("Impossible to deserialize api response body");
    }

    private static void VerifyResponse(HttpResponseMessage response, int status = 200)
    {
        Assert.True((int)response.StatusCode == status, "Status code is correct");

        string contentType =
            response.Content.Headers.GetValues(HeaderNames.ContentType).FirstOrDefault()
            ?? throw new Exception("Response content doesn't have headers");

        Assert.True(
            contentType.Contains("application/json") && contentType.Contains("utf-8"),
            "Content headers is correct"
        );
    }

    private static void VerifyResult(ApiResult result, HttpResponseMessage response, string status)
    {
        Assert.True(result.Status == status, "Response body is correct");
        Assert.True(result.Code == (int)response.StatusCode, "Status codes are the same");
    }

    private static void VerifyContent(ApiResult result, string error, string message)
    {
        Assert.True(result.Error == error, "Error key is correct");
        Assert.True(result.Message == message, "Error message is correct");
    }

#pragma warning disable CA2211
    public static TheoryData<ApiException, string> ErrorData =
        new()
        {
            { new AllFieldsRequiredException(), ApiExceptionError.ALL_FIELDS_REQUIRED },
            { new BadRequestException(), ApiExceptionError.BAD_REQUEST },
            { new FailedToParseRequestException(), ApiExceptionError.FAILED_TO_PARSE_REQUEST },
            { new ForbiddenException(), ApiExceptionError.FORBIDDEN },
            { new InvalidUserCredentialsException(), ApiExceptionError.INVALID_USER_CREDENTIALS },
            { new NetworkErrorException(), ApiExceptionError.NETWORK_ERROR },
            { new NotFoundException(), ApiExceptionError.NOT_FOUND },
            { new UnauthenticatedException(), ApiExceptionError.UNAUTHENTICATED },
            { new UnauthorizedException(), ApiExceptionError.UNAUTHORIZED },
            { new UnknownException(), ApiExceptionError.UNKNOWN },
        };
}
