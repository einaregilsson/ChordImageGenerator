/*
 * Chord Image Generator
 * http://einaregilsson.com/chord-image-generator/
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
        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;

            var chordName = Regex.Match(request.FilePath, @"^/(.*)\.png$").Groups[1].Value;
            var qs = request.QueryString; 

            var pos =        qs["pos"]       ?? qs["p"] ?? "000000";
            var fingers =    qs["fingers"]   ?? qs["f"] ?? "------";
            var size =       qs["size"]      ?? qs["s"] ?? "1";
            
            using (var img = new ChordBoxImage(chordName, pos, fingers, size))
            {
                response.ContentType = "image/png";
                response.ExpiresAbsolute = DateTime.Now.AddDays(7);
                img.Save(context.Response.OutputStream);
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
