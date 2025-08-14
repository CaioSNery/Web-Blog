using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.Atributte
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAtributte : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Query.TryGetValue(Configuration.ApiKeyName, out var extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Api não encontrada"
                };
                return;
            }

            if (!Configuration.ApiKey.Equals(extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 403,
                    Content = "Acesso não autorizado"
                };

                await next();

                return;


            }
        }
    }
}