using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Adstruo.ApiResponse;

public class ApiInvalidModel : IActionResult
{
    private readonly ModelStateDictionary? _model = null;

    public ApiInvalidModel(ModelStateDictionary? model = null)
    {
        _model = model;
    }

    public virtual async Task ExecuteResultAsync(ActionContext context)
    {
        HttpResponse res = context.HttpContext.Response;

        ApiResult<ModelStateDictionary?> result = new();
        result.SetData(_model);
        result.SetError(new FailedToParseRequestException());

        await res.JsonSerializerAsync(result, result.Code);
    }
}
