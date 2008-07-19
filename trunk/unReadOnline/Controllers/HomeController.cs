using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using unReadOnline.Models.Entitys;
using unReadOnline.ViewsData;
using System.Web.Security;

namespace unReadOnline.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            BaseUnReadLinkData viewData = new BaseUnReadLinkData();
            viewData.CurrentPage = 1;
            viewData.UnReadLinkPerPage = 20;
            int totalCount;
            viewData.UnReadLinks = UnReadLink.GetUnReadLinks(viewData.UnReadLinkPerPage, viewData.CurrentPage, out totalCount);
            viewData.TotalUnReadLinks = totalCount;
            return View("Index",viewData);
        }

        public ActionResult About()
        {
            return View("About");
        }

        public ActionResult RefreshList(string id)
        {
            unReadOnline.ViewsData.JsonResult jsonResult = new unReadOnline.ViewsData.JsonResult();
            string forwho = Request.QueryString["forwho"];
            if (!string.IsNullOrEmpty(forwho))
            {
                StringBuilder sb = new StringBuilder();
                if (forwho == "All")
                {
                    BaseUnReadLinkData viewData = new BaseUnReadLinkData();
                    if (!string.IsNullOrEmpty(id))
                    {
                        try
                        {
                            viewData.CurrentPage = Convert.ToInt32(id);
                        }
                        catch
                        {
                            viewData.CurrentPage = 1;
                        }

                    }
                    else
                    {
                        viewData.CurrentPage = 1;
                    }
                    viewData.UnReadLinkPerPage = 20;
                    int totalCount;
                    viewData.UnReadLinks = UnReadLink.GetUnReadLinks(viewData.UnReadLinkPerPage, viewData.CurrentPage, out totalCount);
                    viewData.TotalUnReadLinks = totalCount;

                    bool isAdmin = Roles.IsUserInRole("Administrators");
                    foreach (UnReadLink link in viewData.UnReadLinks)
                    {
                        sb.Append("<div class='linkItem'><h4 class='linkTitle'><a class='linkA' href='");
                        sb.Append(link.Url);
                        sb.Append("' target='_blank'>");
                        sb.Append(link.Title);
                        sb.Append("</a></h4><div class='itemInfo'><div class='linkDes'>");
                        sb.Append(string.IsNullOrEmpty(link.Description) ? link.Title : link.Description);
                        sb.Append("</div><p class='linkFoot'>");
                        sb.Append(link.DateCreated.ToString("yy-MM-dd hh:mm:ss　"));
                        if (isAdmin)
                        {
                            sb.Append("<a class='linkDel' href='Manage.mvc/DelULink/");
                            sb.Append(link.Id);
                            sb.Append("'>删除</a>　<a class='linkEdit' href='Manage.mvc/Edit/");
                            sb.Append(link.Id);
                            sb.Append("'>编辑</a>");
                        }
                        sb.Append("</p></div></div>");
                    }
                    jsonResult.isSuccessful = true;
                    jsonResult.errorMessage = viewData.PageCount.ToString();
                }
                else if (forwho == "User")
                {

                }
                jsonResult.returnHtml = sb.ToString();
            }
            return View("Ajax", jsonResult);
        }
    }
}
