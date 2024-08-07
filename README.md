# Adstruo.ApiResponse [![All Tests](https://github.com/raibtoffoletto/adstruo-apiresponse/actions/workflows/run-tests.yml/badge.svg)](https://github.com/raibtoffoletto/adstruo-apiresponse/actions/workflows/run-tests.yml) [![Publish NuGet](https://github.com/raibtoffoletto/adstruo-apiresponse/actions/workflows/publish.yml/badge.svg)](https://github.com/raibtoffoletto/adstruo-apiresponse/actions/workflows/publish.yml)

> Adstruo _(from the latin: to add, to contribute)_ is a name I use for different support packages I create in different languages and contexts. It's from far a coherent project namespace =)

This `dotNet` package provides a consistent structured json response for APIs. It includes a high order class derived from `IActionResult` to be used in the project's controllers, and a set of specific error messages, each with it's proper http code, description and key.

## Installation

You can easily install the package by using the CLI:

```bash
dotnet add package Adstruo.ApiResponse
```

More details in the official [nuget.org](https://www.nuget.org/packages/Adstruo.ApiResponse) page.

## Usage

### Api Result

The structured result is always composed by a `ApiResultStatus status` and a standard HTTP `int code`. Optionally, data can be appended to the `T? data` property and in case of an error, the properties `error` and `message` will be attached with an error's key and a description respectively.

```cs
class ApiResultStatus
{
    string Ok = "ok";
    string Error = "error";
}

class ApiResult<T> {
    T? data
    string status;
    int code;
    string? error;
    string? message;
}
```

### Error keys

A set of error keys to be used as identifier for the exception (util to translate error messages in the front-end for example):

- ALL_FIELDS_REQUIRED
- BAD_REQUEST
- FAILED_TO_PARSE_REQUEST
- FORBIDDEN
- INVALID_USER_CREDENTIALS
- NETWORK_ERROR
- NOT_FOUND
- UNAUTHENTICATED
- UNAUTHORIZED
- UNKNOWN

Every error code has its own `Exception` implementation with the key, a description and the standard HTTP code attached to it. The error constructors also accept the standard `string? message` argument for custom descriptions.

E.G.: throwing an AllFieldsRequiredException (`throw new AllFieldsRequiredException();`) will result in the following api response:

```json
{
  "status": "error",
  "code": 406,
  "error": "ALL_FIELDS_REQUIRED",
  "message": "The required fields for this method were not provided"
}

// HTTP status code: 406
// HTTP status text: Not Acceptable
```

### Api Response

The basic `ApiResponse` can be returned directly from the controller's method call. The constructor requires a `ILogger` implementation and assumes the route is public (or the authentication is being managed by the framework's router).

If the authentication is handled by a middleware, a second nullable parameter `object? session` can be passed to authenticate the user's access to the route. In case of null an `UnauthenticatedException` will be thrown.

To return any kind of data an `ApiResponse<T>` instance should be returned. It requires an `Func<T>` (asynchronous or not), any internal error will be automatically handled, or the `T data` will be returned.

```cs
    private readonly ILogger _logger;
    ...
    /// <summary>Public heartbeat check</summary>
    [HttpGet()]
    public ApiResponse Get() => new ApiResponse(_logger);

    /// <summary>Public api version check</summary>
    [HttpGet("version")]
    public ApiResponse<string> Get()
    {
        return new ApiResponse<string>(
          () => "1.0.0",
          _logger
        );
    }

    /// <summary>Private web hook</summary>
    [HttpGet("web-hook")]
    public ApiResponse Get()
    {
        MySession? session = GetMyValidSession();

        return new ApiResponse(_logger, session);
    }

    /// <summary>Private user's profile</summary>
    [HttpGet("user/profile")]
    public ApiResponse<UserProfile> Get()
    {
        MySession? session = GetMyValidSession();

        return new ApiResponse<UserProfile>(
          async () => {
            UserProfile? profile = await db.getUserProfile(session!.id);

            if (profile == null) {
              throw new NotFoundException("User profile not found");
            }

            return profile;
          },
          _logger,
          session
        );
    }
```

### Invalid Model Configuration

> To keep compatibility through all api responses, an `IActionResult ApiInvalidModel` class is provided and should be configured in the services set-up. This will wrap any error thrown about invalid models.

```cs
(IServiceCollection services) => {
  services.AddControllersWithInvalidModel();
}
```

### Catch Error Middleware

> To catch all thrown errors and keep the response coherent, a middleware needs to be inject into the app builder.

```cs
(IApplicationBuilder app) => {
  app.UseApiExceptionMiddleware();
}
```
