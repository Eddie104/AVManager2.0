using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libra.log4CSharp
{

    enum LogLevel
    {
        DEBUG, INFO, WARN, ERROR, FATAL
    }

    static class Logger
    {
        public static LogLevel logLevel = LogLevel.DEBUG;

        public static void Debug(string log)
        {
            if (logLevel <= LogLevel.DEBUG)
            {
                Trace("[DEBUG] => " + log);
            }
        }

        public static void Info(string log)
        {
            if (logLevel <= LogLevel.INFO)
            {
                Trace("[INFO] => " + log);
            }
        }

        public static void Wran(string log)
        {
            if (logLevel <= LogLevel.WARN)
            {
                Trace("[WARN] => " + log);
            }
        }

        public static void Error(string log)
        {
            if (logLevel <= LogLevel.ERROR)
            {
                Trace("[ERROR] => " + log);
            }
        }

        public static void Fatal(string log)
        {
            if (logLevel <= LogLevel.FATAL)
            {
                Trace("[FATAL] => " + log);
            }
        }

        private static void Trace(string log)
        {
            Console.WriteLine(log);
        }
    }
}
