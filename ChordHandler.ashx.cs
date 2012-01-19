/*
 * Chord Image Generator
 * http://einaregilsson.com/2009/07/23/chord-image-generator/
 *
 * Copyright (C) 2009-2012 Einar Egilsson [einar@einaregilsson.com]
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
 */
using System;
using System.Text.RegularExpressions;
using System.Web;

namespace EinarEgilsson.Chords
{
    /// <summary>
    /// HTTP Handler that interprets the url and saves a generated
    /// image to the response stream.
    /// </summary>
    public class ChordHandler : IHttpHandler
    {
        private string Get(HttpContext context, string key)
        {
            return  context.Request.QueryString[key] ?? context.Request.QueryString[key.Substring(0,1)];
        }

        public void ProcessRequest(HttpContext context)
        {
            //Important to use .RawUrl, since that hasn't been set to chord.ashx
            string chordName = Regex.Replace(context.Request.AppRelativeCurrentExecutionFilePath, "^~/|/$", "");
            chordName = Regex.Replace(chordName, @"\.png$", "", RegexOptions.IgnoreCase);
            
            string pos = Get(context, "pos") ?? "000000";
            string fingers = Get(context, "fingers") ?? "------";
            string size = Get(context, "size") ?? "1";
            
            using (var img = new ChordBoxImage(chordName, pos, fingers, size))
            {
                context.Response.ContentType = "image/png";
                context.Response.ExpiresAbsolute = DateTime.Now.AddDays(7);
                img.Save(context.Response.OutputStream);
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
