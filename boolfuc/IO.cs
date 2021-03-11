using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boolfuc
{
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
            }
            catch
            {
                return false;
            }
        }
    }
}
