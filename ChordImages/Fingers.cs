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
using System.Linq;
using System.Text.RegularExpressions;

namespace EinarEgilsson.ChordImages
{
    internal class Fingers
    {
        #region Fields

        private Finger[] _fingers = new Finger[] {
            Finger.None,
            Finger.None,
            Finger.None,
            Finger.None,
            Finger.None,
            Finger.None
        };
        private readonly Regex _fingerRegex = new Regex(@"[tT\-1234]{6}", RegexOptions.Compiled);

        #endregion

        #region Properties

        internal bool ParseError { get; private set; }
        internal Finger[] FingerPositions { get { return _fingers; } }

        #endregion

        #region Constructor

        internal Fingers(string fingerPositions)
        {
            ParseFingers(fingerPositions);
        }

        #endregion

        #region Parsing

        private void ParseFingers(string fingers)
        {
            if (fingers == null)
            {
                return; //Allowed to not specify fingers
            }
            else if (!_fingerRegex.IsMatch(fingers))
            {
                ParseError = true;
            }
            else
            {
                _fingers = fingers.Select(x => ToFinger(x)).ToArray();
            }
        }

        #endregion

        internal bool IsFingerAt(int pos)
        {
            return _fingers[pos] != Finger.None;
        }

        internal Finger FingerAt(int pos)
        {
            return _fingers[pos];
        }

        internal string PrintAt(int pos)
        {
            return ConvertToString(_fingers[pos]);
        }

        private static string ConvertToString(Finger f)
        {
            switch (f)
            {
                case Finger.None:
                    return "-";
                case Finger.Thumb:
                    return "T";
                default:
                    var fInt = (int)f;
                    return fInt.ToString();
            }
        }

        private static Finger ToFinger(char c)
        {
            switch (c)
            {
                case 'T':
                case 't':
                    return Finger.Thumb;                
                default:
                    var finger = Finger.None;
                    Enum.TryParse(new string(new[] { c }), out finger);
                    return finger;
            }
        }
    }

    enum Finger
    {
        Thumb = -1,
        None = 0,
        Index = 1,
        Middle = 2,
        Ring = 3,
        Little = 4
    }
}
