using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QBOApp.Filters;
using QBOApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QBOApp.Controllers
{
    public class ItemsController : Controller
    {
        // GET: Items
        [VerifyQBOTokenFilter]
        public async Task<ActionResult> Index()
        {
            var tokenData = FileWriter.Read("temp", "tokens.json");

            if (string.IsNullOrEmpty(tokenData))
            {
                return Json(new { message = "Unable to execute, No access token found." }, JsonRequestBehavior.AllowGet);
            }

            var tokens = JObject.Parse(tokenData);
            var accessToken = tokens["accessToken"].ToString();
            var refreshToken = tokens["refreshToken"].ToString();
            var realmId = tokens["realmId"].ToString();

            string query = "select * from Item";

            var res = await QBOHelper.QueryEntities(
                accessToken,
                ConfigHelper.QBOBaseUrl,
                realmId,
                query);

            if(res == null)
            {
                return Json(new { message = "There was a problem executing the request." }, JsonRequestBehavior.AllowGet);
            }

            var data = new
            {
                accessToken = accessToken,
                refreshToken = refreshToken,
                items = JObject.Parse(JsonConvert.SerializeObject(res))["QueryResponse"]["Item"]
            };

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
    }
}