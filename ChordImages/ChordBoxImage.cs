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
using System.IO;

namespace EinarEgilsson.ChordImages
{
    /// <summary>
    /// Class to create images of chordboxes. Can be saved
    /// to any subclass of Stream.
    /// </summary>
    public class ChordBoxImage : IDisposable
    {
        #region Fields

        private readonly ChordGraphics _graphics;

        #endregion

        #region Constructor

        public ChordBoxImage(string name, string chordString, string fingersString, string sizeString)
        {
            var chord = new Chord(name, chordString);
            var fingers = new Fingers(fingersString);
            _graphics = new ChordGraphics(sizeString);
            _graphics.CreateImage(chord, fingers);
        }

        #endregion

        #region Public methods

        public void SaveImage(Stream output)
        {
            _graphics.Save(output);
        }

        public byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                return ms.ToArray();
            }
        }

        public void Dispose()
        {
            _graphics.Dispose();
        }

        #endregion
    }
}
