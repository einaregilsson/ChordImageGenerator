using Microsoft.VisualStudio.TestTools.UnitTesting;
using EinarEgilsson.Chords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EinarEgilsson.Chords.Tests
{
    [TestClass()]
    public class ChordTests
    {
        [TestMethod()]
        public void getRootNoteTestBang()
        {
            Chord c = new Chord("C#");
            Assert.AreEqual("C#", c.getRootNote());
        }

        [TestMethod()]
        public void getRootNoteTestBare()
        {
            Chord c = new Chord("C");
            Assert.AreEqual("C", c.getRootNote());
        }

        [TestMethod()]
        public void getRootNoteTestb()
        {
            Chord c = new Chord("Cb");
            Assert.AreEqual("Cb", c.getRootNote());
        }

        [TestMethod()]
        public void getIntervalRoot()
        {
            Chord c = new Chord("C");
            Assert.AreEqual("r", c.getIntervalFromRootNote("C"));
        }

        [TestMethod()]
        public void getIntervalRootAnotherChord()
        {
            Chord c = new Chord("E");
            Assert.AreEqual("r", c.getIntervalFromRootNote("E"));
        }

        [TestMethod()]
        public void getIntervalRootAnotherChord2()
        {
            Chord c = new Chord("D#");
            Assert.AreEqual("r", c.getIntervalFromRootNote("D#"));
        }

        [TestMethod()]
        public void getIntervalm6()
        {
            Chord c = new Chord("C");
            Assert.AreEqual("m6", c.getIntervalFromRootNote("G#"));
        }

        [TestMethod()]
        public void getIntervalp5()
        {
            Chord c = new Chord("C+5");
            Assert.AreEqual("+5", c.getIntervalFromRootNote("G#"));
        }

        [TestMethod()]
        public void getIntervalp5bang()
        {
            Chord c = new Chord("C5#");
            Assert.AreEqual("5#", c.getIntervalFromRootNote("G#"));
        }

        /* Can't really tell the diff between (Cb)13 and (C)b13
        [TestMethod()]
        public void getIntervalb13()
        {
            Chord c = new Chord("Cb13");
            Assert.AreEqual("b13", c.getIntervalFromRootNote("G#"));
        }
        */
    }
}