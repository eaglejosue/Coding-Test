using Microsoft.AspNetCore.Http.HttpResults;

namespace CodingTest.Helpers.Middleware;

public class ExceptionHandleMiddleware(RequestDelegate next, ILogger<ExceptionHandleMiddleware> logger)
{
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ex, httpContext);
        }
    }

    private async Task HandleExceptionAsync(Exception ex, HttpContext httpContext)
    {
        logger.LogError(ex, ex.Message);
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var problemHttpResult = Results.Problem("Ocorreu um erro, tente novamente.") as ProblemHttpResult;
        await httpContext.Response.WriteAsJsonAsync(problemHttpResult!.ProblemDetails);
    }
}
