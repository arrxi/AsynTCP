﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncTCPServer {

    internal class Program {

        private static void Main(string[] args)
        {
            //Console.ForegroundColor = ConsoleColor.Blue;
            //Console.WriteLine("console");
            //Console.ForegroundColor = ConsoleColor.White;
            Server.instance.Start(InitHandler.InitMsgHandlerCallBack);
            Console.ReadLine();
        }
    }
}