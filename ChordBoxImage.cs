/*
 * Chord Image Generator
 * http://tech.einaregilsson.com/2009/07/22/chord-image-generator/
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;

namespace EinarEgilsson.Chords {

    /// <summary>
    /// Class to create images of chordboxes. Can be saved
    /// to any subclass of Stream.
    /// </summary>
    public class ChordBoxImage : IDisposable {

        #region Constants

        const char NO_FINGER = '-';
        const char THUMB = 'T';
        const char INDEX_FINGER = '1';
        const char MIDDLE_FINGER = '2';
        const char RING_FINGER = '3';
        const char LITTLE_FINGER = '4';
        const int OPEN = 0;
        const int MUTED = -1;
        const int FRET_COUNT = 5;
        const string FONT_NAME = "Arial";

        #endregion

        #region Fields
        private Bitmap _bitmap;
        private Graphics _graphics;

        private int _size;
        private int[] _chordPositions = new int[6];
        private char[] _fingers = new char[] { NO_FINGER, NO_FINGER, NO_FINGER,
                                             NO_FINGER, NO_FINGER, NO_FINGER};
        private string _chordName;
        private bool _error;

        private float _fretWidth;
        private int _lineWidth;
        private float _boxWidth;
        private float _boxHeight;

        private int _imageWidth;
        private int _imageHeight;
        private float _xstart; //upper corner of the chordbox
        private float _ystart;
        private float _nutHeight;

        private int _dotWidth;
        private float _signWidth;
        private float _signRadius;

        //Different font sizes
        private float _fretFontSize;
        private float _fingerFontSize;
        private float _nameFontSize;
        private float _superScriptFontSize;
        private float _markerWidth;

        private Brush _foregroundBrush = Brushes.Black;
        private Brush _backgroundBrush = Brushes.White;

        private int _baseFret;

        #endregion

        #region Constructor

        public ChordBoxImage(string name, string chord, string fingers, string size) {
            _chordName = (name == null) ? "" : name.Replace(" ", "");
            ParseChord(chord);
            ParseFingers(fingers);
            ParseSize(size);
            InitializeSizes();
        }

        #endregion

        #region Public methods

        public void Save(Stream output) {
            CreateImage();
            _bitmap.Save(output, ImageFormat.Jpeg);
        }

        public void Dispose() {
            _bitmap.Dispose();
        }

        #endregion

        #region Private methods
        
        private void InitializeSizes() {
            _fretWidth = 4 * _size;
            _nutHeight = _fretWidth / 2f;
            _lineWidth = (int)Math.Ceiling(_size * 0.31);
            _dotWidth = (int)Math.Ceiling(0.9 * _fretWidth);
            _markerWidth = 0.7f * _fretWidth;
            _boxWidth = 5 * _fretWidth + 6 * _lineWidth;
            _boxHeight = FRET_COUNT * (_fretWidth + _lineWidth) + _lineWidth;

            //Find out font sizes
            FontFamily family = new FontFamily(FONT_NAME);
            float perc = family.GetCellAscent(FontStyle.Regular) / (float)family.GetLineSpacing(FontStyle.Regular);
            _fretFontSize = _fretWidth / perc;
            _fingerFontSize = _fretWidth * 0.8f;
            _nameFontSize = _fretWidth * 2f / perc;
            _superScriptFontSize = 0.7f * _nameFontSize;
            if (_size == 1) {
                _nameFontSize += 2;
                _fingerFontSize += 2;
                _fretFontSize += 2;
                _superScriptFontSize += 2;
            }

            _xstart = _fretWidth;
            _ystart = 0.2f * _superScriptFontSize + _nameFontSize + _nutHeight + 1.7f * _markerWidth;

            _imageWidth = (int)(_boxWidth + 5 * _fretWidth);
            _imageHeight = (int)(_boxHeight + _ystart + _fretWidth + _fretWidth);

            _signWidth = (int)(_fretWidth * 0.75);
            _signRadius = _signWidth / 2;
        }

        private void ParseSize(string size) {
            if (size == null) {
                _size = 1;
            } else {
                double dsize;
                if (double.TryParse(size, out dsize)) {
                    dsize = Math.Round(dsize, 0);
                    _size = Convert.ToInt32(Math.Min(Math.Max(1, dsize), 10));
                } else {
                    _size = 1;
                }
            }
        }

        private void ParseFingers(string fingers) {
            if (fingers == null) {
                return; //Allowed to not specify fingers
            } else if (!Regex.IsMatch(fingers, @"[tT\-1234]{6}")) {
                _error = true;
            } else {
                _fingers = fingers.ToUpper().ToCharArray();
            }
        }

        private void ParseChord(string chord) {
            if (chord == null || !Regex.IsMatch(chord, @"[\dxX]{6}|((1|2)?[\dxX]-){5}(1|2)?[\dxX]")) {
                _error = true;
            } else {
                string[] parts;
                if (chord.Length > 6) {
                    parts = chord.Split('-');
                } else {
                    parts = new string[6];
                    for (int i = 0; i < 6; i++) {
                        parts[i] = chord[i].ToString();
                    }
                }
                int maxFret = 0, minFret = int.MaxValue;
                for (int i = 0; i < 6; i++) {
                    if (parts[i].ToUpper() == "X") {
                        _chordPositions[i] = MUTED;
                    } else {
                        _chordPositions[i] = int.Parse(parts[i]);
                        maxFret = Math.Max(maxFret, _chordPositions[i]);
                        if (_chordPositions[i] != 0) {
                            minFret = Math.Min(minFret, _chordPositions[i]);
                        }
                    }
                }
                if (maxFret <= 5) {
                    _baseFret = 1;
                } else {
                    _baseFret = minFret;
                }
            }
        }

        private void CreateImage() {
            _bitmap = new Bitmap(_imageWidth, _imageHeight);
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.SmoothingMode = SmoothingMode.AntiAlias;
            _graphics.FillRectangle(_backgroundBrush, 0, 0, _bitmap.Width, _bitmap.Height);
            if (_error) {
                //Draw red x
                Pen errorPen = new Pen(Color.Red, 3f);
                _graphics.DrawLine(errorPen, 0f, 0f, _bitmap.Width, _bitmap.Height);
                _graphics.DrawLine(errorPen, 0f, _bitmap.Height, _bitmap.Width, 0);
            } else {
                DrawChordBox();
                DrawChordPositions();
                DrawChordName();
                DrawFingers();
                DrawBars();
            }
        }

        private void DrawChordBox() {
            Pen pen = new Pen(_foregroundBrush, _lineWidth);
            float totalFretWidth = _fretWidth + _lineWidth;

            for (int i = 0; i <= FRET_COUNT; i++) {
                float y = _ystart + i * totalFretWidth;
                _graphics.DrawLine(pen, _xstart, y, _xstart + _boxWidth - _lineWidth, y);
            }

            for (int i = 0; i < 6; i++) {
                float x = _xstart + (i * totalFretWidth);
                _graphics.DrawLine(pen, x, _ystart, x, _ystart + _boxHeight - pen.Width);
            }

            if (_baseFret == 1) {
                //Need to draw the nut
                float nutHeight = _fretWidth / 2f;
                _graphics.FillRectangle(_foregroundBrush, _xstart - _lineWidth / 2f, _ystart - nutHeight, _boxWidth, nutHeight);
            }
        }

        private struct Bar { public int Str, Pos, Length; public char Finger; }
        
        private void DrawBars() {
            var bars = new Dictionary<char, Bar>();
            for (int i = 0; i < 5; i++) {
                if (_chordPositions[i] != MUTED && _chordPositions[i] != OPEN && _fingers[i] != NO_FINGER && !bars.ContainsKey(_fingers[i])) {
                    Bar bar = new Bar { Str = i, Pos = _chordPositions[i], Length = 0, Finger = _fingers[i] };
                    for (int j = i + 1; j < 6; j++) {
                        if (_fingers[j] == bar.Finger && _chordPositions[j] == _chordPositions[i]) {
                            bar.Length = j - i;
                        }
                    }
                    if (bar.Length > 0) {
                        bars.Add(bar.Finger, bar);
                    }
                }
            }

            Pen pen = new Pen(_foregroundBrush, _lineWidth * 3);
            float totalFretWidth = _fretWidth + _lineWidth;
            foreach (Bar bar in bars.Values) {
                float xstart = _xstart + bar.Str * totalFretWidth;
                float xend = xstart + bar.Length * totalFretWidth;
                float y = _ystart + (bar.Pos - _baseFret + 1) * totalFretWidth - (totalFretWidth / 2);
                pen = new Pen(_foregroundBrush, _dotWidth / 2);
                _graphics.DrawLine(pen, xstart, y, xend, y);
            }
        }

        private void DrawChordPositions() {
            float yoffset = _ystart - _fretWidth;
            float xoffset = _lineWidth / 2f;
            float totalFretWidth = _fretWidth + _lineWidth;
            float xfirstString = _xstart + 0.5f * _lineWidth;
            for (int i = 0; i < _chordPositions.Length; i++) {
                int absolutePos = _chordPositions[i];
                int relativePos = absolutePos - _baseFret + 1;

                float xpos = _xstart - (0.5f * _fretWidth) + (0.5f * _lineWidth) + (i * totalFretWidth);
                if (relativePos > 0) {
                    float ypos = relativePos * totalFretWidth + yoffset;
                    _graphics.FillEllipse(_foregroundBrush, xpos, ypos, _dotWidth, _dotWidth);
                } else if (absolutePos == OPEN) {
                    Pen pen = new Pen(_foregroundBrush, _lineWidth);
                    float ypos = _ystart - _fretWidth;
                    float markerXpos = xpos + ((_dotWidth - _markerWidth) / 2f);
                    if (_baseFret == 1) {
                        ypos -= _nutHeight;
                    }
                    _graphics.DrawEllipse(pen, markerXpos, ypos, _markerWidth, _markerWidth);
                } else if (absolutePos == MUTED) {
                    Pen pen = new Pen(_foregroundBrush, _lineWidth * 1.5f);
                    float ypos = _ystart - _fretWidth;
                    float markerXpos = xpos + ((_dotWidth - _markerWidth) / 2f);
                    if (_baseFret == 1) {
                        ypos -= _nutHeight;
                    }
                    _graphics.DrawLine(pen, markerXpos, ypos, markerXpos + _markerWidth, ypos + _markerWidth);
                    _graphics.DrawLine(pen, markerXpos, ypos + _markerWidth, markerXpos + _markerWidth, ypos);
                }
            }
        }

        private void DrawFingers() {
            float xpos = _xstart + (0.5f * _lineWidth);
            float ypos = _ystart + _boxHeight;
            Font font = new Font(FONT_NAME, _fingerFontSize);
            foreach (char finger in _fingers) {
                if (finger != NO_FINGER) {
                    SizeF charSize = _graphics.MeasureString(finger.ToString(), font);
                    _graphics.DrawString(finger.ToString(), font, _foregroundBrush, xpos - (0.5f * charSize.Width), ypos);
                }
                xpos += (_fretWidth + _lineWidth);
            }
        }

        private void DrawChordName() {

            Font nameFont = new Font(FONT_NAME, _nameFontSize, GraphicsUnit.Pixel);
            Font superFont = new Font(FONT_NAME, _superScriptFontSize, GraphicsUnit.Pixel);
            String name, super;
            if (_chordName.IndexOf('_') == -1) {
                name = _chordName;
                super = "";
            } else {
                string[] parts = _chordName.Split('_');
                name = parts[0];
                super = parts[1];
            }
            SizeF stringSize = _graphics.MeasureString(name, nameFont);

            float xTextStart = _xstart;
            if (stringSize.Width < _boxWidth) {
                xTextStart = _xstart + ((_boxWidth - stringSize.Width) / 2f);
            }
            _graphics.DrawString(name, nameFont, _foregroundBrush, xTextStart, 0.2f * _superScriptFontSize);
            if (super != "") {
                _graphics.DrawString(super, superFont, _foregroundBrush, xTextStart + 0.8f * stringSize.Width, 0);
            }

            if (_baseFret > 1) {
                Font fretFont = new Font(FONT_NAME, _fretFontSize, GraphicsUnit.Pixel);
                float offset = (fretFont.Size - _fretWidth) / 2f;
                _graphics.DrawString(_baseFret + "fr", fretFont, _foregroundBrush, _xstart + _boxWidth + 0.3f * _fretWidth, _ystart - offset);
            }
        }

        #endregion
    }
}
