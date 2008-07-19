using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using unReadOnline.Models.Entitys;
using unReadOnline.ViewsData;
using System.Web.Routing;
using System.Text.RegularExpressions;

namespace unReadOnline.Controllers
{
    public class UserController : Controller
    {
        public void Index()
        {
            // Add action logic here
        }

        /// <summary>
        /// 注册
        /// </summary>
        public ActionResult Register()
        {
            string userName = Request.Form["userName"];
            string password = Request.Form["password"];
            string email = Request.Form["email"];
            unReadOnline.ViewsData.JsonResult jsonResult = new unReadOnline.ViewsData.JsonResult();
            MembershipUser newUser;
            if (userName == null || password == null || email == null)
            {
                jsonResult.isSuccessful = false;
                jsonResult.errorMessage = "用户名、密码、Email不能为空！";
            }
            else if (!ValEmail(email))
            {
                jsonResult.isSuccessful = false;
                jsonResult.errorMessage = "Email 格式不对！";
            }
            else if (UserIsExist(userName))
            {
                jsonResult.isSuccessful = false;
                jsonResult.errorMessage = "改用户名已经被注册，请选择另外一个用户名！";
            }
            else
            {
                newUser = Membership.CreateUser(userName, password, email);
                jsonResult.isSuccessful = true;
                HttpCookie cookie = new HttpCookie("AdduLink");
                cookie.Value = "true";
                Response.Cookies.Add(cookie);
                if (Request.Form["ReturnUrl"] != null)
                {
                    FormsAuthentication.RedirectFromLoginPage(userName, false);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(userName, false);
                }
            }

            return View("Ajax", jsonResult);
        }

        /// <summary>
        /// 登录
        /// </summary>
        public ActionResult Login()
        {
            unReadOnline.ViewsData.JsonResult jsonResult = new unReadOnline.ViewsData.JsonResult();
            string userName = Request.Form["userName"];
            string password = Request.Form["password"];
            bool rememberMe = Convert.ToBoolean(Request.Form["rememberMe"]);
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                jsonResult.isSuccessful = false;
                jsonResult.errorMessage = "用户名或者密码不能为空！";
            }
            else
            {
                if (Membership.ValidateUser(userName, password))
                {
                    HttpCookie cookie = new HttpCookie("AdduLink");
                    cookie.Value = "true";
                    Response.Cookies.Add(cookie);
                    if (Request.Form["ReturnUrl"] != null)
                    {

                        FormsAuthentication.RedirectFromLoginPage(userName, rememberMe);

                    }

                    else
                    {

                        FormsAuthentication.SetAuthCookie(userName, rememberMe);

                    }
                    jsonResult.isSuccessful = true;
                    string myULink = @"<a href='/PEuLink/MyULink.mvc'>我的友链</a>";
                    jsonResult.returnHtml = myULink;
                }
                else
                {
                    jsonResult.isSuccessful = false;
                    jsonResult.errorMessage = "用户名或者密码错误,请重新登录";
                }
            }
            return View("Ajax", jsonResult);
        }

        /// <summary>
        /// 注销用户
        /// </summary>
        public ActionResult Logout()
        {   
            unReadOnline.ViewsData.JsonResult jr = new unReadOnline.ViewsData.JsonResult();
            try
            {
                FormsAuthentication.SignOut();
                jr.isSuccessful = true;
                HttpCookie cookie = new HttpCookie("AdduLink");
                cookie.Value = "false";
                cookie.Expires = DateTime.Now.AddDays(-30);
                Response.Cookies.Add(cookie);
            }
            catch (Exception)
            {
                jr.isSuccessful = false;
            }

            return View("Ajax", jr);
        }

        /// <summary>
        /// 验证Email格式是否正确
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private bool ValEmail(string email)
        {
            Regex regEmail = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            return regEmail.IsMatch(email);
        }

        /// <summary>
        /// 检查用户是否存在，即该用户名是否已经被注册
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private bool UserIsExist(string userName)
        {
            if (Membership.GetUser(userName) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
