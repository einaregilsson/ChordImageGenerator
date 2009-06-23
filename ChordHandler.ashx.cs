using System;
using System.Web;

namespace EinarEgilsson.Chords {

    public class ChordHandler : IHttpHandler {

        public void ProcessRequest(HttpContext context) {
            string path = context.Request.RawUrl;
            if (path.ToLower() == "/chordhandler.ashx") {
                context.Response.Redirect("http://tech.einaregilsson.com/2009/05/20/online-chord-image-generator/", true);
            }
            string prefix = "/ChordHandler.ashx?404;http://chords.einaregilsson.com:80";
            if (path.ToLower().StartsWith(prefix.ToLower())) {
                path = path.Substring(prefix.Length);
            }
            
            if (path.StartsWith("/")) {
                path = path.Substring(1);
            }

            if (path.EndsWith("/")) {
                path = path.Substring(0, path.Length - 1);
            }

            string[] parts = path.Split('/');
            string name = "", chord = "000000", fingers = null, size = "2";
            if (parts.Length > 0) {
                name = parts[0];
            }
            if (parts.Length > 1) {
                chord = parts[1];
            }
            if (parts.Length > 2) {
                fingers = parts[2];
            }
            if (parts.Length > 3) {
                size = parts[3];
            }
            ChordBoxImage img = new ChordBoxImage(name, chord, fingers, size);
            context.Response.ContentType = "image/jpg";
            img.Save(context.Response.OutputStream);
        }

        public bool IsReusable {
            get { return true; }
        }
    }
}
