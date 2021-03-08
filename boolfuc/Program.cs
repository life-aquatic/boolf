﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Numerics;


namespace boolfuc
{
    public class Tape
    {
        public int Cursor = 0;
        //TODO - remove 999 bit limit
        public BitArray Positive = new BitArray(999);
        public BitArray Negative = new BitArray(999);
        public (BitArray stor, int ind) UnsigifyStorage(int cursor)
        {
            return cursor < 0 ? (Negative, cursor * -1 - 1) : (Positive, cursor);
        }
        public bool ReadBit(int index)
        {
            var storageInd = UnsigifyStorage(index);
            return storageInd.stor[storageInd.ind];
        }


        public void FlipCursor()
        {
            var storageInd = UnsigifyStorage(Cursor);
            storageInd.stor[storageInd.ind] = !storageInd.stor[storageInd.ind];
        }
        public void WriteCursor(bool value)
        {
            var storageInd = UnsigifyStorage(Cursor);
            storageInd.stor[storageInd.ind] = value;
        }
    }
    public class InputBuffer
    {
        //TODO: if end of tape has been reached, do not ask for input, but pad the rest of this with zeros
        public BitArray readBuffer = new BitArray(0);
        int inputCursor = -1;

        public bool OneBitFromBuffer()
        {
            while (inputCursor >= readBuffer.Length - 1)
            {
                string input = Console.ReadLine();
                readBuffer = new BitArray(Encoding.ASCII.GetBytes(input.ToCharArray()));
                inputCursor = -1;
            }
            inputCursor += 1;
            return readBuffer[inputCursor];
        }
    }
    public class InputBuffer2
    {
        public Queue<bool> allInput = new Queue<bool>();
        public InputBuffer2(string input)
        {
            foreach (bool i in new BitArray(Encoding.ASCII.GetBytes(input.ToCharArray())))
                allInput.Enqueue(i);
        }

        public bool OneBitFromBuffer()
        {
            try
            {
                return allInput.Dequeue();
            } catch
            {
                return false;
            }
        }
    }







    public static class FlowOfControl
    {
        //now if a bracket is not correctly termintated, we fly out of the program. maybe I need a nice exception for that.
        public static int JumpCursor(string program, int cursor, bool backwards = false)
        {
            (char openingBrac, char closingBrac, int direction) instructions = backwards ? ('[', ']', -1) : (']', '[', 1);
            int moreBracketsToSkip = 0;
            bool foundCounterpart = false;
            while (!foundCounterpart | moreBracketsToSkip > 0)
            {
                if (program[cursor] ==instructions.openingBrac)
                {
                    foundCounterpart = true;
                    if (moreBracketsToSkip > 0)
                        moreBracketsToSkip -= 1;
                }
                else if (program[cursor] == instructions.closingBrac)
                {
                    moreBracketsToSkip += 1;
                }
                cursor += instructions.direction;
            }
            return cursor;
        }
    }
    public class Boolfuck
    {

        public static string interpret(string code, string input)
        {
            int EOF = code.Length;
            int cursor = 0;
            Tape tape = new Tape();
            
            OutputBuffer2 outputBuffer2 = new OutputBuffer2();
            InputBuffer2 inputBuffer2 = new InputBuffer2(input);
            
            while (cursor < EOF)
            {
                switch (code[cursor])
                {
                    case '+':
                        tape.FlipCursor();
                        break;
                    case ';':
                        outputBuffer2.Dump(tape.ReadBit(tape.Cursor));
                        break;
                    case ',':
                        tape.WriteCursor(inputBuffer2.OneBitFromBuffer());
                        break;
                    case '>':
                        tape.Cursor += 1;
                        break;
                    case '<':
                        tape.Cursor -= 1;
                        break;
                    case '[':
                        if (!tape.ReadBit(tape.Cursor))
                            cursor = FlowOfControl.JumpCursor(code, cursor);
                        break;
                    case ']':
                        cursor = FlowOfControl.JumpCursor(code, cursor, true) + 1;
                        break;
                    default:
                        break;
                }
                cursor += 1;
            }
            
            return Encoding.ASCII.GetString(outputBuffer2.Export());
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //string bractest = "01[3456]89[1[[45]]]9012[[56]]9";
            //string hw = ";;;+;+;;+;+;+;+;+;+;;+;;+;;;+;;+;+;;+;;;+;;+;+;;+;+;;;;+;+;;+;;;+;;+;+;+;;;;;;;+;+;;+;;;+;+;;;+;+;;;;+;+;;+;;+;+;;+;;;+;;;+;;+;+;;+;;;+;+;;+;;+;+;+;;;;+;+;;;+;+;+;";
            //string prog = ",>,>,>,>,>,>,>,<<<<<<<;>;>;>;>;>;>;>;";
            string hwbrac = ";;;+;+;;+;+;" +    //H
            "+; +; +; +; ; +; ; +; " +          //e
             "; ; +; ; +; +; ; +; " +           //l
             "; +; +; +;+;;;; " +                //newline
             "; ; +; ; +; +; ; +; " +           //l
             "+; ; ; ; +; +; ; +; " +           //o
             "; +; +; +;+;;;; " +               //newline
             "; ; +; ; +; +; +; ; " +           //,
             "; ; ; ; ; +; +; ; " +             //space
             "+; ; ; +; +; ; ; +; " +           //w
             "+; ; ; ; +; +; ; +; " +           //o
             "; +; +; +;+fffff;00;;; " +               //space
             "; +; +; ; +; ; ; +; " +           //r
             "; ; +; ; +; +; ; +; " +           //l
             "; ; +; +; ;jj +; ; +; " +           //d
             "+; +; ; ; ; +; +; ; " +           //!
             "; +; +; +; ";
            Console.WriteLine(Boolfuck.interpret(hwbrac, ""));
            
        }
    }
}
