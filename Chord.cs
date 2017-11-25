using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace EinarEgilsson.Chords
{
    public class Chord
    {
        const char NO_FINGER = '-';
        const char THUMB = 'T';
        const char INDEX_FINGER = '1';
        const char MIDDLE_FINGER = '2';
        const char RING_FINGER = '3';
        const char LITTLE_FINGER = '4';
        const int OPEN = 0;
        const int MUTED = -1;
        const int FRET_COUNT = 5;

        private int[] _chordPositions;
        private char[] _fingers = new char[] { NO_FINGER, NO_FINGER, NO_FINGER,
                                             NO_FINGER, NO_FINGER, NO_FINGER};
        private int _baseFret;
        private bool _error;

        public String Name { get; set; }
        public enum FrettingMode { Muted = -1, Open = 0, Fretted }
        public int NumberOfStrings { get; } = 6;
        public int BaseFret
        {
            get { return this._baseFret; }
        }

        #region initializers

        public Chord(String name = "", String parseString = "", String fingers = "") {
            _chordPositions = new int[NumberOfStrings];

            if (parseString != null && !"".Equals(parseString))
            {               
                ParseChord(parseString);
            }
            if (name != null && !"".Equals(name))
            {
                Name = ParseName(name);
            }
            if (fingers != null && !"".Equals(fingers))
            {
                ParseFingers(fingers);
            }
        }

        public Chord parseChordFromString(String chordString) {
            Chord newChord = new Chord(ParseName(chordString));
            return newChord;
        }

        #endregion

        #region staticmethods
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
                    return returnFirstMatch(chordName, new string[] { "b2", "b9" });
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

        #endregion

        #region public Interface

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

        public int getFretNumberOnString(int stringNumber)
        {
            return _chordPositions[stringNumber];
        }

        public FrettingMode getFrettingModeOnString(int stringNumber)
        {
            if (_chordPositions[stringNumber] == MUTED)
            {
                return FrettingMode.Muted;
            }
            else if (_chordPositions[stringNumber] == OPEN)
            {
                return FrettingMode.Open;
            }
            else
            {
                return FrettingMode.Fretted;
            }
        }

        public char getUsedFingerOnString(int stringNumber)
        {
            return _fingers[stringNumber];
        }

        #endregion

        #region private methods
        private static String returnFirstMatch(String chordName, String[] possibleNoteNames)
        {
            foreach (String note in possibleNoteNames)
            {
                if (chordName.Contains(note))
                {
                    return note;
                }
            }
            return possibleNoteNames.Last(); // last element as default
        }

        private string ParseName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "";
            }
            var splitString = name.Split('_');
            for (int i = 1; i < splitString.Length; i++)
            {
                if (i % 2 == 0)
                {
                    continue;
                }
                splitString[i] = ConvertSharpSign(splitString[i]);
                splitString[i] = ConvertFlatSign(splitString[i]);
            }
            return string.Join("_", splitString);
        }

        private static string ConvertSharpSign(string name)
        {
            if (name.Length > 1)
            {
                return name;
            }
            return name.Replace("#", "\u266f");
        }

        private static string ConvertFlatSign(string name)
        {
            if (name.Length > 1)
            {
                return name;
            }
            name = name.Replace("b", "\u266d");
            return name.Replace("B", "\u266d");
        }

        private void ParseChord(string chord)
        {
            if (chord == null || !Regex.IsMatch(chord, @"[\dxX]{6}|((1|2)?[\dxX]-){5}(1|2)?[\dxX]"))
            {
                _error = true;
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
                    if (parts[i].ToUpper() == "X")
                    {
                        _chordPositions[i] = MUTED;
                    }
                    else
                    {
                        _chordPositions[i] = int.Parse(parts[i]);
                        maxFret = Math.Max(maxFret, _chordPositions[i]);
                        if (_chordPositions[i] != 0)
                        {
                            minFret = Math.Min(minFret, _chordPositions[i]);
                        }
                    }
                }
                if (maxFret <= 5)
                {
                    _baseFret = 1;
                }
                else
                {
                    _baseFret = minFret;
                }
            }
        }

        private void ParseFingers(string fingers)
        {
            if (fingers == null)
            {
                return; //Allowed to not specify fingers
            }
            else if (!Regex.IsMatch(fingers, @"[tT\-1234]{6}"))
            {
                _error = true;
            }
            else
            {
                _fingers = fingers.ToUpper().ToCharArray();
            }
        }
        #endregion
    }
}