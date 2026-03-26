using System.Security.Claims;

namespace ToDoList.Api.Middleware;

/// <summary>
/// Runs after authentication. For every authenticated request, parses the
/// NameIdentifier claim into an integer and stores it in HttpContext.Items["UserId"].
/// Returns 401 immediately if the claim is absent or not a valid integer — preventing
/// a downstream FormatException / NullReferenceException from leaking as a 500.
/// </summary>
public class UserIdMiddleware(RequestDelegate next)
{
    public const string UserIdKey = "UserId";

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var value = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(value, out var userId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
            context.Items[UserIdKey] = userId;
        }

        await next(context);
    }
}
