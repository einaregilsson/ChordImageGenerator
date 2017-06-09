using System;
using System.Collections.Generic;
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
            HttpApplication application = (HttpApplication) sender;
            HttpContext context = application.Context;
            if (context.Request.Url.Host == "www.chordgenerator.net")
            {
                context.Response.RedirectPermanent("http://chordgenerator.net" + context.Request.Url.PathAndQuery);
                context.Response.End();
            }
        }
    }
}