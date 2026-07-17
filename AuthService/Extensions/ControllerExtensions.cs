namespace AuthService.Extensions;

public static class ControllerExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller,Result<T> result)
    {
        var traceId = controller.HttpContext.TraceIdentifier;

        if (result.IsFailure)
        {
            return controller.BadRequest(new
            {
                errors = result.Errors,
                traceId
            });
        }

        return controller.Ok(new
        {
            data = result.Value,
            traceId
        });
    }
}