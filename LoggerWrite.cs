using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractEntity_TEXT
{
    internal class LoggerWrite
    {
        public static string LogPath { get; set; }
        public static void Logger(string message)
        {
            message = $"{DateTime.Now}: \t {message}";
            using (StreamWriter writer = new StreamWriter(LogPath, true))
            {
                writer.WriteLine(message);
            }
        }
    }
}
