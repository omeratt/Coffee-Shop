using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CoffeeShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");



            routes.MapRoute(
                name: "Home",
                url: "Home",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "Reg",
               url: "Reg",
               defaults: new { controller = "LogReg", action = "Reg", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "Log",
               url: "Log",
               defaults: new { controller = "LogReg", action = "Log", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "LogOut",
               url: "LogOut",
               defaults: new { controller = "LogReg", action = "Logout", id = UrlParameter.Optional }
           );
           routes.MapRoute(
               name: "CreateTable",
               url: "CreateTable",
               defaults: new { controller = "Admin", action = "CreateTable", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "SearchUser",
               url: "SearchUser",
               defaults: new { controller = "Admin", action = "SearchUser", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "SearchItems",
               url: "SearchItems",
               defaults: new { controller = "Home", action = "SearchItems", id = UrlParameter.Optional }
           );
            routes.MapRoute(
              name: "edit",
              url: "EditCoffee",
              defaults: new { controller = "Admin", action = "EditCoffee", id = UrlParameter.Optional }
          );
            routes.MapRoute(
             name: "create",
             url: "Create",
             defaults: new { controller = "Admin", action = "Create", id = UrlParameter.Optional }
         );
            routes.MapRoute(
              name: "DeleteDrink",
              url: "DeleteDrink",
              defaults: new { controller = "Admin", action = "DeleteDrink", id = UrlParameter.Optional }
          );
            routes.MapRoute(
              name: "Admin",
              url: "Admin",
              defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
          );

            routes.MapRoute(
              name: "addUser",
              url: "AddUser",
              defaults: new { controller = "Admin", action = "AddUser", id = UrlParameter.Optional }
          );
            routes.MapRoute(
              name: "menu",
              url: "Menu",
              defaults: new { controller = "Home", action = "Menu", id = UrlParameter.Optional }
          );

            routes.MapRoute(
              name: "booking",
              url: "BookTable",
              defaults: new { controller = "Home", action = "BookTable", id = UrlParameter.Optional }
          );
            routes.MapRoute(
              name: "cart",
              url: "Cart",
              defaults: new { controller = "Home", action = "Cart", id = UrlParameter.Optional }
          );
            routes.MapRoute(
            name: "updatecart",
            url: "UpdateCart",
            defaults: new { controller = "Home", action = "UpdateCart", id1 = UrlParameter.Optional }
        );
            routes.MapRoute(
            name: "BookTableCart",
            url: "BookTableCart",
            defaults: new { controller = "Home", action = "BookTableCart", id1 = UrlParameter.Optional }
        );
            routes.MapRoute(
           name: "Pay",
           url: "Pay",
           defaults: new { controller = "Home", action = "Pay", id1 = UrlParameter.Optional }
       );

            routes.MapRoute(
            name: "Default",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
        );
        }
    }
}