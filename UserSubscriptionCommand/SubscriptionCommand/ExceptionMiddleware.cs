namespace SubscriptionCommand;

public class ExceptionMiddleware

{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.Message);  
            Console.WriteLine(ex.InnerException);

        }
    }
}
