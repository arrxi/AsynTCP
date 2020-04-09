using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Common.Protocol;
using Common;

namespace AsyncTCPClient {

    public class Client<T> where T : Common.Protocol.Protocol, new() {
        private static Client<T> _instance;

        public static Client<T> instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Client<T>();
                }
                return _instance;
            }
        }

        private Common.OpMessage opMessage;// = new Common.OpMessage();
        private T protocol;

        public bool IsConnection
        {
            get
            {
                return isConnection;
            }
        }

        private Socket _clientSocket;
        private bool isConnection;

        private Client()
        {
            protocol = new T();
        }

        public void Start(Queue<KeyValuePair<Socket, DataPack<NormalProtocol>>> queue, string addr = "127.0.0.1", int port = 9999, Action registerAction = null)
        {
            HandlerCenter.instance.InitHandler(registerAction);
            opMessage = new Common.OpMessage(queue);
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(addr), port);
            _clientSocket.BeginConnect(ip, ConnectionCallBack/*(ar)=> { _clientSocket.EndConnect(ar); }*/, null);
        }

        private void ConnectionCallBack(IAsyncResult ar)
        {
            try
            {
                _clientSocket.EndConnect(ar);
                Common.Log.log("连接成功！！！");
                isConnection = true;

                #region 发送测试消息

                var data = protocol.Serialize(Common.Message.MsgType.Req_Test, new Common.Message.Req_Test("heheheh", "arrxi"));

                Common.Log.log("发送数据！");
                _clientSocket.Send(data);

                #endregion 发送测试消息

                Recevice();
            }
            catch (Exception e)
            {
                isConnection = false;
                Close();

                throw e;
            }
        }

        public void SendTest()
        {
        }

        private void Recevice()
        {
            _clientSocket.BeginReceive(opMessage.Buffer, 0, opMessage.Buffer.Length, SocketFlags.None, RecvCallBack, null);
        }

        private void RecvCallBack(IAsyncResult ar)
        {
            int count = -1;
            try
            {
                count = _clientSocket.EndReceive(ar);
                opMessage.UnPack(_clientSocket, count);

                Recevice();
            }
            catch (Exception e)
            {
                Common.Log.logError(e.Message);
            }
        }

        public void Close()
        {
            _clientSocket.Close();
        }
    }
}