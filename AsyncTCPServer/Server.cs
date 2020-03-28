using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Protocol;

namespace AsyncTCPServer {

    public class Server {
        private static Server _instance;
        private Socket _serverSocket;
        private IPEndPoint ip;
        private List<Client> clientList = new List<Client>();
        #region 消息处理线程
        public Queue<KeyValuePair<Socket, DataPack<NormalProtocol>>> msgQueue = new Queue<KeyValuePair<Socket, DataPack<NormalProtocol>>>();
        private Thread operatorMsg;
        #endregion


        public static Server instance
        {
            get
            {
                if (_instance == null) _instance = new Server();
                return _instance;
            }
        }


        private Server()
        {
        }

        public void Start(string ip="127.0.0.1",int port=9999)
        {
            Common.HandlerCenter.instance.InitHandler();
            var ipe = new IPEndPoint(IPAddress.Parse(ip),port); ;// new IPEndPoint(GetLocalIPAddressinUse(), 9999);
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _serverSocket.Bind(ipe);
            _serverSocket.Listen(10);
            operatorMsg = new Thread(OpMsgThread);
            operatorMsg.Start();
            _serverSocket.BeginAccept(AcceptCallBack, null);
            Common.Log.log("服务器已启动,服务器ip为：" + ip);
        }
        /// <summary>
        /// 处理消息的线程
        /// </summary>
        private void OpMsgThread()
        {
            while (true)
            {
                if (msgQueue.Count>0)
                {
                    Common.HandlerCenter.instance.Udpate(msgQueue.Dequeue());
                }
            }
        }

        private void AcceptCallBack(IAsyncResult ar)
        {
            Socket clientSocket = _serverSocket.EndAccept(ar);
            Client client = new Client(clientSocket, this);
            clientList.Add(client);
            client.Start();
            _serverSocket.BeginAccept(AcceptCallBack, null);
        }

        public void RemoveClient(Client client)
        {
            lock (clientList)
            {
                if (client != null)
                {
                    clientList.Remove(client);
                }
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