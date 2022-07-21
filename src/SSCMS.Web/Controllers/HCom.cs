using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace SSCMS.Web.Controllers
{
    internal class HCom
    {
        /// <summary>
        /// 是否跳过权限验证，用于详情页直接访问，todo 需要实现简单验证，防止任意访问
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public static bool strictCheck(ControllerBase controller, int? siteId = null)
        {
            var token = AuthManager_ApiToken(controller);
            return false;
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
