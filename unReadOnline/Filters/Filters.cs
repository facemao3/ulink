using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO.Compression;

namespace unReadOnline.Filters
{
    #region 身份验证过滤器

    /// <summary>
    /// 使用FormsAuthentication来检查用户的身份验证
    /// 并在验证失败时重定向到登录页
    /// </summary>
    public class RequiresAuthenticationAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            //redirect if not authenticated
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {

                //use the current url for the redirect
                string redirectOnSuccess = filterContext.HttpContext.Request.Url.AbsolutePath;

                //send them off to the login page
                string redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                string loginUrl = FormsAuthentication.LoginUrl + redirectUrl;
                filterContext.HttpContext.Response.Redirect(loginUrl, true);

            }

        }
    }

    /// <summary>
    /// 使用FormsAuthentication来检查用户的角色
    /// 如果未授权的时候会抛出一个 UnauthorizedAccessException 异常
    /// </summary>
    public class RequiresRoleAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 要检查的角色，例如"Administrator"
        /// </summary>
        public string RoleToCheckFor { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //redirect if the user is not authenticated
            if (!String.IsNullOrEmpty(RoleToCheckFor))
            {

                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {

                    //use the current url for the redirect
                    string redirectOnSuccess = filterContext.HttpContext.Request.Url.AbsolutePath;

                    //send them off to the login page
                    string redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                    string loginUrl = FormsAuthentication.LoginUrl + redirectUrl;
                    filterContext.HttpContext.Response.Redirect(loginUrl, true);

                }
                else
                {
                    bool isAuthorized = filterContext.HttpContext.User.IsInRole(this.RoleToCheckFor);
                    if (!isAuthorized)
                    {
                        TempDataDictionary td = new TempDataDictionary(filterContext.HttpContext);
                        td["errorMsg"] = "你未被授权查看该页或未被授权进行该操作！";
                        filterContext.HttpContext.Response.Redirect("~/Error.mvc");
                        //throw new UnauthorizedAccessException("你未被授权查看该页！");
                    }
                }
            }
            else
            {
                TempDataDictionary td = new TempDataDictionary(filterContext.HttpContext);
                td["errorMsg"] = "没有指定具体的角色列表！";
                filterContext.HttpContext.Response.Redirect("~/Error.mvc");
                //throw new InvalidOperationException("没有指定具体的角色列表！");
            }
        }
    }

    #endregion

    #region 缓存与压缩过滤器

    /// <summary>
    /// 缓存过滤器
    /// </summary>
    public class CacheFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 以秒为单位获取或者设置缓存持续时间。默认为10秒
        /// </summary>
        /// <value>以秒为单位的缓存持续时间</value>
        public int Duration
        {
            get;
            set;
        }

        public CacheFilterAttribute()
        {
            Duration = 10;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (Duration <= 0) return;

            HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
            TimeSpan cacheDuration = TimeSpan.FromSeconds(Duration);

            cache.SetCacheability(HttpCacheability.Public);
            cache.SetExpires(DateTime.Now.Add(cacheDuration));
            cache.SetMaxAge(cacheDuration);
            cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
        }
    }

    /// <summary>
    /// 压缩过滤器，GZIP
    /// </summary>
    public class CompressFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;

            string acceptEncoding = request.Headers["Accept-Encoding"];

            if (string.IsNullOrEmpty(acceptEncoding)) return;

            acceptEncoding = acceptEncoding.ToUpperInvariant();

            HttpResponseBase response = filterContext.HttpContext.Response;

            if (acceptEncoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
            else if (acceptEncoding.Contains("DEFLATE"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
        }
    }

    #endregion
}
