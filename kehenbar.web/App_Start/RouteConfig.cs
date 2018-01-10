using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace kehenbar.web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "表单列表",
                url: "database/datalist/{id}/{pagesize}/{page}",
                defaults: new { controller = "database", action = "datalist" }
            );

            routes.MapRoute(
                name: "表单编辑",
                url: "database/dataEdit/{bm}/{id}",
                defaults: new { controller = "database", action = "dataEdit" }
            );

            routes.MapRoute(
                name: "表单查看",
                url: "database/dataShow/{bm}/{id}",
                defaults: new { controller = "database", action = "dataShow" }
            );

            routes.MapRoute(
                name: "默认",
                url: "{controller}/{action}/{id}"
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}