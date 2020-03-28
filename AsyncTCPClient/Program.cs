using Common;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncTCPClient {
    class Program {
        static void Test(string[] args)
        {
            Queue<KeyValuePair<Socket, DataPack<NormalProtocol>>> queue = new Queue<KeyValuePair<Socket, DataPack<NormalProtocol>>>();
            
            Client<NormalProtocol>.instance.Start(queue);

            while (true)
            {
                if (queue.Count>0)
                {
                    HandlerCenter.instance.Udpate(queue.Dequeue());

                }
                //string str = Console.ReadLine();
                //if (str.Equals("quit"))
                //{
                //    break;
                //}
            }
            Client<NormalProtocol>.instance.Close();
        }
    }
}
