using System;
using System.Web;

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
            string path = HttpContext.Current.Request.RawUrl;
            if (path.ToLower().EndsWith(".css") 
                || path.ToLower().EndsWith(".js") 
                || path.ToLower().StartsWith("/chord.ashx")
                || path == "/" 
                || path.ToLower() == "/index.html"
                || path.ToLower() == "/favicon.ico")
            {
                return;
            }
            HttpContext.Current.RewritePath("/chord.ashx");
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