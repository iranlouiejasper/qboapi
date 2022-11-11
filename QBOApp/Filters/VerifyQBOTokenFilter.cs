using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QBOApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QBOApp.Filters
{
    public class VerifyQBOTokenFilter : ActionFilterAttribute, IActionFilter
    {
        async void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            var tokenData = FileWriter.Read("temp", "tokens.json");

            if (string.IsNullOrEmpty(tokenData))
            {
                if(filterContext.HttpContext.Request.Url.PathAndQuery == "/")
                {
                    filterContext.Result = new RedirectResult("~/oauth2");
                }
                else
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new { message = "No access token for QBO." },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            else
            {
                var tokens = JObject.Parse(tokenData);
                var accessToken = tokens["accessToken"].ToString();
                var refreshToken = tokens["refreshToken"].ToString();
                var accessTokenExpiresIn = DateTime.Parse(tokens["accessTokenExpiresIn"].ToString());
                var refreshTokenExpiresIn = DateTime.Parse(tokens["refreshTokenExpiresIn"].ToString());
                var code = tokens["code"].ToString();
                var realmId = tokens["realmId"].ToString();

                if (DateTime.Now > accessTokenExpiresIn)
                {
                    if (DateTime.Now > refreshTokenExpiresIn)
                    {
                        if (filterContext.HttpContext.Request.Url.PathAndQuery == "/")
                        {
                            filterContext.Result = new RedirectResult("~/oauth2");
                        }
                        else
                        {
                            filterContext.Result = new JsonResult
                            {
                                Data = new { message = "QBO refresh token is expired." },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                        }
                    }

                    //Do refresh
                    var tokenResp = await QBOHelper.RefreshAccessToken(ConfigHelper.AccessTokenUrl,
                           refreshToken,
                           ConfigHelper.ClientId,
                           ConfigHelper.ClientSecret
                        );

                    if(tokenResp != null)
                    {
                        var data = new
                        {
                            accessToken = tokenResp.access_token,
                            refreshToken = tokenResp.refresh_token,
                            accessTokenExpiresIn = DateTime.Now.AddSeconds(Convert.ToInt32(tokenResp.expires_in)),
                            refreshTokenExpiresIn = DateTime.Now.AddSeconds(Convert.ToInt32(tokenResp.x_refresh_token_expires_in)),
                            code = code,
                            realmId = realmId
                        };

                        FileWriter.Write("temp",
                           "tokens",
                           "json",
                           JsonConvert.SerializeObject(data));
                    }
                }
            }
        }
    }
}