using System.Net;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Api.ExceptionHandling;
public class ExceptionHandler {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandler> _logger;
    
    public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context) 
    {
        try 
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");
            
            await HandleExceptionAsync(context, ex);
        }
    }
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var response = new {
            status = context.Response.StatusCode,
            message = "An unexpected error occurred."
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
