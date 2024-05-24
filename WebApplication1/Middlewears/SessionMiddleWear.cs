using Microsoft.AspNetCore.Authentication;


public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;
    //Constructor: It takes a RequestDelegate as a parameter, which is a delegate that represents the next middleware in the pipeline.
    public SessionValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    //The HttpContext parameter provides access to the current HTTP request.
    public async Task InvokeAsync(HttpContext context)
    {
        // to Check if the user is authenticated add this
        //if (context.User.Identity.IsAuthenticated)
        //{

            // Check if the user is authenticated
            if (context.Request.Cookies.TryGetValue("Mysessioncookie", out string cookieValue))
        {
            var userId = context.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                // If user ID is not found in session, sign out and redirect to login page
                await context.SignOutAsync();
                return ;
            }
            }

        // Call the next middleware in the pipeline
        await _next(context);
    }
}
