using System;
using System.Collections.Generic;
using System.Text;
using LitJson;
using Common.Message;
using System.IO;

namespace Common.Protocol {

    public class DataPack<T> where T : Protocol, new() {
        public static T protocol = new T();

        public MsgType type;
        public byte[] data;

        public DataPack(byte[] data)
        {
            //if (protocol != null)
            //{
            //    protocol = new T();
            //}
            this.data = data;
            this.type = (MsgType)BitConverter.ToInt32(this.data, 0);
        }

    }

    public abstract class Protocol {

        public Protocol()
        {
        }

        /// <summary>
        /// 序列化数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract byte[] Serialize<T>(MsgType type, T t) where T : IDataPack;

        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public abstract T Deserialze<T>(byte[] data) where T : IDataPack;
    }

    /// <summary>
    /// 协议类型   length+type+data
    /// </summary>
    public class NormalProtocol : Protocol {

        //public MsgType type;
        //public MsgType getType(byte[] )
        //{
        //}
        public override T Deserialze<T>(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data, 4, data.Length - 4);
            try
            {
                T t = JsonMapper.ToObject<T>(json);
                Log.log(t.ToString());
                return t;
            }
            catch (Exception)
            {
                Common.Log.logError("反序列化数据失败");
                return default(T);
            }
        }

        public override byte[] Serialize<T>(MsgType type, T t)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                var data = Encoding.UTF8.GetBytes(JsonMapper.ToJson(t));
                memoryStream.Write(BitConverter.GetBytes(data.Length + 8), 0, 4);
                memoryStream.Write(BitConverter.GetBytes((int)type), 0, 4);
                memoryStream.Write(data, 0, data.Length);
                return memoryStream.ToArray();
            }
        }
    }
}