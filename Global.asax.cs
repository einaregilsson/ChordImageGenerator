using System;
using System.Collections.Generic;
using System.Web;
using EinarEgilsson.Chords;

namespace Chords
{
    public class Global : HttpApplication
    {
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string path = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath;
            if (path.ToLower() == "~/" || path.ToLower().StartsWith("~/index.html"))
            {
                return;
            }
            HttpContext.Current.RemapHandler(new ChordHandler());
        }
    }
}