/*
 * Chord Image Generator
 * http://tech.einaregilsson.com/2009/07/23/chord-image-generator/
 *
 * Copyright (C) 2009 Einar Egilsson [einar@einaregilsson.com]
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 * $HeadURL$
 * $LastChangedDate$
 * $Author$
 * $Revision$
 */
using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Configuration;

namespace EinarEgilsson.Chords {

    /// <summary>
    /// HTTP Handler that interprets the url and saves a generated
    /// image to the response stream.
    /// </summary>
    public class Chord : IHttpHandler {

        private static StreamWriter logger;
        static Chord()
        {
            string logfile = (string) new AppSettingsReader().GetValue("logfile",typeof(string));
            FileStream stream = new FileStream(logfile, FileMode.Append, FileAccess.Write);
            logger = new StreamWriter(stream);
        }

        public void ProcessRequest(HttpContext context) {
            string path = context.Request.RawUrl;

            //Special cases for chords.einaregilsson.com
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

            if (path.ToLower().StartsWith("chord.ashx/")) {
                path = path.Substring("chord.ashx/".Length);
            }

            //Log this
            string logEntry = String.Format("{0} - {1} - {2} - {3}\r\n", DateTime.Now, path, context.Request.ServerVariables["HTTP_REFERER"], context.Request.ServerVariables["REMOTE_ADDR"]);
            lock (logger) {
                logger.Write(logEntry);
                logger.Flush();
            }

            List<string> parts = new List<string>(path.Split('/'));

            //Defaults
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
            context.Response.ExpiresAbsolute = DateTime.Now.AddDays(7);
            img.Save(context.Response.OutputStream);
        }

        public bool IsReusable {
            get { return true; }
        }
    }
}
