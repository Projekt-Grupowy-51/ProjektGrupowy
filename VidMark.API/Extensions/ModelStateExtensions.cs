using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VidMark.API.Extensions;

public static class ModelStateExtensions
{
    public static object GetErrorsAsResponse(this ModelStateDictionary modelState)
    {
        var errors = modelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new
            {
                Field = x.Key,
                Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
            })
            .ToList();

        return new
        {
            Status = "Error",
            Message = "Invalid model state.",
            Errors = errors
        };
    }
}
