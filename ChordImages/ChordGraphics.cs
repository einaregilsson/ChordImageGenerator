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

namespace EinarEgilsson.ChordImages
{
    internal class ChordGraphics : IDisposable
    {
        #region Constants

        private const int FRET_COUNT = 5;
        private const string FONT_NAME = "Arial";

        #endregion

        #region Fields

        private Bitmap _bitmap;
        private Graphics _graphics;

        private int _size;

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

        private readonly Brush _foregroundBrush = Brushes.Black;
        private readonly Brush _backgroundBrush = Brushes.White;

        #endregion

        #region Constructor

        internal ChordGraphics(string sizeString)
        {
            ParseSize(sizeString);
            InitializeSizes();
        }

        #endregion

        #region Public and Internal Methods

        internal void Save(Stream output)
        {
            _bitmap.Save(output, ImageFormat.Png);
        }

        public void Dispose()
        {
            _bitmap.Dispose();
            _graphics.Dispose();
        }

        #endregion

        #region Private Methods

        private void ParseSize(string size)
        {
            if (string.IsNullOrEmpty(size))
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

        #region Initialization

        private void InitializeSizes()
        {
            _fretWidth = 4 * _size;
            _nutHeight = _fretWidth / 2f;
            _lineWidth = (int)Math.Ceiling(_size * 0.31);
            _dotWidth = (int)Math.Ceiling(0.9 * _fretWidth);
            _markerWidth = 0.7f * _fretWidth;
            _boxWidth = 5 * _fretWidth + 6 * _lineWidth;
            _boxHeight = FRET_COUNT * (_fretWidth + _lineWidth) + _lineWidth;

            //Find out font sizes
            var family = new FontFamily(FONT_NAME);
            float perc = family.GetCellAscent(FontStyle.Regular) / (float)family.GetLineSpacing(FontStyle.Regular);
            _fretFontSize = _fretWidth / perc;
            _fingerFontSize = _fretWidth * 0.8f;
            _nameFontSize = _fretWidth * 2f / perc;
            _superScriptFontSize = 0.7f * _nameFontSize;
            if (_size == 1)
            {
                _nameFontSize += 2;
                _fingerFontSize += 2;
                _fretFontSize += 2;
                _superScriptFontSize += 2;
            }

            _xstart = _fretWidth;
            _ystart = (float)Math.Round(0.2f * _superScriptFontSize + _nameFontSize + _nutHeight + 1.7f * _markerWidth);

            _imageWidth = (int)(_boxWidth + 5 * _fretWidth);
            _imageHeight = (int)(_boxHeight + _ystart + _fretWidth + _fretWidth);

            _signWidth = (int)(_fretWidth * 0.75);
            _signRadius = _signWidth / 2;
        }

        #endregion

        #region Draw Image

        internal void CreateImage(Chord chord, Fingers fingers)
        {
            _bitmap = new Bitmap(_imageWidth, _imageHeight);
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.SmoothingMode = SmoothingMode.HighQuality;
            _graphics.FillRectangle(_backgroundBrush, 0, 0, _bitmap.Width, _bitmap.Height);
            if (chord.ParseError || fingers.ParseError)
            {
                //Draw red x
                var errorPen = new Pen(Color.Red, 3f);
                _graphics.DrawLine(errorPen, 0f, 0f, _bitmap.Width, _bitmap.Height);
                _graphics.DrawLine(errorPen, 0f, _bitmap.Height, _bitmap.Width, 0);
            }
            else
            {
                DrawChordBox(chord.NeedToDrawNut());
                DrawChordPositions(chord);
                DrawChordName(chord);
                DrawFingers(fingers);
                DrawBars(chord, fingers);
            }
        }

        private void DrawChordBox(bool isOnFirstFret)
        {
            var pen = new Pen(_foregroundBrush, _lineWidth);
            float totalFretWidth = _fretWidth + _lineWidth;

            for (int i = 0; i <= FRET_COUNT; i++)
            {
                float y = _ystart + i * totalFretWidth;
                _graphics.DrawLine(pen, _xstart, y, _xstart + _boxWidth - _lineWidth, y);
            }

            for (int i = 0; i < 6; i++)
            {
                float x = _xstart + (i * totalFretWidth);
                _graphics.DrawLine(pen, x, _ystart, x, _ystart + _boxHeight - pen.Width);
            }

            if (isOnFirstFret)
            {
                //Need to draw the nut
                float nutHeight = _fretWidth / 2f;
                _graphics.FillRectangle(_foregroundBrush, _xstart - _lineWidth / 2f, _ystart - nutHeight, _boxWidth, nutHeight);
            }
        }

        private void DrawChordPositions(Chord chord)
        {
            float yoffset = _ystart - _fretWidth;
            float xoffset = _lineWidth / 2f;
            float totalFretWidth = _fretWidth + _lineWidth;
            float xfirstString = _xstart + 0.5f * _lineWidth;
            for (int i = 0; i < chord.Length; i++)
            {
                var absolutePos = chord.PlaystyleAt(i);
                int relativePos = (int)absolutePos - chord.BaseFret + 1;

                float xpos = _xstart - (0.5f * _fretWidth) + (0.5f * _lineWidth) + (i * totalFretWidth);
                if (relativePos > 0)
                {
                    float ypos = relativePos * totalFretWidth + yoffset;
                    _graphics.FillEllipse(_foregroundBrush, xpos, ypos, _dotWidth, _dotWidth);
                }
                else if (absolutePos == Playstyle.Open)
                {
                    var pen = new Pen(_foregroundBrush, _lineWidth);
                    float ypos = _ystart - _fretWidth;
                    float markerXpos = xpos + ((_dotWidth - _markerWidth) / 2f);
                    if (chord.NeedToDrawNut())
                    {
                        ypos -= _nutHeight;
                    }
                    _graphics.DrawEllipse(pen, markerXpos, ypos, _markerWidth, _markerWidth);
                }
                else if (absolutePos == Playstyle.Muted)
                {
                    var pen = new Pen(_foregroundBrush, _lineWidth * 1.5f);
                    float ypos = _ystart - _fretWidth;
                    float markerXpos = xpos + ((_dotWidth - _markerWidth) / 2f);
                    if (chord.NeedToDrawNut())
                    {
                        ypos -= _nutHeight;
                    }
                    _graphics.DrawLine(pen, markerXpos, ypos, markerXpos + _markerWidth, ypos + _markerWidth);
                    _graphics.DrawLine(pen, markerXpos, ypos + _markerWidth, markerXpos + _markerWidth, ypos);
                }
            }
        }

        private void DrawChordName(Chord chord)
        {
            var nameFont = new Font(FONT_NAME, _nameFontSize, GraphicsUnit.Pixel);
            var superFont = new Font(FONT_NAME, _superScriptFontSize, GraphicsUnit.Pixel);
            string[] parts = chord.Name.Split('_');
            float xTextStart = _xstart;

            //Set max parts to 4 for protection
            int maxParts = parts.Length;
            if (maxParts > 4)
            {
                maxParts = 4;
            }

            //count total width of the chord in pixels
            float chordNameSize = 0;
            for (int i = 0; i < maxParts; i++)
            {
                if (i % 2 == 0)
                {  //odd parts are normal text
                    SizeF stringSize2 = _graphics.MeasureString(parts[i], nameFont);
                    chordNameSize += 0.75f * stringSize2.Width;
                }
                else
                {    //even parts are superscipts
                    SizeF stringSize2 = _graphics.MeasureString(parts[i], superFont);
                    chordNameSize += 0.8f * stringSize2.Width;
                }
            }

            //set the x position for the chord name
            if (chordNameSize < _boxWidth)
            {
                xTextStart = _xstart + ((_boxWidth - chordNameSize) / 2f);
            }
            else if ((xTextStart + chordNameSize) > _imageWidth)
            {   // if it goes outside the boundaries
                var nx = (xTextStart + chordNameSize) / 2f;
                if (nx < _imageWidth / 2)
                {                         // if it can fit inside the image
                    xTextStart = (_imageWidth / 2) - nx;
                }
                else
                {
                    xTextStart = 2f;
                }
            }

            // Paint the chord
            for (int i = 0; i < maxParts; i++)
            {
                if (i % 2 == 0)
                {
                    var stringSize2 = _graphics.MeasureString(parts[i], nameFont);
                    _graphics.DrawString(parts[i], nameFont, _foregroundBrush, xTextStart, 0.2f * _superScriptFontSize);
                    xTextStart += 0.75f * stringSize2.Width;
                }
                else
                {
                    var stringSize2 = _graphics.MeasureString(parts[i], superFont);
                    _graphics.DrawString(parts[i], superFont, _foregroundBrush, xTextStart, 0);
                    xTextStart += 0.8f * stringSize2.Width;
                }
            }

            if (chord.BaseFret > 1)
            {
                var fretFont = new Font(FONT_NAME, _fretFontSize, GraphicsUnit.Pixel);
                float offset = (fretFont.Size - _fretWidth) / 2f;
                _graphics.DrawString(chord.BaseFret + "fr", fretFont, _foregroundBrush, _xstart + _boxWidth + 0.3f * _fretWidth, _ystart - offset);
            }
        }

        private void DrawFingers(Fingers fingers)
        {
            var xpos = _xstart + (0.5f * _lineWidth);
            var ypos = _ystart + _boxHeight;
            var font = new Font(FONT_NAME, _fingerFontSize);
            for (int i = 0; i < fingers.FingerPositions.Length; i++)
            {
                if (fingers.IsFingerAt(i))
                {
                    SizeF charSize = _graphics.MeasureString(fingers.PrintAt(i), font);
                    _graphics.DrawString(fingers.PrintAt(i), font, _foregroundBrush, xpos - (0.5f * charSize.Width), ypos);
                }
                xpos += (_fretWidth + _lineWidth);
            }
        }

        private void DrawBars(Chord chord, Fingers fingers)
        {
            var bars = Bars.GetBars(chord, fingers);
            float totalFretWidth = _fretWidth + _lineWidth;
            float arcWidth = _dotWidth / 7;
            foreach (Bar bar in bars)
            {
                DrawSingleBar(bar, chord.BaseFret, totalFretWidth, arcWidth);
            }
        }

        private void DrawSingleBar(Bar bar, int baseFret, float totalFretWidth, float arcWidth)
        {
            float yTempOffset = 0.0f;

            if (bar.Pos == 1)
            {  // the bar must go a little higher in order to be shown correctly
                yTempOffset = -0.3f * totalFretWidth;
            }

            float xstart = _xstart + bar.Str * totalFretWidth - (_dotWidth / 2);
            float y = _ystart + (bar.Pos - baseFret) * totalFretWidth - (0.6f * totalFretWidth) + yTempOffset;
            var pen = new Pen(_foregroundBrush, arcWidth);
            var pen2 = new Pen(_foregroundBrush, 1.3f * arcWidth);

            float barWidth = bar.Length * totalFretWidth + _dotWidth;

            _graphics.DrawArc(pen, xstart, y, barWidth, totalFretWidth, -1, -178);
            _graphics.DrawArc(pen2, xstart, y - arcWidth, barWidth, totalFretWidth + arcWidth, -4, -172);
            _graphics.DrawArc(pen2, xstart, y - 1.5f * arcWidth, barWidth, totalFretWidth + 3 * arcWidth, -20, -150);
        }

        #endregion

        #endregion
    }
}
