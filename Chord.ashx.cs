using System;
using System.Web;
using System.Collections.Generic;

namespace EinarEgilsson.Chords {

    public class Chord: IHttpHandler {

        public void ProcessRequest(HttpContext context) {
            string path = context.Request.RawUrl;

            string productionPrefix = "/Chord.ashx?404;http://chords.einaregilsson.com:80";
            string developmentPrefix = "/Chord.ashx/";
            if (path.ToLower().StartsWith(productionPrefix.ToLower())) {
                path = path.Substring(productionPrefix.Length);
            } else if (path.ToLower().StartsWith(developmentPrefix)) {
                path = path.Substring(developmentPrefix.Length);
            }
            
            if (path.StartsWith("/")) {
                path = path.Substring(1);
            }

            if (path.EndsWith("/")) {
                path = path.Substring(0, path.Length - 1);
            }

            List<string>  parts = new List<string>(path.Split('/'));

            if (parts.Count > 0 && parts[0].ToLower().StartsWith("chord.ashx")) {
                parts.RemoveAt(0);
            }
            string name = "", chord = "000000", fingers = null, size = "2";
            if (parts.Count > 0) {
                name = parts[0];
            }
            if (parts.Count > 1) {
                chord = parts[1];
            }
            if (parts.Count > 2) {
                fingers = parts[2];
            }
            if (parts.Count > 3) {
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
