using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EinarEgilsson.Chords.Tests
{
    [TestClass()]
    public class ChordTests
    {
        #region RootNoteTests

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

        #endregion

        #region intervalTests

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

        #endregion

        #region NoteTests

        [TestMethod()]
        public void getSixthStringFirstFret()
        {
            Assert.AreEqual("E", Chord.GetNoteLetter(5, 0));
        }

        [TestMethod()]
        public void getSixthStringTwelfthFret()
        {
            Assert.AreEqual("E", Chord.GetNoteLetter(5, 12));
        }


        [TestMethod()]
        public void getFifthStringFirstFret()
        {
            Assert.AreEqual("B", Chord.GetNoteLetter(4, 0));
        }

        [TestMethod()]
        public void getFifthStringTwelfthFret()
        {
            Assert.AreEqual("B", Chord.GetNoteLetter(4, 12));
        }

        [TestMethod()]
        public void getFourthStringFirstFret()
        {
            Assert.AreEqual("G", Chord.GetNoteLetter(3, 0));
        }

        [TestMethod()]
        public void getFourthStringTwelfthFret()
        {
            Assert.AreEqual("G", Chord.GetNoteLetter(3, 12));
        }

        [TestMethod()]
        public void getThirdStringFirstFret()
        {
            Assert.AreEqual("D", Chord.GetNoteLetter(2, 0));
        }

        [TestMethod()]
        public void getThirdStringTwelfthFret()
        {
            Assert.AreEqual("D", Chord.GetNoteLetter(2, 12));
        }

        [TestMethod()]
        public void getSecondStringFirstFret()
        {
            Assert.AreEqual("A", Chord.GetNoteLetter(1, 0));
        }

        [TestMethod()]
        public void getSecondStringTwelfthFret()
        {
            Assert.AreEqual("A", Chord.GetNoteLetter(1, 12));
        }

        [TestMethod()]
        public void getFirstStringFirstFret()
        {
            Assert.AreEqual("E", Chord.GetNoteLetter(0, 0));
        }

        [TestMethod()]
        public void getFirstStringTwelfthFret()
        {
            Assert.AreEqual("E", Chord.GetNoteLetter(0, 12));
        }

        #endregion
    }
}
