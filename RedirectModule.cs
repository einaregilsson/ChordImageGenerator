using System;
using System.Web;

namespace EinarEgilsson.Chords
{
    public class RedirectModule : IHttpModule
    {
        public void Dispose()
        {
            
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequest;
        }

        void BeginRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication) sender;
            HttpContext context = application.Context;
            const string wwwUrl = "www.chordgenerator.net";
            if (context.Request.Url.Host ==  wwwUrl || context.Request.Headers.Get("X-Forwarded-Host") == wwwUrl)
            {
                context.Response.RedirectPermanent("http://chordgenerator.net" + context.Request.Url.PathAndQuery);
                context.Response.End();
            }
        }
    }
}