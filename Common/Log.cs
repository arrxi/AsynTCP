using System;
using System.Collections.Generic;
using System.Text;

namespace Common {

    public static class Log {
        public static void log(string format,params object[] args)
        {
            Console.WriteLine(format,args);
        }
        public static void logWarning(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(format,args);
            Console.ForegroundColor = ConsoleColor.White;

        }
        public static void logError(string format,params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(format,args);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
