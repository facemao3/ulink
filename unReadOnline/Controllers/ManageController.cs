using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using unReadOnline.Filters;
using unReadOnline.Models.Entitys;
using System.Web.Security;

namespace unReadOnline.Controllers
{
    public class ManageController : Controller
    {
        public ActionResult Index()
        {
            // Add action logic here
            throw new NotImplementedException();
        }

        [RequiresRole(RoleToCheckFor = "Administrators")]
        public ActionResult DelULink(int id)
        {
            unReadOnline.ViewsData.JsonResult jsonResult = new unReadOnline.ViewsData.JsonResult();
            string synType = Request.Form["synType"] ?? "";
            UnReadLink ul = UnReadLink.Load(id);
            if (ul != null)
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
                jsonResult.errorMessage = "指定ID的uLink不存在，删除失败！";
            }
            if (synType.ToLower() != "ajax")
            {
                if (jsonResult.isSuccessful)
                {
                    return RedirectToAction("Index","Home");
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

        [RequiresRole(RoleToCheckFor = "Administrators")]
        public ActionResult EditULink(int id)
        {
            unReadOnline.ViewsData.JsonResult jsonResult = new unReadOnline.ViewsData.JsonResult();
            string synType = Request.Form["synType"] ?? "";
            UnReadLink ul = UnReadLink.Load(id);
            if (ul != null)
            {
                
                if (ul.Save() == SaveAction.Update)
                {
                    jsonResult.isSuccessful = true;
                }
                else
                {
                    jsonResult.isSuccessful = false;
                    jsonResult.errorMessage = "发生未知错误，更新失败！";
                }
            }
            else
            {
                jsonResult.isSuccessful = false;
                jsonResult.errorMessage = "指定ID的uLink不存在，更新失败！";
            }
            if (synType.ToLower() != "ajax")
            {
                if (jsonResult.isSuccessful)
                {
                    return RedirectToAction("Index","Home");
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
