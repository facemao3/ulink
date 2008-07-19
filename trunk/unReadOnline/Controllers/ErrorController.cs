using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace unReadOnline.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            ViewData["errorMsg"] = TempData["errorMsg"];
            return View();
        }
    }
}
