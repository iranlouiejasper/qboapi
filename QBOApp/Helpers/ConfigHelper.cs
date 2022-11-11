using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace QBOApp.Helpers
{
    public static class ConfigHelper
    {
        public static string ClientId
        {
            get { return ConfigurationManager.AppSettings["clientId"]; }
        }

        public static string ClientSecret
        {
            get { return ConfigurationManager.AppSettings["clientSecret"]; }
        }

        public static string AuthUrl
        {
            get { return ConfigurationManager.AppSettings["authUrl"]; }
        }

        public static string RedirectUrl
        {
            get { return ConfigurationManager.AppSettings["redirectUrl"]; }
        }

        public static string AccessTokenUrl
        {
            get { return ConfigurationManager.AppSettings["accessTokenUrl"]; }
        }

        public static string Scope
        {
            get { return ConfigurationManager.AppSettings["scope"]; }
        }

        public static string State
        {
            get { return ConfigurationManager.AppSettings["state"]; }
        }

        public static string QBOApiUrl
        {
            get { return ConfigurationManager.AppSettings["qboApiUrl"]; }
        }

        public static string QBOBaseUrl
        {
            get { return ConfigurationManager.AppSettings["baseUrl"]; }
        }
    }
}