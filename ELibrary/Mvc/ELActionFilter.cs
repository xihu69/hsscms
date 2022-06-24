using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELibrary.Mvc
{
    public class ELActionFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Action 执行后拦截
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            var actionArguments = context.HttpContext.Items["__ActionArguments"] as IDictionary<string, object?>;
            var actionInfo=context.ActionDescriptor as ControllerActionDescriptor;
            if (actionInfo == null)
                return; 
            if (actionInfo.ControllerTypeInfo.FullName==""&&actionInfo.ActionName=="")
            Console.Write(context.RouteData);
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
           context.HttpContext.Items.Add("__ActionArguments", context.ActionArguments);
        }
    }
}
