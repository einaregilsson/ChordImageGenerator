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

namespace EinarEgilsson.ChordImages
{
    internal class Chord
    {
        #region Fields

        private readonly Playstyle[] _chordPositions = new Playstyle[6];
        private readonly Regex _chordRegex = new Regex(@"[\dxX]{6}|((1|2)?[\dxX]-){5}(1|2)?[\dxX]", RegexOptions.Compiled);

        #endregion

        #region Properties

        internal string Name { get; }
        internal bool ParseError { get; private set; }
        internal int Length { get { return _chordPositions.Length; } }
        internal int BaseFret { get; private set; }

        #endregion

        #region Constructor

        internal Chord(string name, string chord)
        {
            Name = ParseName(name);
            ParseChord(chord);
        }

        #endregion

        #region Parsing

        private string ParseName(string name)
        {
            return (name == null) ? "" : name.Replace(" ", "");
        }

        private void ParseChord(string chord)
        {
            if (chord == null || !_chordRegex.IsMatch(chord))
            {
                ParseError = true;
            }
            else
            {
                string[] parts;
                if (chord.Length > 6)
                {
                    parts = chord.Split('-');
                }
                else
                {
                    parts = new string[6];
                    for (int i = 0; i < 6; i++)
                    {
                        parts[i] = chord[i].ToString();
                    }
                }
                int maxFret = 0, minFret = int.MaxValue;
                for (int i = 0; i < 6; i++)
                {
                    if (string.Equals(parts[i], "X", StringComparison.OrdinalIgnoreCase))
                    {
                        _chordPositions[i] = Playstyle.Muted;
                    }
                    else
                    {
                        Enum.TryParse(parts[i], out _chordPositions[i]);
                        maxFret = Math.Max(maxFret, (int)_chordPositions[i]);
                        if (_chordPositions[i] != 0)
                        {
                            minFret = Math.Min(minFret, (int)_chordPositions[i]);
                        }
                    }
                }
                if (maxFret <= 5)
                {
                    BaseFret = 1;
                }
                else
                {
                    BaseFret = minFret;
                }
            }
        }

        #endregion

        internal bool MutedAt(int pos)
        {
            return _chordPositions[pos] == Playstyle.Muted;
        }

        internal bool OpenAt(int pos)
        {
            return _chordPositions[pos] == Playstyle.Open;
        }

        internal Playstyle PlaystyleAt(int pos)
        {
            return _chordPositions[pos];
        }

        internal int PlaystyleAsIntAt(int pos)
        {
            return (int)_chordPositions[pos];
        }

        internal bool NeedToDrawNut()
        {
            return BaseFret == 1;
        }
    }

    internal enum Playstyle
    {
        Muted = -1,
        Open = 0
    }
}
