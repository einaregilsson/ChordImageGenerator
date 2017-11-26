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

        const int FRET_COUNT = 5;
        const string FONT_NAME = "Arial";

        #endregion

        #region Fields
        private Bitmap _bitmap;
        private Graphics _graphics;

        private int _size;
        private bool _error;
        private Chord _chord;

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
        private float _noteFontSize;
        private float _intervalFontSize;

        private float _nameFontSize;
        private float _superScriptFontSize;
        private float _markerWidth;

        private Brush _foregroundBrush = Brushes.Black;
        private Brush _backgroundBrush = Brushes.White;

        private bool _expandedInfo;

        #endregion

        #region Constructor

        public ChordBoxImage(string name, string chord, string fingers, string size, string expanded) {
            try
            {
                _chord = new Chord(name: name, parseString: chord, fingers: fingers);
            } catch (ArgumentException)
            {
                _error = true;
            }

            bool result = bool.TryParse(expanded, out _expandedInfo);
            if (!result)
            {
                _expandedInfo = false;
            }

            ParseSize(size);
            InitializeSizes();
            CreateImage();
        }

        #endregion

        #region Public methods

        public void Save(Stream output) {
            _bitmap.Save(output, ImageFormat.Png);
        }

        public byte[]  GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                byte[] buffer = new byte[ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
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
            _intervalFontSize = _fingerFontSize -1;
            _noteFontSize = _fingerFontSize -1;
            _nameFontSize = _fretWidth * 2f / perc;
            _superScriptFontSize = 0.7f * _nameFontSize;
            if (_size == 1) {
                _nameFontSize += 2;
                _fingerFontSize += 2;
                _intervalFontSize += 2;
                _noteFontSize += 2;
                _fretFontSize += 2;
                _superScriptFontSize += 2;
            }

            //Where to start the chordbox
            _xstart = _fretWidth;
            _ystart = (float)Math.Round(0.2f * _superScriptFontSize + _nameFontSize + _nutHeight + 1.7f * _markerWidth);

            _imageWidth = (int)(_boxWidth + 5 * _fretWidth);
            _imageHeight = (int)(_boxHeight + _ystart + _fretWidth + _fretWidth + _intervalFontSize + _noteFontSize);

            _signWidth = (int)(_fretWidth * 0.75);
            _signRadius = _signWidth / 2;
        }
        
        private void ParseSize(string size)
        {
            if (size == null)
            {
                _size = 1;
            }
            else
            {
                double dsize;
                if (double.TryParse(size, out dsize))
                {
                    dsize = Math.Round(dsize, 0);
                    _size = Convert.ToInt32(Math.Min(Math.Max(1, dsize), 10));
                }
                else
                {
                    _size = 1;
                }
            }
        }

        private void CreateImage() {
            _bitmap = new Bitmap(_imageWidth, _imageHeight);
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.SmoothingMode = SmoothingMode.HighQuality;
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
                if (_expandedInfo)
                {
                    DrawNotes();
                    DrawIntervals();
                }
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

            if (_chord.BaseFret == 1) {
                //Need to draw the nut
                float nutHeight = _fretWidth / 2f;
                _graphics.FillRectangle(_foregroundBrush, _xstart - _lineWidth / 2f, _ystart - nutHeight, _boxWidth, nutHeight);
            }
        }

        private struct Bar { public int Str, Pos, Length; public char Finger; }
        
        private void DrawBars() {
            var bars = new Dictionary<char, Bar>();
            for (int i = 0; i < 5; i++) {
                int firstFretPosition = _chord.getFretNumberOnString(i);
                char firstFinger = _chord.getUsedFingerOnString(i);
                Chord.FrettingMode frettingMode = _chord.getFrettingModeOnString(i);
                if (frettingMode != Chord.FrettingMode.Muted && frettingMode != Chord.FrettingMode.Open && !bars.ContainsKey(firstFinger)) {
                    Bar bar = new Bar { Str = i, Pos = firstFretPosition, Length = 0, Finger = firstFinger };
                    for (int j = i + 1; j < 6; j++) {
                        int nextFretPosition = _chord.getFretNumberOnString(j);
                        int nextFinger = _chord.getUsedFingerOnString(j);
                        if (nextFinger == bar.Finger && nextFretPosition == firstFretPosition) {
                            bar.Length = j - i;
                        }
                    }
                    if (bar.Length > 0) {
                        bars.Add(bar.Finger, bar);
                    }
                }
            }

            float totalFretWidth = _fretWidth + _lineWidth;
            float arcWidth = _dotWidth / 7;
            foreach (Bar bar in bars.Values) {                
                float yTempOffset = 0.0f;
                
                if (bar.Pos == 1) {  // the bar must go a little higher in order to be shown correctly
                    yTempOffset = - 0.3f * totalFretWidth ;
                }

                float xstart = _xstart + bar.Str * totalFretWidth - (_dotWidth / 2);
                float y = _ystart + (bar.Pos - _chord.BaseFret) * totalFretWidth - (0.6f * totalFretWidth) + yTempOffset;
                Pen pen = new Pen(_foregroundBrush, arcWidth);
                Pen pen2 = new Pen(_foregroundBrush, 1.3f * arcWidth);
                //_graphics.DrawLine(pen, xstart, y, xend, y);

                float barWidth = bar.Length * totalFretWidth + _dotWidth;

                _graphics.DrawArc(pen, xstart, y, barWidth, totalFretWidth, -1, -178);
                _graphics.DrawArc(pen2, xstart, y - arcWidth, barWidth, totalFretWidth + arcWidth, -4, -172);
                _graphics.DrawArc(pen2, xstart, y - 1.5f * arcWidth, barWidth, totalFretWidth + 3 * arcWidth, -20, -150);
            }
        }

        //Seems to draw both the top text and the finger dots
        private void DrawChordPositions() {
            float yoffset = _ystart - _fretWidth;
            float xoffset = _lineWidth / 2f;
            float totalFretWidth = _fretWidth + _lineWidth;
            float xfirstString = _xstart + 0.5f * _lineWidth;
            for (int i = 0; i < _chord.NumberOfStrings; i++) {
                int absolutePos = _chord.getFretNumberOnString(i);
                int relativePos = absolutePos - _chord.BaseFret + 1;
                Chord.FrettingMode frettingMode = _chord.getFrettingModeOnString(i);

                float xpos = _xstart - (0.5f * _fretWidth) + (0.5f * _lineWidth) + (i * totalFretWidth);
                if (relativePos > 0) {
                    float ypos = relativePos * totalFretWidth + yoffset;
                    _graphics.FillEllipse(_foregroundBrush, xpos, ypos, _dotWidth, _dotWidth);
                } else if (frettingMode == Chord.FrettingMode.Open && !_expandedInfo)
                {
                    Pen pen = new Pen(_foregroundBrush, _lineWidth);
                    float ypos = _ystart - _fretWidth;
                    float markerXpos = xpos + ((_dotWidth - _markerWidth) / 2f);
                    if (_chord.BaseFret == 1)
                    {
                        ypos -= _nutHeight;
                    }
                    _graphics.DrawEllipse(pen, markerXpos, ypos, _markerWidth, _markerWidth);
                } else if (frettingMode == Chord.FrettingMode.Muted && !_expandedInfo)
                {
                    Pen pen = new Pen(_foregroundBrush, _lineWidth * 1.5f);
                    float ypos = _ystart - _fretWidth;
                    float markerXpos = xpos + ((_dotWidth - _markerWidth) / 2f);
                    if (_chord.BaseFret == 1)
                    {
                        ypos -= _nutHeight;
                    }
                    _graphics.DrawLine(pen, markerXpos, ypos, markerXpos + _markerWidth, ypos + _markerWidth);
                    _graphics.DrawLine(pen, markerXpos, ypos + _markerWidth, markerXpos + _markerWidth, ypos);
                }
            }
        }

        //Draws the text above the box specifying finger numbers
        private void DrawFingers() {            
            float xpos = _xstart + (0.5f * _lineWidth);
            float ypos;
            if (_expandedInfo)
            {
                ypos = _ystart - _fretWidth - 3;
                if (_chord.BaseFret == 1)
                {
                    ypos -= _nutHeight;
                }
            }
            else
            {
                ypos = _ystart + _boxHeight;
            }

            Font font = new Font(FONT_NAME, _fingerFontSize);
            for(int i = 0; i < _chord.NumberOfStrings; i++) {
                Chord.FrettingMode frettingMode = _chord.getFrettingModeOnString(i);
                char finger = _chord.getUsedFingerOnString(i);
                if (frettingMode == Chord.FrettingMode.Fretted) {
                    SizeF charSize = _graphics.MeasureString(finger.ToString(), font);
                    _graphics.DrawString(finger.ToString(), font, _foregroundBrush, xpos - (0.5f * charSize.Width), ypos);
                } else if (frettingMode == Chord.FrettingMode.Open && _expandedInfo)
                {
                    SizeF charSize = _graphics.MeasureString("O", font);
                    _graphics.DrawString("O", font, _foregroundBrush, xpos - (0.5f * charSize.Width), ypos);
                } else if (frettingMode == Chord.FrettingMode.Muted && _expandedInfo) {
                        SizeF charSize = _graphics.MeasureString("X", font);
                        _graphics.DrawString("X", font, _foregroundBrush, xpos - (0.5f * charSize.Width), ypos);
                }
                xpos += (_fretWidth + _lineWidth);
            }
        }

        private void DrawNotes() {
            float xpos = _xstart + (0.5f * _lineWidth);
            float ypos = _ystart + _boxHeight;
            Font font = new Font(FONT_NAME, _noteFontSize);
            for (int i = 0; i < _chord.NumberOfStrings; i++) {
                int absolutePos = _chord.getFretNumberOnString(i);
                Chord.FrettingMode frettingMode = _chord.getFrettingModeOnString(i);

                if (frettingMode != Chord.FrettingMode.Muted)
                {
                    String noteLetter = Chord.GetNoteLetter(i, absolutePos);
                    SizeF charSize = _graphics.MeasureString(noteLetter, font);
                    _graphics.DrawString(noteLetter, font, _foregroundBrush, xpos - (0.5f * charSize.Width), ypos);
                }
                xpos += (_fretWidth + _lineWidth);
            }
        }

        private void DrawIntervals()
        {
            float xpos = _xstart + (0.5f * _lineWidth);

            Font intervalFont = new Font(FONT_NAME, _intervalFontSize);

            Font noteFont = new Font(FONT_NAME, _noteFontSize);
            SizeF noteCharSize = _graphics.MeasureString("X", noteFont);
            float notecharHeight = noteCharSize.Height;

            //The line above are the notes, so use that char size to find out how 
            //far down to start the next line
            float ypos = _ystart + _boxHeight + notecharHeight - 2;

            for (int i = 0; i < _chord.NumberOfStrings; i++)
            {
                int absolutePos = _chord.getFretNumberOnString(i);
                Chord.FrettingMode frettingMode = _chord.getFrettingModeOnString(i);
                if (frettingMode != Chord.FrettingMode.Muted) {
                    String rootNote = _chord.getRootNote();
                    String noteLetter = Chord.GetNoteLetter(i, absolutePos);
                    String noteInterval = _chord.getIntervalFromRootNote(noteLetter);
                    SizeF charSizeIndividual = _graphics.MeasureString(noteInterval, intervalFont);
                    _graphics.DrawString(noteInterval, intervalFont, _foregroundBrush, xpos - (0.5f * charSizeIndividual.Width), ypos);
                }
                xpos += (_fretWidth + _lineWidth);
            }
        }

        private void DrawChordName() {
            Font nameFont = new Font(FONT_NAME, _nameFontSize, GraphicsUnit.Pixel);
            Font superFont = new Font(FONT_NAME, _superScriptFontSize, GraphicsUnit.Pixel);
            string[] parts = _chord.Name.Split('_');
            float xTextStart = _xstart;

            //Set max parts to 4 for protection
            int maxParts = parts.Length;
            if ( maxParts > 4 ) {
                maxParts = 4;
            }

            //count total width of the chord in pixels
            float chordNameSize = 0;
            for (int i = 0; i < maxParts; i++)
            {
                if (i % 2 == 0) {  //odd parts are normal text
                    SizeF stringSize2 = _graphics.MeasureString(parts[i], nameFont);
                    chordNameSize += 0.75f * stringSize2.Width;
                }
                else {    //even parts are superscipts
                    SizeF stringSize2 = _graphics.MeasureString(parts[i], superFont);
                    chordNameSize += 0.8f * stringSize2.Width;
                }
            }

            //set the x position for the chord name
            if (chordNameSize < _boxWidth) {
                xTextStart = _xstart + ((_boxWidth - chordNameSize) / 2f);
            }
            else if((xTextStart + chordNameSize) > _imageWidth ) {   // if it goes outside the boundaries
                float nx = (xTextStart + chordNameSize) / 2f;
                if (nx < _imageWidth / 2) {                         // if it can fit inside the image
                    xTextStart = (_imageWidth / 2) - nx;
                }
                else {
                    xTextStart = 2f;
                }
            }

            // Paint the chord
            for (int i = 0; i < maxParts; i++) {
                if (i % 2 == 0) {
                    SizeF stringSize2 = _graphics.MeasureString(parts[i], nameFont);
                    _graphics.DrawString(parts[i], nameFont, _foregroundBrush, xTextStart, 0.2f * _superScriptFontSize);
                    xTextStart += 0.75f * stringSize2.Width;
                }
                else {
                    SizeF stringSize2 = _graphics.MeasureString(parts[i], superFont);
                    _graphics.DrawString(parts[i], superFont, _foregroundBrush, xTextStart, 0);
                    xTextStart += 0.8f * stringSize2.Width;
                }
            }

            if (_chord.BaseFret > 1) {
                Font fretFont = new Font(FONT_NAME, _fretFontSize, GraphicsUnit.Pixel);
                float offset = (fretFont.Size - _fretWidth) / 2f;
                _graphics.DrawString(_chord.BaseFret + "fr", fretFont, _foregroundBrush, _xstart + _boxWidth + 0.3f * _fretWidth, _ystart - offset);
            }
        }

        #endregion
    }
}
