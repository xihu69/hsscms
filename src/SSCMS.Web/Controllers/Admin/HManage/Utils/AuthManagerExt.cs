using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SSCMS.Services
{
    public static class AuthManagerExt
    {

        /// <summary>
        ///默认  v-if='utils.isSa'
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="y"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        public static IHtmlContent IsSaStr(this IAuthManager auth,string y=null,string no=null) {
            if (y == null)
                y = " v-if='utils.isSa' ";
            if (no == null)
                no = " v-if='utils.isSa' ";
             if (auth.AdminName == "sa")
                return new HtmlString(y);
             else
                return new HtmlString(no);
        }
        public static IHtmlContent IfSaShow(this IAuthManager auth)
        {
            return new HtmlString($" ");
            // return " v-if='utils.isSa' ";v-if='utils.isSa' 
            //if (auth.AdminName == "sa")
            //    return " v-if='true' ";
            //return " v-if='false' ";
        }
        public static IHtmlContent IfSaShowAppend(this IAuthManager auth)
        {
            return new HtmlString($"utils.isSa&&");
            // return " v-if='utils.isSa' ";
            //if (auth.AdminName == "sa")
            //    return " v-if='true' ";
            //return " v-if='false' ";
        }
        public static IHtmlContent IfShow(this IHtmlHelper helper,string appendStr=null)
        {
            if (helper.ViewContext.HttpContext.Request.Path.Value.IndexOf("/cms/contents/") > -1)    //内容列表
            {

            }
            else if (helper.ViewContext.HttpContext.Request.Path.Value.IndexOf("/cms/editor/") > -1)    //内容编辑
            {
                var jsStr = "utils.isSa||(typeof(channel)!='undefined'&&!channel.groupNames.indexOf('资源书籍'))";
                if (appendStr == null)
                {
                    return new HtmlString($" v-if={jsStr}  "); //v-if=typeof(channel)!='undefined'&&!channel.groupNames.indexOf('资源书籍') //channelId
                }
                return new HtmlString($"{appendStr}{jsStr}"); //v-if=typeof(channel)!='undefined'&&!channel.groupNames.indexOf('资源书籍') //channelId 
            }
            return null;
           
        }

        public static bool AppendUserSite(Models.User user, List<int> siteIds)
        {
            //var user = await _authManager.GetUserAsync();
            if (user == null || user.SiteId < 1)
                return false;
            siteIds.Add(user.SiteId);
            return true;

        }

        /// <summary>
        /// 有站点权限，并且限制只能访问用户站点
        /// </summary>
        /// <param name="user"></param>
        /// <param name="siteIds"></param>
        /// <returns></returns>
        public static bool LimitUserSite(Models.User user, List<int> siteIds) {
            if (user == null || user.SiteId < 1)
                return false;
            if (siteIds.IndexOf(user.SiteId) >= 0)
            {
                siteIds.Clear();
                siteIds.Add(user.SiteId);
                return true;
            }
            return false;
        }

    }
}
