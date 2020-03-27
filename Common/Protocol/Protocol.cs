using System;
using System.Collections.Generic;
using System.Text;
using LitJson;
using Common.Message;
using System.IO;

namespace Common.Protocol {
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
        public abstract byte[] Serialize<T>(T t) where T :IMessage;
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public abstract T Deserialze<T>(byte[] data)where T:IMessage;
    }

    public class NormalProtocol : Protocol {
        public override T Deserialze<T>(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
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

        public override byte[] Serialize<T>(T t)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                var data = Encoding.UTF8.GetBytes(JsonMapper.ToJson(t));
                memoryStream.Write(BitConverter.GetBytes(data.Length + 4), 0, 4);
                memoryStream.Write(data, 0, data.Length);
                return memoryStream.ToArray();
            }
        }
    }
}
