using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);

            switch (context.Response.StatusCode)
            {
                case 400:
                    await WriteError(context, 400, "Błąd żądania (Bad Request).");
                    break;
                case 401:
                    await WriteError(context, 401, "Brak autoryzacji. Zaloguj się, aby uzyskać dostęp.");
                    break;
                case 403:
                    await WriteError(context, 403, "Brak uprawnień do wykonania tej operacji.");
                    break;
                case 404:
                    await WriteError(context, 404, "Nie znaleziono zasobu.");
                    break;
                case 500:
                    await WriteError(context, 500, "Wewnętrzny błąd serwera.");
                    break;
            }
        }
        catch (Exception ex)
        {
            await WriteError(context, 500, "Wystąpił nieoczekiwany błąd: " + ex.Message);
        }
    }

    private async Task WriteError(HttpContext context, int statusCode, string message)
    {
        if (context.Response.HasStarted) return;
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        var result = JsonSerializer.Serialize(new { error = message });
        await context.Response.WriteAsync(result);
    }
}