﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using unReadOnline.Core;

namespace unReadOnline
{
    public class GlobalApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutesForOld(RouteCollection routes)
        {
            // Note: Change the URL to "{controller}.mvc/{action}/{id}" to enable
            //       automatic support on IIS6 and IIS7 classic mode

            //routes.Add(new Route("{controller}.mvc/{action}/{forwho}/{id}", new MvcRouteHandler())
            //{
            //    Defaults = new RouteValueDictionary(new { action = "Index", forwho = "", id = "" }),
            //});

            routes.Add(new Route("{controller}.mvc/{action}/{id}", new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(new { action = "Index", id = "" ,synType=""}),
            });

            routes.Add(new Route("{controller}.mvc/{action}/{forwho}/{id}", new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(new { action = "Index", forwho="", id = "" }),
            });

            routes.Add(new Route("Default.aspx", new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(new { controller = "Home", action = "Index", id = "" }),
            });
        }

        public static void RegisterRoutesForNew(RouteCollection routes)
        {
            // Note: Change the URL to "{controller}.mvc/{action}/{id}" to enable
            //       automatic support on IIS6 and IIS7 classic mode

            routes.Add(new Route("{controller}.mvc/{action}/{forwho}/{id}", new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(new { action = "Index", forwho = "", id = "" }),
            });

            routes.Add(new Route("{controller}/{action}/{id}", new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(new { action = "Index", id = "" }),
            });

            routes.Add(new Route("{controller}.mvc/{action}/{forwho}/{id}", new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(new { action = "Index", forwho = "", id = "" }),
            });

            routes.Add(new Route("Default.aspx", new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(new { controller = "Home", action = "Index", id = "" }),
            });
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            ControllerBuilder.Current.SetControllerFactory(typeof(uLinkControllerFactory));
            RegisterRoutesForOld(RouteTable.Routes);
        }
    }
}