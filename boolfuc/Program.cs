using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace boolfuc
{
    public class Tape
    {
        public int Cursor = 0;
        public BitArray Positive = new BitArray(9999);
        public BitArray Negative = new BitArray(9999);
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

            if (storageInd.stor.Length <= storageInd.ind)
            {
                storageInd.stor.Length = storageInd.ind + 1;
                storageInd.stor[storageInd.ind] = value;
            }
            else
            {
                storageInd.stor[storageInd.ind] = value;
            }
        }
        public override string ToString()
        {
            return base.ToString();
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
        
        public static string interpret(string code, string input, bool stdIO = false)
        {
            //remove logger
            Logger logger = new Logger();
            int EOF = code.Length;
            int cursor = 0;
            Tape tape = new Tape();
            OutputBuffer outputBuffer = new OutputBuffer();
            IInputBuffer inputBuffer;
            if (stdIO)
                inputBuffer = new StdInputBuffer();
            else
                inputBuffer = new StringInputBuffer(input);


            while (cursor < EOF)
            {
                switch (code[cursor])
                {
                    case '+':
                        tape.FlipCursor();
                        break;
                    case ';':
                        outputBuffer.Dump(tape.ReadBit(tape.Cursor), stdIO);
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
                        if (tape.ReadBit(tape.Cursor) == false)
                            cursor = FlowOfControl.JumpCursor(code, cursor);
                        break;
                    case ']':
                        if (tape.ReadBit(tape.Cursor) == true)
                            cursor = FlowOfControl.JumpCursor(code, cursor, true) + 1;
                        break;
                    default:
                        break;
                }
                //remove this consolewriteline
                //Console.WriteLine(tape.ReadBit(tape.Cursor));
                cursor += 1;
            }
            return Encoding.ASCII.GetString(outputBuffer.Export());
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var program = new StringBuilder();
            try
            {
                program.Append(File.ReadAllText(args[0]));
            }
            catch
            {
                Console.WriteLine("Unable to get program from first argument, exiting");
                Environment.Exit(13);
            }
            Console.Write(Boolfuck.interpret(program.ToString(), "", true));
        }
    }
}
