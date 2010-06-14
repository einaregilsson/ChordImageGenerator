using System;
using System.Collections.Generic;
using System.Web;
using EinarEgilsson.Chords;

namespace Chords
{
    public class Global : HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var excluded = new List<string>(new[] {"~/chords.css", "~/chords.js", "~/", "~/chord.ashx", "~/index.html"});
            string path = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath;

            if (excluded.Contains(path.ToLower()))
            {
                return;
            }
            HttpContext.Current.RemapHandler(new Chord());
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}