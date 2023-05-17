using Common.Common;
using Common.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Application.Interfaces;
using WebApi.Constants;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebApi.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private const string Prefix = ":";
        public ErrorHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ErrorHandlerMiddleware>();
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuthenticatedUserService authenticatedUser)
        {
            try
            {
                await _next(context);
                WriteLog(context, authenticatedUser);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new Response() { Code = Code.ServerError, Message = error.Message};

                switch (error)
                {
                    case ValidationException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(responseModel);

                await response.WriteAsync(result);
            }
        }
        public void WriteLog(HttpContext context, IAuthenticatedUserService authenticatedUser)
        {
            string controllerName = context.Request.RouteValues["controller"]?.ToString();
            string actionName = context.Request.RouteValues["action"]?.ToString();
            string methodName = context.Request.Method;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var username = authenticatedUser.UserName;
            var userId = authenticatedUser.UserId;
            using StreamReader r = new StreamReader("ModuleDefine.json");
            string json = r.ReadToEnd();
            r.Close();
            List<ModuleModel> listModule = JsonConvert.DeserializeObject<List<ModuleModel>>(json);
            var actionPrefix = methodName.StandardizedTextLowerCase() + Prefix +
                               actionName.StandardizedTextLowerCase();
            foreach (var item in listModule)
            {
                if (item.Key.StandardizedTextLowerCase() == controllerName.StandardizedTextLowerCase())
                {
                    foreach (var val in item.Value)
                    {
                        if (val.Key.StandardizedTextLowerCase() == actionPrefix)
                        {
                            _logger.LogInformation(LogConstants.TemplateMessage, methodName, val.Value.StandardizedTextLowerCase(), ipAddress, userId, username);
                        }
                    }
                }
            }
        }
        private static IDictionary<string, string> ConvertKeysToLowerCase(
            IDictionary<string, string> dictionaries)
        {
            var convertedDictionatry = new Dictionary<string, string>();
            foreach (string key in dictionaries.Keys)
            {
                convertedDictionatry.Add(key.ToLower(), dictionaries[key]);
            }
            return convertedDictionatry;
        }
    }
}
