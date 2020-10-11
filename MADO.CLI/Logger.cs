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
        private Logger()
        {
        }
        
        public void LogMessage(string message)
        {
            string timestamp = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
            Console.WriteLine($"[{timestamp}]   INFO    {message}");
        }
        public void LogError(string errorMessage)
        {
            string timestamp = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
            Console.WriteLine($"[{timestamp}]   ERROR   {errorMessage}");
        }
    }
}
