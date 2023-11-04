using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Adstruo.ApiResponse;

/// <summary>Middleware to be set up in the InvalidModelStateResponseFactory</summary>
public class ApiInvalidModel : IActionResult
{
    private readonly ModelStateDictionary? _model = null;

    /// <summary>Creates a new model using ApiResult</summary>
    public ApiInvalidModel(ModelStateDictionary? model = null)
    {
        _model = model;
    }

    /// <summary>Sets the HttpResponse</summary>
    public virtual async Task ExecuteResultAsync(ActionContext context)
    {
        HttpResponse res = context.HttpContext.Response;

        ApiResult<ModelStateDictionary?> result = new();
        result.SetData(_model);
        result.SetError(new FailedToParseRequestException());

        await res.JsonSerializerAsync(result, result.Code);
    }
}
