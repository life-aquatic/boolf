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

    public class OutputBuffer2
    {
        byte currentChar = 0b0000_0000;
        byte position = 0b0000_0001;
        int bitsWritten = 0;
        public List<byte> outBuffer = new List<byte>();

        public void Dump(bool value)
        {
            if (value)
                currentChar |= position;
            position <<= 1;
            bitsWritten += 1;
            if (bitsWritten % 8 == 0)
            {
                outBuffer.Add(currentChar);
                currentChar = 0;
                position = 0b0000_0001;
            }
        }

        public byte[] Export()
        {
            //if (outBuffer.Count == 0)
            //    Dump(false);
            for (int i = 0; i < bitsWritten % 8; i++)
                Dump(false);
            return outBuffer.ToArray();
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
            Console.Write(Boolfuck.interpret(program.ToString(), ""));
        }
    }
}
