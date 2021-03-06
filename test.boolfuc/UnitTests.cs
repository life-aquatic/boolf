﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace boolfuc
{
    [TestClass]
    public class UnitTests

    {
        Tape tape = new Tape();

        [TestMethod]
        public void TestReadCursor()
        {
            tape.Positive = new BitArray(new int[] { 262733568 });
            tape.Negative = new BitArray(new int[] { 262733568 });
            //00001111 10101000 11111111 00000000

            tape.Cursor = 0;
            Assert.AreEqual(tape.ReadBit(tape.Cursor), false);
            tape.Cursor = 7;
            Assert.AreEqual(tape.ReadBit(tape.Cursor), false);
            tape.Cursor = 8;
            Assert.AreEqual(tape.ReadBit(tape.Cursor), true);
            tape.Cursor = 15;
            Assert.AreEqual(tape.ReadBit(tape.Cursor), true);
            tape.Cursor = 16;
            Assert.AreEqual(tape.ReadBit(tape.Cursor), false);
            tape.Cursor = -1;
            Assert.AreEqual(tape.ReadBit(tape.Cursor), false);
            tape.Cursor = -8;
            Assert.AreEqual(tape.ReadBit(tape.Cursor), false);
            tape.Cursor = -9;
            Assert.AreEqual(tape.ReadBit(tape.Cursor), true);
            tape.Cursor = -16;
            Assert.AreEqual(tape.ReadBit(tape.Cursor), true);
            tape.Cursor = -17;
            Assert.AreEqual(tape.ReadBit(tape.Cursor), false);
        }

        [TestMethod]
        public void TestFlipCursor()
        {
            tape.Positive = new BitArray(new int[] { 262733568 });
            tape.Negative = new BitArray(new int[] { 262733568 });
            //00001111 10101000 11111111 00000000

            tape.Cursor = 0;
            tape.FlipCursor();
            Assert.AreEqual(tape.ReadBit(tape.Cursor), true);
            tape.FlipCursor();
            Assert.AreEqual(tape.ReadBit(tape.Cursor), false);
        }

        [TestMethod]
        public void TestInputBuffer()
        {
            StdInputBuffer inputBuffer = new StdInputBuffer();
            inputBuffer.readBuffer = new BitArray(new byte[] { 97, 98, 99 });
            List<bool> testBits = new List<bool>() { true, false, false, false, false, true, true, false,
             false, true, false, false, false, true, true, false,
             true, true, false, false, false, true, true, false };
            //abc
            //97,98,99
            //("True ", "False", "False ", "False ", "False ", "True ", "True ", "False", 
            // "False", "True ", "False ", "False ", "False ", "True ", "True ", "False", 
            // "True ", "True ", "False ", "False ", "False ", "True ", "True ", "False")
            foreach (bool i in testBits)
            {
                Assert.AreEqual(i, inputBuffer.OneBitFromBuffer());
            }
            var emulatedInput = new System.IO.StringReader("abc");
            Console.SetIn(emulatedInput);
            foreach (bool i in testBits)
            {
                Assert.AreEqual(i, inputBuffer.OneBitFromBuffer());
            }

        }

        [TestMethod]
        public void TestInputBuffer2()
        {
            StringInputBuffer inputBuffer2 = new StringInputBuffer("abc");
            List<bool> testBitsNoPadding = new List<bool>() { true, false, false, false, false, true, true, false,
             false, true, false, false, false, true, true, false,
             true, true, false, false, false, true, true, false };
            for (int i = 0; i < 24; i++)
            {
                Assert.AreEqual(testBitsNoPadding[i], inputBuffer2.OneBitFromBuffer());
            }


            //in the second test I have only 24 bits in my input string, but I am querying it 27 times. last 3 results should be "false"
            inputBuffer2 = new StringInputBuffer("abc");
            List<bool> testBitsWithPadding = new List<bool>() { true, false, false, false, false, true, true, false,
             false, true, false, false, false, true, true, false,
             true, true, false, false, false, true, true, false, false, false, false };
            for (int i = 0; i < 27; i++)
            {
                Assert.AreEqual(testBitsWithPadding[i], inputBuffer2.OneBitFromBuffer());
            }
        }



    }

    [TestClass]
    public class OutputTests
    {
        public System.IO.StringWriter w;
        private string _consoleOutput;
        OutputBuffer outputBuffer;

        List<bool> testBits24 = new List<bool>()
                { true, false, false, false, false, true, true, false,
             false, true, false, false, false, true, true, false,
             true, true, false, false, false, true, true, false };

        List<bool> testBits23 = new List<bool>()
                { true, false, false, false, false, true, true, false,
             false, true, false, false, false, true, true, false,
             true, true, false, false, false, true, false };

        List<bool> testBits0 = new List<bool>()
        { };

        public string TestOutputBuffer(List<bool> outputB)
        {
            outputBuffer = new OutputBuffer();
            w = new System.IO.StringWriter();
            Console.SetOut(w);
            foreach (var i in outputB)
            {
                outputBuffer.Dump(i, true);
            }
            _consoleOutput = w.GetStringBuilder().ToString().Trim();
            return _consoleOutput;
        }
        [TestMethod]
        public void TestOutputs()
        {
            Assert.AreEqual(TestOutputBuffer(testBits24), "abc");
            Assert.AreEqual(TestOutputBuffer(testBits23), "ab");
            Assert.AreEqual(TestOutputBuffer(testBits0), "");
        }


    }

    [TestClass]
    public class TestOutputBuffer2
    {

        List<bool> testBits24 = new List<bool>()
                { true, false, false, false, false, true, true, false,
                  false, true, false, false, false, true, true, false,
                  true, true, false, false, false, true, true };

        List<bool> testBits25 = new List<bool>()
                { true, false, false, false, false, true, true, false,
                  false, true, false, false, false, true, true, false,
                  true, true, false, false, false, true, true, false };

        List<bool> testBits11 = new List<bool>()
                { false, false, false, false, false, false, false, false,
                  true, true };

        List<bool> testBits0 = new List<bool>();

        byte[] expectedRes24 = new byte[] { 97, 98, 99 };
        byte[] expectedRes0 = new byte[0];
        byte[] expectedRes11 = new byte[] { 0, 3 };

        public byte[] TestOutputBufferDump2(List<bool> outputB)
        {
            OutputBuffer outputBuffer2 = new OutputBuffer();
            foreach (bool i in outputB)
                outputBuffer2.Dump(i);
            return outputBuffer2.Export();
        }

        [TestMethod]
        public void TestDump()
        {
            CollectionAssert.AreEqual(expectedRes24, TestOutputBufferDump2(testBits24));
            CollectionAssert.AreEqual(expectedRes24, TestOutputBufferDump2(testBits25));
            CollectionAssert.AreEqual(expectedRes0, TestOutputBufferDump2(testBits0));
            CollectionAssert.AreEqual(expectedRes11, TestOutputBufferDump2(testBits11));

        }
    }


    [TestClass]
    public class TestFlowOfControl
    {
        [TestMethod]
        public void TestJumpCursor()
        {
            Assert.AreEqual(FlowOfControl.JumpCursor("abcd[ed]fhg", 4), 8);
            Assert.AreEqual(FlowOfControl.JumpCursor("01[3456]89[1[[45]]]9012[[56]]9", 2), 8);
            Assert.AreEqual(FlowOfControl.JumpCursor("01[3456]89[1[[45]]]9012[[56]]9", 10), 19);
            Assert.AreEqual(FlowOfControl.JumpCursor("01[3456]89[1[[45]]]9012[[56]]9", 13), 17);
            Assert.AreEqual(FlowOfControl.JumpCursor("01[3456]89[1[[45]]]9012[[56]]9", 23), 29);
            Assert.AreEqual(FlowOfControl.JumpCursor("01[3456]89[1[[45]]]9012[[56]]9", 24), 28);
            Assert.AreEqual(FlowOfControl.JumpCursor("01[3456]89[1[[45]]]9012[[56]]9", 28, true), 22);
            Assert.AreEqual(FlowOfControl.JumpCursor("01[3456]89[1[[45]]]9012[[56]]9", 7, true), 1);
            Assert.AreEqual(FlowOfControl.JumpCursor("01[3456]89[1[[45]]]9012[[56]]9", 18, true), 9);
            Assert.AreEqual(FlowOfControl.JumpCursor("01[3456]89[1[[45]]]9012[[56]]9", 17, true), 11);
            Assert.AreEqual(FlowOfControl.JumpCursor("01[3456]89[1[[45]]]9012[[56]]9", 16, true), 12);
            Assert.AreEqual(FlowOfControl.JumpCursor("01[3456]89[1[[45]]]9012[[56]]9", 28, true), 22);
        }
    }
}

