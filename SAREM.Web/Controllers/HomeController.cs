using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SAREM.Web.Controllers
{
   

    public class HomeController : Controller
    {
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    if (filterContext.HttpContext.Session["usuario"] != null)
        //    {
        //        Debug.WriteLine("USUARIO NO ES NULL...");
        //        //paciente = (string)filterContext.HttpContext.Session["usuario"];
        //        //tenant = (string)filterContext.HttpContext.Session["tenant"];
        //        //fabrica = new FabricaSAREM(tenant);
        //    }
        //    else
        //    {
        //        Debug.WriteLine("USUARIO NULL...");
        //        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "GetLogOff", controller = "Account" }));
        //    }
        //}

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}