using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 
/// 
/// 
/// 
/// 
/// 
/// 未完成 搁浅了
/// </summary>
namespace SocektTCPServer {
    using System.Net;
    using System.Net.Sockets;
    using LitJson;
    internal class Program {

        private static void Main(string[] args)
        {
            TextIP();
            //Server.instance.Start();
            //Console.ReadLine();
            //Server.instance.Close();
        }
        public static void TextIP()
        {
            IPAddress ip = IPAddress.Parse("112.80.248.73");
            IPEndPoint ipe = new IPEndPoint(ip, 4562);
            Console.WriteLine(ipe);
            Console.WriteLine(ipToLong("112.80.248.73"));
        }
        //public static long ipToLong(string ipAddress)
        //{
        //    //将目标IP地址字符串strIPAddress转换为数字
        //    string[] arrayIP = ipAddress.Split('.');
        //    long sip1 = long.Parse(arrayIP[0]);
        //    long sip2 = long.Parse(arrayIP[1]);
        //    long sip3 = long.Parse(arrayIP[2]);
        //    long sip4 = long.Parse(arrayIP[3]);

        //    long r1 = sip1 * 256 * 256 * 256;
        //    long r2 = sip2 * 256 * 256;
        //    long r3 = sip3 * 256;
        //    long r4 = sip4;

        //    long result = r1 + r2 + r3 + r4;
        //    return result;
        //}

        public static long ipToLong(string ip)
        {
            long result = 0;
            var nums = ip.Split('.');
            for (int i = 0; i < nums.Length; i++)
            {
                result = long.Parse(nums[i]) | result << 8;
            }
            Console.WriteLine(result);
         return   result;
        }
    }
}
