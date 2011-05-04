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
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace EinarEgilsson.Chords.Cgi
{
    class Program
    {
        private static string BasePath
        {
            get
            {
                return (string)new AppSettingsReader().GetValue("BasePath", typeof(string));
            }
        }

        private static string GetPath(string[] args)
        {
            string path;
            if (args.Length > 0)
            {
                path = args[0];
            }
            else
            {
                path = Environment.GetEnvironmentVariable("PATH_INFO")
                       ?? Environment.GetEnvironmentVariable("REQUEST_URI");
            }

            if (path == null)
            {
                throw new Exception("Could not get path from arguments or environment");
            }
            if (BasePath == "")
            {
                return path;
            }
            if (!path.StartsWith(BasePath))
            {
                throw new ConfigurationErrorsException("BasePath is incorrectly configured. BasePath=" + BasePath +
                                                 ", Path=" + path);
            }

            return path.Substring(BasePath.Length);
        }

        private static byte[] Line(string format, params object[] args)
        {
            return Encoding.ASCII.GetBytes(string.Format(format + "\r\n", args));
        }

        static void Main(string[] args)
        {
            string path = GetPath(args);
            using (var img = new ChordBoxImage(path))
            using (var writer = new BinaryWriter(Console.OpenStandardOutput()))
            {
                byte[] imgBytes = img.GetBytes();
                //writer.Write(Line("HTTP/1.1 200 OK"));
                writer.Write(Line("Content-Type: {0}", img.MimeType));
                writer.Write(Line("Content-Length: {0}", imgBytes.Length));
                writer.Write(Line(""));
                writer.Write(imgBytes);
            }
        }
    }
}
