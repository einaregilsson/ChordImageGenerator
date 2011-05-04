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
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;

namespace EinarEgilsson.Chords
{

    /// <summary>
    /// HTTP Handler that interprets the url and saves a generated
    /// image to the response stream.
    /// </summary>
    public class Chord : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //Important to use .RawUrl, since that hasn't been set to chord.ashx
            string path = Regex.Replace(context.Request.AppRelativeCurrentExecutionFilePath, "^~/|/$", "");
            using (var img = new ChordBoxImage(path))
            {
                context.Response.ContentType = img.MimeType;
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
