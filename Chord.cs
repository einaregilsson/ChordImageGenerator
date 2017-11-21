using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EinarEgilsson.Chords
{
    public class Chord
    {
        String Name { get; set; }
        private int[] _chordPositions = new int[6];

        public Chord(String name) {
            Name = name;
        }

        public String getRootNote() {
            System.Text.StringBuilder chordRoot = new System.Text.StringBuilder();
            if (Name.Length < 1)
            {
                return "";
            }
            chordRoot.Append(Name[0]);

            if (Name.Length > 1)
            {
                char secondLetter = Name[1];
                if (secondLetter == 'b' || secondLetter == '#')
                {
                    chordRoot.Append(secondLetter);
                }
            }
            return chordRoot.ToString();
        }

        public String getIntervalFromRootNote(String secondNote) {
            String rootNote = getRootNote();
            return getInterval(Name, rootNote, secondNote);
        }

        public static String getInterval(String chordName, String rootNote, String secondNote)
        {
            String[] scale = { "E", "F", "F#", "G", "G#", "A", "A#", "B", "C", "C#", "D", "D#" };
            int rootIndex = Array.IndexOf(scale, rootNote, 0);
            String target = "";
            int interval = 0;
            int counter = rootIndex;
            while (!target.Equals(secondNote))
            {
                interval++;
                counter++;
                counter = counter % scale.Length;
                target = scale[counter];
            }

            switch (interval)
            {
                case 0:
                    return "r";
                case 1: //b2, b9
                    return returnFirstMatch(chordName, new string[] { "b2", "b9"});
                case 2:// 2, 9
                    return returnFirstMatch(chordName, new string[] { "2", "9" });
                case 3: // b3, #9
                    return returnFirstMatch(chordName, new string[] { "#9", "b3" });
                case 4:
                    return "3";
                case 5: //4, 11
                    return returnFirstMatch(chordName, new string[] { "4", "11" });
                case 6: //b5, #11
                    return returnFirstMatch(chordName, new string[] { "#11", "b5" });
                case 7:
                    return "5";
                case 8: // m6, 5#, +5, b13
                    return returnFirstMatch(chordName, new string[] { "b13", "+5", "5#", "m6" });
                case 9: // 6, 13
                    return returnFirstMatch(chordName, new string[] { "13", "6" });
                case 10:
                    return "b7";
                case 11:
                    return "7";
                case 12:
                    return "r";
                default:
                    return "?";
            }
        }

        private static String returnFirstMatch(String chordName, String[] possibleNoteNames) {
            foreach(String note in possibleNoteNames) {
                if (chordName.Contains(note)) {
                    return note;
                }
            }
            return possibleNoteNames.Last(); // last element as default
        }

        public static String GetNoteLetter(int stringnum, int fret)
        {
            String[,] scale = new String[,] { { "E", "F", "F#", "G", "G#", "A", "A#", "B", "C", "C#", "D", "D#"},
                                              { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#"},
                                              { "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", "C", "C#"},
                                              { "G", "G#", "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#"},
                                              { "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#"},
                                              { "E", "F", "F#", "G", "G#", "A", "A#", "B", "C", "C#", "D", "D#"}};
            int realFret = fret;
            if (realFret >= 12)
            {
                realFret = realFret % 12;
            }
            return scale[stringnum, realFret];
        }
    }
}