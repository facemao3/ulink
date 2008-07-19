using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using unReadOnline.Filters;
using unReadOnline.ViewsData;
using unReadOnline.Models.Entitys;
using System.Web.Security;
using System.IO;

namespace unReadOnline.Controllers
{
    public class MyULinkController : Controller
    {
        /// <summary>
        /// 我的uLink首页
        /// 显示我的uLink列表
        /// </summary>
        /// <returns></returns>
        [RequiresAuthentication]
        public ActionResult Index()
        {
            BaseUnReadLinkData viewData = new BaseUnReadLinkData();
            MembershipUser user = Membership.GetUser();
            viewData.CurrentPage = 1;
            viewData.UnReadLinkPerPage = 10;
            int totalCount;
            viewData.UnReadLinks = UnReadLink.GetUnReadLinksByUser(Convert.ToInt32(user.ProviderUserKey), viewData.CurrentPage, viewData.UnReadLinkPerPage, out totalCount);
            viewData.TotalUnReadLinks = totalCount;
            return View("Index", viewData);
        }

        /// <summary>
        /// 添加一条新的uLink
        /// </summary>
        /// <returns></returns>
        [RequiresAuthentication]
        public ActionResult Add()
        {
            unReadOnline.ViewsData.JsonResult jsonResult = new unReadOnline.ViewsData.JsonResult();
            try
            {
                string title = Request.Form["Title"];
                string url = Request.Form["URL"];
                string description = Request.Form["Description"];
                string category = Request.Form["Category"];
                string isPublic = Request.Form["Public"];
                string isForever = Request.Form["Forever"];
                UnReadLink link = new UnReadLink();
                link.Title = title;
                link.Url = url.Contains(@"http://") ? url : @"http://" + url;
                link.Description = description;
                link.UserId = Convert.ToInt32(Membership.GetUser().ProviderUserKey);
                link.CategoryId = Convert.ToInt32(category);
                link.IsPublic = Convert.ToBoolean(isPublic);
                link.IsForever = Convert.ToBoolean(isForever);
                link.Save();
                jsonResult.isSuccessful = true;
            }
            catch (Exception e)
            {
                jsonResult.isSuccessful = false;
                jsonResult.errorMessage = e.Message;
            }

            return View("Ajax", jsonResult);
        }

        /// <summary>
        /// 加载添加uLink的表单
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadAddForm()
        {
            unReadOnline.ViewsData.JsonResult jsonResult = new unReadOnline.ViewsData.JsonResult();

            if (ControllerContext.HttpContext.User.Identity.IsAuthenticated)
            {
                jsonResult.isSuccessful = true;
                using (StreamReader srHtml = new StreamReader(HttpRuntime.AppDomainAppPath + "/Views/MyuLink/AdduLink.htm"))
                {
                    jsonResult.returnHtml = srHtml.ReadToEnd();
                }
            }
            else
            {
                jsonResult.isSuccessful = false;
                jsonResult.errorMessage = "你还没有登录，请先登录或注册！";
            }

            return View("Ajax", jsonResult);
        }

        /// <summary>
        /// 删除指定ID的uLink
        /// </summary>
        /// <param name="id">uLink的ID</param>
        /// <param name="synType">是否为异步(AJAX)请求</param>
        /// <returns></returns>
        [RequiresAuthentication]
        public ActionResult Delete(int id)
        {
            unReadOnline.ViewsData.JsonResult jsonResult = new unReadOnline.ViewsData.JsonResult();
            string synType = Request.Form["synType"] ?? "";
            UnReadLink ul = UnReadLink.Load(id);
            if (ul != null)
            {
                MembershipUser user = Membership.GetUser();
                if (ul.UserId == Convert.ToInt32(user.ProviderUserKey))
                {
                    ul.Delete();
                    if (ul.Save() == SaveAction.Delete)
                    {
                        jsonResult.isSuccessful = true;
                    }
                    else
                    {
                        jsonResult.isSuccessful = false;
                        jsonResult.errorMessage = "发生未知错误，删除失败！";
                    }
                }
                else
                {
                    jsonResult.isSuccessful = false;
                    jsonResult.errorMessage = "指定ID的uLink不是属于你的，你不可以删除别人的uLink！";
                }
            }
            else
            {
                jsonResult.isSuccessful = false;
                jsonResult.errorMessage = "指定ID的uLink不存在，删除失败！";
            }
            if (synType.ToLower() != "ajax")
            {
                if (jsonResult.isSuccessful)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["errorMsg"] = jsonResult.errorMessage;
                    return RedirectToAction("Index", "Error");
                }
            }
            else
            {
                return View("Ajax", jsonResult);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="synType"></param>
        /// <returns></returns>
        [RequiresAuthentication]
        public ActionResult Edit(int id, string synType)
        {
            unReadOnline.ViewsData.JsonResult jsonResult = new unReadOnline.ViewsData.JsonResult();

            UnReadLink ul = UnReadLink.Load(id);
            if (ul != null)
            {

            }
            if (synType.ToLower() != "ajax")
            {
                if (jsonResult.isSuccessful)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["errorMsg"] = jsonResult.errorMessage;
                    return RedirectToAction("Index", "Error");
                }
            }
            else
            {
                return View("Ajax", jsonResult);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="synType"></param>
        /// <returns></returns>
        [RequiresAuthentication]
        public ActionResult Update(int id, string synType)
        {
            unReadOnline.ViewsData.JsonResult jsonResult = new unReadOnline.ViewsData.JsonResult();

            UnReadLink ul = UnReadLink.Load(id);
            if (ul != null)
            {

            }
            if (synType.ToLower() != "ajax")
            {
                if (jsonResult.isSuccessful)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["errorMsg"] = jsonResult.errorMessage;
                    return RedirectToAction("Index", "Error");
                }
            }
            else
            {
                return View("Ajax", jsonResult);
            }
        }
    }
}
