using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boolfuc
{
    class Logger
    {
        StreamWriter writer = new StreamWriter("log.txt");
        public void Log(string message)
        {
            writer.WriteLine(message);
        }
    }
}
