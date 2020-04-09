using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Common {

    public static class Log {

        public static void log(string format, params object[] args)
        {
            Debug.Log(string.Format(format, args));

            Console.WriteLine(format, args);
        }

        public static void logWarning(string format, params object[] args)
        {
            Debug.LogWarning(string.Format(format, args));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(format, args);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void logError(string format, params object[] args)
        {
            Debug.LogError(string.Format(format, args));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(format, args);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void log(object obj)
        {
            Debug.Log(obj);
            Console.WriteLine(obj);
        }

        public static void logWarning(object obj)
        {
            Debug.LogWarning(obj);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(obj);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void logError(object obj)
        {
            Debug.LogError(obj);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(obj);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}