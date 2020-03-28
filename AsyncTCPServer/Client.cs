using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Common;
using Common.Protocol;

namespace AsyncTCPServer {

    public class Client {
        private Server server;
        private Socket clientSocket;
        private BinaryReader mem;
        private byte[] buffer;
        private OpMessage message = new OpMessage(Server.instance.msgQueue);

        public Client()
        {
        }

        public Client(Socket clientSocket, Server server)
        {
            mem = new BinaryReader(new MemoryStream());
            buffer = new byte[1024];
            this.clientSocket = clientSocket;
            this.server = server;
            Log.log("-客户端   {0}   上线", clientSocket.RemoteEndPoint);
        }

        public void Start()
        {
            clientSocket.BeginReceive(message.Buffer, 0, message.Buffer.Length, SocketFlags.None, RecevieCallBack, null);
            //clientSocket.BeginReceive(buffer, 0, buffer.Length,SocketFlags.None, RecevieCallBack, null);
        }

        private void RecevieCallBack(IAsyncResult ar)
        {
            int count = -1;
            try
            {
                count = clientSocket.EndReceive(ar);
                if (count == 0)
                {
                    Log.log(clientSocket.RemoteEndPoint + "下线了！！");
                    Close();
                    return;
                }
            }
            catch (Exception)
            {
                Log.logWarning(clientSocket.RemoteEndPoint + "异常掉线");
                Close();
                return;
            }
            Log.log("接收到的数据长度：{0}", count);

            message.UnPack(clientSocket, count);
            Start();
        }

        private int StreamCurrentLength(Stream mem)
        {
            return (int)(mem.Length - mem.Position);
        }

        private void Close()
        {
            mem.Close();
            message.Close();
            if (clientSocket != null)
            {
                clientSocket.Close();
            }
            server.RemoveClient(this);
        }
    }
}