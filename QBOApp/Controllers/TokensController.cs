using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QBOApp.Helpers;

namespace QBOApp.Controllers
{
    public class TokensController : Controller
    {
        // GET: Tokens
        public async Task<ActionResult> Index()
        {
            string code = Request.QueryString["code"] ?? "none";
            string realmId = Request.QueryString["realmId"] ?? "none";

            var res = await QBOHelper.GetAccessTokenByCode(
                ConfigHelper.AccessTokenUrl, 
                code,
                ConfigHelper.ClientId,
                ConfigHelper.ClientSecret,
                ConfigHelper.RedirectUrl);

            if(res != null)
            {
                var data = new
                {
                    accessToken = res.access_token,
                    refreshToken = res.refresh_token,
                    accessTokenExpiresIn = DateTime.Now.AddSeconds(Convert.ToInt32(res.expires_in)),
                    refreshTokenExpiresIn = DateTime.Now.AddSeconds(Convert.ToInt32(res.x_refresh_token_expires_in)),
                    code = code,
                    realmId = realmId
                };

                FileWriter.Write("temp",
                    "tokens",
                    "json",
                    JsonConvert.SerializeObject(data));

                return Json(new { message = "OAuth 2.0 token success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "OAuth 2.0 token error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("refresh")]
        public async Task<ActionResult> RefreshTokens()
        {
            var tokenData = FileWriter.Read("temp", "tokens.json");

            if (!string.IsNullOrEmpty(tokenData))
            {
                var tokens = JObject.Parse(tokenData);
                var refreshToken = tokens["refreshToken"].ToString();
                var code = tokens["code"].ToString();
                var realmId = tokens["realmId"].ToString();

                var res = await QBOHelper.RefreshAccessToken(
                   ConfigHelper.AccessTokenUrl,
                   refreshToken,
                   ConfigHelper.ClientId,
                   ConfigHelper.ClientSecret);

                if(res != null)
                {
                    var data = new
                    {
                        accessToken = res.access_token,
                        refreshToken = res.refresh_token,
                        accessTokenExpiresIn = DateTime.Now.AddSeconds(Convert.ToInt32(res.expires_in)),
                        refreshTokenExpiresIn = DateTime.Now.AddSeconds(Convert.ToInt32(res.x_refresh_token_expires_in)),
                        code = code,
                        realmId = realmId
                    };

                    FileWriter.Write("temp",
                        "tokens",
                        "json",
                        JsonConvert.SerializeObject(data));

                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "Error refreshing tokens." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { message = "No refresh token found." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}