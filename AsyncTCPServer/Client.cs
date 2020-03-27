
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
namespace AsyncTCPServer
{
    public class Client
    {
        Server server;
        Socket clientSocket;
        BinaryReader mem;
        byte[] buffer;
        Message<NormalProtocol> message = new Message<NormalProtocol>();
        public Client()
        {
        }

        public Client(Socket clientSocket,Server server)
        {
            mem = new BinaryReader(new MemoryStream());
            buffer = new byte[1024];
            this.clientSocket = clientSocket;
            this.server = server;
            Log.log("-客户端   {0}   上线",clientSocket.RemoteEndPoint);
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
                if (count==0)
                {
                    Log.log(clientSocket.RemoteEndPoint+"下线了！！");
                    Close();
                    return;
                }

            }
            catch (Exception)
            {
                Log.logWarning(clientSocket.RemoteEndPoint+"异常掉线");
                Close();
                return;
            }
            Log.log("接收到的数据长度：{0}", count);
            //mem.BaseStream.Write(buffer, 0, count);
            //mem.BaseStream.Position = 0;
            //while (StreamCurrentLength(mem.BaseStream) > 4)
            //{
            //    int contentLen = mem.ReadInt32();
            //    if (StreamCurrentLength(mem.BaseStream) >= contentLen - 4)
            //    {
            //        var content = mem.ReadBytes(contentLen - 4);

            //        Console.Write(clientSocket.RemoteEndPoint + ">>  ");
            //        Log.log(Encoding.UTF8.GetString(content));

            //    }
            //}
            //var remain = mem.ReadBytes(StreamCurrentLength(mem.BaseStream));
            //mem.BaseStream.SetLength(0);
            //mem.BaseStream.Write(remain, 0, remain.Length);
            //mem.BaseStream.Seek(0, SeekOrigin.Begin);
            //mem.BaseStream.Position = 0;
            message.UnPack(clientSocket, count);
            Start();


        }
        private int StreamCurrentLength(Stream mem)
        {
            return (int)(mem.Length - mem.Position);
        }
        void Close()
        {
            mem.Close();
            message.Close();
            if (clientSocket!=null)
            {
                clientSocket.Close();
            }
            server.RemoveClient(this);
        }

    }
}
