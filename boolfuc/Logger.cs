using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boolfuc
{
    class SuspectLogger
    {
        StreamWriter writer = new StreamWriter(@"c:\temp\log.txt");
        public void Log(string message)
        {
            writer.WriteLine(message);
        }
    }


    public class Logger
    {
        public void Log(string message)
        {
            using (StreamWriter logFile = new StreamWriter(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "logfile.txt")))
            {
                logFile.WriteLine(message);
            }
        }
    }
}
