using Microsoft.AspNetCore.Http;
using NetCore31.Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore31.Demo.Utility
{
    public static class CookieSessionHelper
    {
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="minutes"></param>
        public static void SetCookies(this HttpContext httpContext, string key, string value, int minutes = 30)
        {
            httpContext.Response.Cookies.Append(key, value, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(minutes)
            });
        }

        /// <summary>
        /// 移除Cookie
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="key"></param>
        public static void DeleteCookies(this HttpContext httpContext, string key)
        {
            httpContext.Response.Cookies.Delete(key);
        }

        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetCookiesValue(this HttpContext httpContext, string key)
        {
            httpContext.Request.Cookies.TryGetValue(key, out string value);
            return value;
        }

        /// <summary>
        /// 从session中获取当前用户
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static CurrentUser GetCurrentUserBySession(this HttpContext context)
        {
            string sUser = context.Session.GetString("CurrentUser");
            if (sUser == null)
            {
                return null;
            }
            else
            {
                CurrentUser currentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<CurrentUser>(sUser);
                return currentUser;
            }
        }
    }
}
