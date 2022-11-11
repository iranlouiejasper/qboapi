using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Http;
using System.Configuration;
using QBOApp.Helpers;
using QBOApp.Filters;

namespace QBOApp.Controllers
{
    public class HomeController : Controller
    {
        [VerifyQBOTokenFilter]
        public ActionResult Index()
        {
            return Json(new { app = "QBOApp" }, JsonRequestBehavior.AllowGet);
        }

        [Route("oauth2")]
        public ActionResult GetQBOAuthURL()
        {
            return Redirect(
               QBOHelper.GetAuthURL(
                   ConfigHelper.AuthUrl,
                   ConfigHelper.ClientId,
                   ConfigHelper.Scope,
                   ConfigHelper.RedirectUrl,
                   ConfigHelper.State));
        }
    }
}