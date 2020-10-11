using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MADO.CLI
{
    public class Logger
    {
        public static Logger instance = new Logger();
        private const string SEPARATOR = "\t";
        private Logger()
        {
        }
        public void Log(string message,string logType = null,bool showTimestamp = true)
        {
            string timestamp = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
            List<string> strings = new List<string>();
            if (showTimestamp)
            {
                strings.Add(timestamp);
            }
            if (!string.IsNullOrWhiteSpace(logType))
            {
                strings.Add(logType);
            }
            strings.Add(message ?? "");
            string final = string.Join(SEPARATOR, strings);
            Console.WriteLine(final);
        }

        public void LogInfo(string message,bool showTimestamp = true)
        {
            Log(message, "INFO", showTimestamp);
        }
        public void LogError(string message, bool showTimestamp = true)
        {
            Log(message, "ERROR", showTimestamp);
        }
    }
}
