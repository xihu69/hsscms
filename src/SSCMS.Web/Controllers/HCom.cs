using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace SSCMS.Web.Controllers
{
    internal class HCom
    {
        public static bool strictCheck(ControllerBase controller, int? siteId = null)
        {
            var token = AuthManager_ApiToken(controller);
            if (string.IsNullOrWhiteSpace(token))
                return true;
            string sid = siteId == null ? null : siteId + "";
            if (sid == null && controller.HttpContext.Request.Method == "GET")
            {
                sid = controller.RouteData.Values["siteId"] + "";
                if (string.IsNullOrEmpty(sid))
                    sid = controller.HttpContext.Request.Query["siteId"];
            }
            if (string.IsNullOrEmpty(sid))
                return true;
            if (token.EndsWith(siteId + ""))
                return false;
            return true;
        }
        /// <summary>
        /// 获取apitoken
        /// </summary>
        /// <param name="controller"></param>
        /// <see cref="SSCMS.Core.Services.AuthManager.ApiToken"/>
        /// <returns></returns>
        public static string AuthManager_ApiToken(ControllerBase controller)
        {
            var _context = controller;
                if (_context.HttpContext.Request.Query.TryGetValue("apiKey", out var queries))
                {
                    var token = queries.SingleOrDefault();
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        return token;
                    }
                }
                if (_context.HttpContext.Request.Headers.TryGetValue("X-SS-API-KEY", out var headers))
                {
                    var token = headers.SingleOrDefault();
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        return token;
                    }
                }
                return null;
        }
    }
}
