using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using System.Data.SqlClient;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace AppZeroAPI.Middleware
{
    public enum TraceCategory
    {
        TokenProvider,
        APIException

    }
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        IHostEnvironment env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            this.logger = logger;
            this.next = next;
            this.env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            
            bool allowTraceDev = env.IsDevelopment();
            allowTraceDev = false;
            var message =   ((allowTraceDev) ? exception.StackTrace : exception.Message); 
            
            var  statuscode = GetStatusCode(exception);
            if (exception.InnerException != null)
            {
                logger.LogError(exception, "Inner Exception : " + exception.InnerException.Message);
                 message =  message + (allowTraceDev  ? exception.InnerException.StackTrace : exception.InnerException.Message);
            }

            if (exception is SqlException)
            {
                //resp = AppResponse.BadRequest("Server Error");
                logger.LogError(exception, "Sql Exception : " + exception.Message);
            }
            else if (exception is AppException)
            {
                statuscode =  400;
                logger.LogError(exception, "AppException : " + exception.Message);
            } 
            else
            {
                logger.LogError(exception, "Unknown Exception : " + exception.Message);
               
            } 
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statuscode; 
            var resp = AppResponse.SystemError(message); 
            return context.Response.WriteAsync(JsonConvert.SerializeObject(resp, Formatting.Indented));
        }
        

        private int GetStatusCode(Exception exception)
        {
            switch (exception.GetType().Name)
            {
                case nameof(NotFoundException):
                    return 404;
                default:
                    return 500;

            }

        }

    }

    public class NotFoundException : Exception
    {
        public NotFoundException() : base()
        {
        }

        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}