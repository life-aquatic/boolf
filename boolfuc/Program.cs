using System;
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
        //TODO: if end of program has been reached, do not ask for input, but pad the rest of this with zeros
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
    public class OutputBuffer
    {
        //TODO: send whatever we have (padded with zeros) to stdout when end of program is reached
        public BitArray outBuffer = new BitArray(8);
        int outputCursor = 0;
        public void Dump(bool value)
        {
            if (outputCursor < 8)
            {
                outBuffer[outputCursor] = value;
                outputCursor += 1;
            }
            if (outputCursor == 8)
            {
                byte[] outputByte = new byte[1];
                outBuffer.CopyTo(outputByte, 0);
                Console.Write(Encoding.ASCII.GetString(outputByte));
                outputCursor = 0;
                outBuffer[outputCursor] = value;
            }
        }
    }

    public class OutputBufferSimple
    {
        //TODO: send whatever we have (padded with zeros) to stdout when end of program is reached
        public BitArray outBuffer = new BitArray(8);
        int outputCursor = 0;
        public void Dump(bool value)
        {
            if (outputCursor < 8)
            {
                outBuffer[outputCursor] = value;
                outputCursor += 1;
            }
            if (outputCursor == 8)
            {
                byte[] outputByte = new byte[1];
                outBuffer.CopyTo(outputByte, 0);
                Console.Write(Encoding.ASCII.GetString(outputByte));
                outputCursor = 0;
                outBuffer[outputCursor] = value;
            }
        }
    }




    public static class FlowOfControl
    {
        public static int JumpCursor(string program, int cursor, bool backwards = false)
        {
            (char openingBrac, char closingBrac, int direction) instructions = backwards ? ('[', ']', -1) : (']', '[', 1);
            int moreBracketsToSkip = 0;
            bool foundCounterpart = false;
            while (!foundCounterpart | moreBracketsToSkip > 0)
            {
                if (program[cursor] == instructions.openingBrac)
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
            Queue<bool> outBufferSimple = new Queue<bool>();
            //OutputBuffer outputBuffer = new OutputBuffer();
            InputBuffer inputBuffer = new InputBuffer();
            while (cursor < EOF)
            {
                switch (code[cursor])
                {
                    case '+':
                        tape.FlipCursor();
                        break;
                    case ';':
                        outBufferSimple.Enqueue(tape.ReadBit(tape.Cursor));
                        break;
                    case ',':
                        tape.WriteCursor(inputBuffer.OneBitFromBuffer());
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
            foreach (bool i in Enumerable.Repeat(false, 8 - (outBufferSimple.Count % 8)))
                outBufferSimple.Enqueue(i);
            //byte[] outputByte = new byte[outBufferSimple.Count / 8];
            //outBufferSimple.CopyTo(outputByte, 0);
            //Console.Write(Encoding.ASCII.GetString(outputByte));
            return "ff";
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
             "; +; +; +;+;;;; " +               //space
             "; +; +; ; +; ; ; +; " +           //r
             "; ; +; ; +; +; ; +; " +           //l
             "; ; +; +; ; +; ; +; " +           //d
             "+; +; ; ; ; +; +; ; " +           //!
             "; +; +; +; ";
            //int EOF = hwbrac.Length;
            //int cursor = 0;
            //Tape tape = new Tape();
            //OutputBuffer outputBuffer = new OutputBuffer();
            //InputBuffer inputBuffer = new InputBuffer();
            //while (cursor < EOF)
            //{
            //    //TODO: replace hwbrac with a variable
            //    switch (hwbrac[cursor])
            //    {
            //        case '+':
            //            tape.FlipCursor();
            //            break;
            //        case ';':
            //            outputBuffer.Dump(tape.ReadBit(tape.Cursor));
            //            break;
            //        case ',':
            //            tape.WriteCursor(inputBuffer.OneBitFromBuffer());
            //            break;
            //        case '>':
            //            tape.Cursor += 1;
            //            break;
            //        case '<':
            //            tape.Cursor -= 1;
            //            break;
            //        case '[':
            //            if (!tape.ReadBit(tape.Cursor))
            //                cursor = FlowOfControl.JumpCursor(hwbrac, cursor);
            //            break;
            //        case ']':
            //            cursor = FlowOfControl.JumpCursor(hwbrac, cursor, true) + 1;
            //            break;
            //        default:
            //            break;
            //    }
            //    cursor += 1;
            //}
        }
    }
}
