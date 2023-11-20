using OohelpWebApps.Software.Server.Exceptions;

namespace OohelpWebApps.Software.Server.Mapping;
public static class ExceptionToIResult
{
    public static IResult ToApiErrorResult(this Exception exception)
    {
        if (exception is ApiException api)
        {
            return api.Reason switch
            {
                ExceptionReason.NotFound => Results.NotFound(),
                _ => Results.Problem($"{api.Reason}: {api.Message}")
            };
        }
        return Results.Problem(exception.Message);
    }
}
