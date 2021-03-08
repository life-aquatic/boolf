using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boolfuc
{
    public class OutputBuffer2
    {
        byte currentChar = 0b0000_0000;
        byte position = 0b0000_0001;
        int bitsWritten = 0;
        public List<byte> outBuffer = new List<byte>();
    

        public void Dump (bool value)
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
            if (outBuffer.Count == 0)
                Dump(false);
            for (int i = 0; i < bitsWritten % 8; i++)
                Dump(false);
            //the line below is only for debugging, I will need to get rid of it
            //System.IO.File.WriteAllBytes("du.dat", outBuffer.ToArray());
            return outBuffer.ToArray();
        }

        //this ToString override is here only for debugging, I'll get rid of it.
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
