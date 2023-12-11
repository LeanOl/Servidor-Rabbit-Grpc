using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace PurchasesServer.Controllers;

public class ExceptionFilter : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        int statusCode;

        if (context.Exception is ArgumentException or FormatException or IndexOutOfRangeException)
        {
            statusCode = StatusCodes.Status400BadRequest;
        }
        else
        {
            statusCode = StatusCodes.Status500InternalServerError;
        }

        context.Result = new ObjectResult(context.Exception.Message)
        {
            StatusCode = statusCode
        };


    }
}