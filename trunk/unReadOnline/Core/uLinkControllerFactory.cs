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
using System.Web.Routing;

namespace unReadOnline.Core
{
    public class uLinkControllerFactory : DefaultControllerFactory
    {
        protected override IController CreateController(RequestContext requestContext, string controllerName)
        {
            Controller controller = (Controller)base.CreateController(requestContext, controllerName);
            controller.ViewEngine = new uLinkViewEngine();//修改默认的视图引擎为我们刚才创建的视图引擎
            return controller;
        }

    }
}
