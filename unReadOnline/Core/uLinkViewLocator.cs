using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Mvc;

namespace unReadOnline.Core
{
    public class uLinkViewLocator : ViewLocator
    {
        public uLinkViewLocator()
        {
            base.ViewLocationFormats = new string[] { BaseviewPath + "/{1}/{0}.aspx",
                                                      BaseviewPath + "/{1}/{0}.aspx",
                                                      BaseviewPath + "/Shared/{0}.aspx",
                                                      BaseviewPath + "/Shared/{0}.aspx",
                                                      "~/Views/{1}/{0}.aspx",
                                                      "~/Views/{1}/{0}.aspx",
                                                      "~/Views/Shared/{0}.aspx",
                                                      "~/Views/Shared/{0}.aspx"
            };
        }

        private string _baseviewPath;
        private string BaseviewPath
        {
            get
            {
                if (string.IsNullOrEmpty(_baseviewPath))
                {
                    string viewTheme = ConfigXml.Element("appsetting").Attribute("ViewTheme").Value;
                    viewTheme = string.IsNullOrEmpty(viewTheme) ? "Default" : viewTheme;
                    _baseviewPath = "~/Views/" + viewTheme;
                }
                return _baseviewPath;
            }
        }

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
    }
    
}
