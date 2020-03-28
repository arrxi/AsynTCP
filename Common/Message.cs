using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common.Protocol;

namespace Common {

    public class OpMessage{
        private byte[] buffer;
        private MemoryStream memoryStream;
        private BinaryReader reader;
        Queue<KeyValuePair<Socket,DataPack<NormalProtocol>>> queue;
        public OpMessage(Queue<KeyValuePair<Socket, DataPack<NormalProtocol>>> queue)
        {
            buffer = new byte[1024];
            memoryStream = new MemoryStream();
            reader = new BinaryReader(memoryStream);
            this.queue = queue;
        }

        public byte[] Buffer
        {
            get
            {
                return buffer;
            }

            //set
            //{
            //    buffer = value;
            //}
        }


        ///
        /// <summary>
        /// 处理粘包
        /// PS：有bug未处理
        /// 读取消息长度后当接收到的消息不完整时，未重置内存流中的数据
        /// </summary>
        /// <param name="_client">发来消息的客户端</param>
        /// <param name="count">接收到的数据的字节数</param>
        public void UnPack(Socket _client, int count)
        {
            reader.BaseStream.Write(buffer, 0, count);
            reader.BaseStream.Position = 0;
            while (StreamCurrentLength(reader.BaseStream) > 4)
            {
                int contentLen = reader.ReadInt32();
                if (StreamCurrentLength(reader.BaseStream) >= contentLen - 4)
                {
                    //content为一个消息 不好含长度数据的

                    var content = reader.ReadBytes(contentLen - 4);
                    var data = new KeyValuePair<Socket, DataPack<NormalProtocol>>(_client, new DataPack<NormalProtocol>(content));
                    queue.Enqueue(data);
                    //Console.Write(_client.RemoteEndPoint + ">>  ");
                    //var result = protocol.Deserialze<Req_Test>(content);
                    //if (result != null)
                    //{
                    //    Common.Log.log(result.ToString());
                    //}
                }
            }
            var remain = reader.ReadBytes(StreamCurrentLength(reader.BaseStream));
            reader.BaseStream.SetLength(0);
            reader.BaseStream.Write(remain, 0, remain.Length);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            reader.BaseStream.Position = 0;
        }

        private int StreamCurrentLength(Stream mem)
        {
            return (int)(mem.Length - mem.Position);
        }

        public void Close()
        {
            reader.Close();
            memoryStream.Close();
        }
    }
}