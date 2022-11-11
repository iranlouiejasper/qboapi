using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace QBOApp.Helpers
{
    public static class FileWriter
    {
        public static void Write(string path,string fileName,string extension,string content)
        {
            string fullPath = path + "/" + fileName + "." + extension;

            if(!Directory.Exists(HttpContext.Current.Server.MapPath("~/" + path)))
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/" + path));
            }

            File.WriteAllText(HttpContext.Current.Server.MapPath("~/" + fullPath), content);
        }

        public static string Read(string path, string fullFileName)
        {
            string content = "";

            if (File.Exists(HttpContext.Current.Server.MapPath("~/" + path + "/" + fullFileName)))
            {
                content = File.ReadAllText(HttpContext.Current.Server.MapPath("~/" + path + "/" + fullFileName));
            }

            return content;
        }
    }
}