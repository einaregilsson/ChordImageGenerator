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
using System.Collections.Generic;

namespace EinarEgilsson.ChordImages
{
    internal static class Bars
    {
        internal static IEnumerable<Bar> GetBars(Chord chord, Fingers fingers)
        {
            var bars = new Dictionary<Finger, Bar>();
            for (int i = 0; i < chord.Length; i++)
            {
                if (!chord.MutedAt(i) && !chord.OpenAt(i) && fingers.IsFingerAt(i) && !bars.ContainsKey(fingers.FingerAt(i)))
                {
                    var bar = CreateNewBar(chord, fingers, i);
                    if (bar.Length > 0)
                    {
                        bars.Add(bar.Finger, bar);
                    }
                }
            }
            return bars.Values;
        }

        private static Bar CreateNewBar(Chord chord, Fingers fingers, int i)
        {
            var bar = new Bar { Str = i, Pos = chord.PlaystyleAsIntAt(i), Length = 0, Finger = fingers.FingerAt(i) };
            for (int j = i + 1; j < 6; j++)
            {
                if (fingers.FingerAt(j) == bar.Finger && chord.PlaystyleAsIntAt(j) == chord.PlaystyleAsIntAt(i))
                {
                    bar.Length = j - i;
                }
            }
            return bar;
        }
    }

    internal struct Bar
    {
        public int Str, Pos, Length;
        public Finger Finger;
    }
}
