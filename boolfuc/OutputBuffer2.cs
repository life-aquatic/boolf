using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boolfuc
{
    class OutputBuffer2
    {
        byte currentChar = 0b0000_0000;
        int bitsWritten = 0;
        List<byte> outBuffer = new List<byte>();
        public void Dump(bool value)
        {
            if (bitsWritten == 8)
            {
                outBuffer.Add(currentChar);
                bitsWritten = 0;
                currentChar = 0;
            }
            currentChar <<= 1;
            currentChar += value ? 1 : 0;
            bitsWritten += 1;
            
        }
        public override string ToString()
        {
            var ou = new List<string>();
            foreach (byte i in outBuffer)
            {
                string converted = Convert.ToString(i, toBase: 2);
                ou.Add(converted.PadLeft(8, '_'));
            }
            return String.Join(", ", ou);
        }
    }
}
