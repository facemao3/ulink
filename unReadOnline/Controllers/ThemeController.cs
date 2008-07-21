using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace unReadOnline.Controllers
{
    public class ThemeController : Controller
    {
        private XElement configXml;
        private string path = System.Web.HttpRuntime.AppDomainAppPath + "/Config/Site.config";
        /// <summary>
        /// 加载XML文件
        /// </summary>
        private XElement ConfigXml
        {
            get
            {
                if (configXml == null)
                {
                    configXml = XElement.Load(path);
                }
                return configXml;
            }
        }

        public ActionResult Index()
        {
            // Add action logic here
            throw new NotImplementedException();
        }

        public ActionResult ChangeTheme(string id)
        {
            string theme = id.ToLower() == "simple" ? id : "Default";
            ConfigXml.Element("appsetting").SetAttributeValue("ViewTheme", theme);
            ConfigXml.Save(path);

            return RedirectToAction("Index", "Home");
        }
    }
}
