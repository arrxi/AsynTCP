using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
/// <summary>
///   4      4    ?
/// length type data
/// </summary>
namespace SocektTCPServer {

    public class Exception1 : SocketException {
    }

    public class Server : Singleton<Server> {
        private Socket _server;
        private IPAddress addr;
        private byte[] buffer = new byte[1024 * 8];
        private int port;
        private Thread acceptListener;
        private Thread recvListener;
        private List<Socket> all_clients = new List<Socket>();

        private List<Socket> mobile_clients = new List<Socket>();
        private List<Socket> pc_clients = new List<Socket>();
        #region 解包的缓冲区
        MemoryStream m = new MemoryStream();
        BinaryReader mem;

        #endregion

        public Socket server { get { return _server; } set { _server = value; } }
        public IPAddress Addr { get { return addr; } set { addr = value; } }
        public byte[] Buffer { get { return buffer; } set { buffer = value; } }
        public int Port { get { return port; } set { port = value; } }

        public Server()
        {
            mem = new BinaryReader(m);
            Port = 9000;
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Addr = GetLocalIPAddressinUse();
            Console.WriteLine("本机地址:" + Addr);
            server.Bind(new IPEndPoint(Addr, Port));
            server.Listen(5);
        }

        public void Start()
        {
            Console.WriteLine("accept线程启动中。。。。。。。");
            acceptListener = new Thread(_accept);
            acceptListener.Start();
            Console.WriteLine("recv线程启动中。。。。。。。");
            recvListener = new Thread(_recv);
            recvListener.Start();
        }

        private void _accept()
        {
            while (true)
            {
                Socket client = server.Accept();
                all_clients.Add(client);
                Console.WriteLine(client.RemoteEndPoint + "链接成功");
            }
        }

        private void _recv()
        {

            List<Socket> rms = new List<Socket>();

            while (true)
            {
                foreach (var item in all_clients)
                {
                    item.Blocking = false;
                    try
                    {
                        int length = item.Receive(buffer);
                        //Console.WriteLine("接受道信息,长度为："+length);
                        opStickPackage(length);
                    }
                    catch (SocketException se)
                    {
                        ///10035 非阻塞式
                        ///10054 socket意外关闭
                        switch (se.ErrorCode)
                        {
                            case 10054:
                                Console.WriteLine(item.RemoteEndPoint + "非正常掉线");
                                item.Close();
                                rms.Add(item);
                                continue;
                            case 10035:
                                Console.WriteLine("非阻塞式套接字");
                                continue;
                            default:
                                continue;
                        }
                    }
                }
                foreach (var item in rms)
                {
                    all_clients.Remove(item);
                }
                rms.Clear();
                Thread.Sleep(200);
            }
        }
        /// <summary>
        /// 处理粘包
        /// </summary>
        /// <param name="length"></param>
        private void opStickPackage(int length)
        {
            //MemoryStream memory = new MemoryStream(buffer, 0, length);

            //memory.Close();
            mem.BaseStream.Write(buffer, 0, length);
            mem.BaseStream.Position = 0;
            while (StreamCurrentLength(mem)>4)
            {
                int contentLen = mem.ReadInt32();
                if (StreamCurrentLength(mem)>=contentLen-4)
                {
                   var content = mem.ReadBytes(contentLen-4);
                   Console.WriteLine(Encoding.UTF8.GetString(content));
                }
            }
            byte[] remain = mem.ReadBytes(StreamCurrentLength(mem));
            mem.BaseStream.SetLength(0);
            mem.BaseStream.Write(remain, 0, remain.Length);
            mem.BaseStream.Seek(0, SeekOrigin.Begin);
            mem.BaseStream.Position = 0;

        }
        //private void opStickPackage(BinaryReader mem, int len,object obj = null)
        //{    //分包

        //    mem.BaseStream.Seek(0, SeekOrigin.End);
        //    mem.BaseStream.Write(buffer, 0, len);
        //    mem.BaseStream.Seek(0, SeekOrigin.Begin);
        //    while (StreamCurrentLength(mem) > 4)
        //    {
        //        int contentLen = mem.ReadInt32();

        //        if (StreamCurrentLength(mem) >= contentLen)
        //        {
        //            //内容：type+data
        //            byte[] content = mem.ReadBytes(contentLen);   //
        //            Console.WriteLine(Encoding.UTF8.GetString(content,0,content.Length));
        //            //Console.WriteLine("\n" + BytesToString(content));
        //            //MsgPackage m = ProtoBufTools.ToObject<MsgPackage>(content);
        //            //Console.WriteLine(m);
        //        }
        //        else
        //        {
        //            mem.BaseStream.Seek(-4, SeekOrigin.Current);
        //            break;
        //        }
        //    }
        //    byte[] remain = mem.ReadBytes(StreamCurrentLength(mem));
        //    mem.BaseStream.SetLength(0);
        //    mem.BaseStream.Write(remain, 0, remain.Length);

        //}

        private int StreamCurrentLength(BinaryReader mem)
        {
            return (int)(mem.BaseStream.Length - mem.BaseStream.Position);
        }


        public void SendStr(Socket client ,string t)
        {
            using (MemoryStream ms = new MemoryStream())
            {
               var data = Encoding.UTF8.GetBytes(t);
                ms.Write(BitConverter.GetBytes(data.Length + 4), 0, 4);
                //ms.Write(BitConverter.GetBytes(1000), 0, 4);
                ms.Write(data, 0, data.Length);
                client.Send(ms.ToArray());

            }
        }
        public void Close()
        {
            acceptListener.Abort();
            recvListener.Abort();
            server.Close();
            foreach (var item in all_clients)
            {
                item.Close();
            }
        }

        /// <summary>
        /// 获取本机所有ip地址
        /// </summary>
        /// <param name="netType">"InterNetwork":ipv4地址，"InterNetworkV6":ipv6地址</param>
        /// <returns>ip地址集合</returns>
        public static List<IPAddress> GetLocalIpAddress(string netType)
        {
            string hostName = Dns.GetHostName();                    //获取主机名称
            IPAddress[] addresses = Dns.GetHostAddresses(hostName); //解析主机IP地址

            List<IPAddress> IPList = new List<IPAddress>();
            if (netType == string.Empty)
            {
                for (int i = 0; i < addresses.Length; i++)
                {
                    IPList.Add(addresses[i]);
                }
            }
            else
            {
                //AddressFamily.InterNetwork表示此IP为IPv4,
                //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                for (int i = 0; i < addresses.Length; i++)
                {
                    if (addresses[i].AddressFamily.ToString() == netType)
                    {
                        IPList.Add(addresses[i]);
                    }
                }
            }
            return IPList;
        }

        public static IPAddress GetLocalIPAddressinUse()
        {
            var tmp = GetLocalIpAddress("InterNetwork");
            return tmp[tmp.Count - 1];
        }
    }
}