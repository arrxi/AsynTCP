using Common.Message;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common {

    public interface IMessageHandler {

        void Register();
    }

    public class HandlerCenter : Singleton<HandlerCenter> {
        private bool initOK = false;
        private Dictionary<MsgType, Action<Socket, DataPack<NormalProtocol>>> handlers = new Dictionary<MsgType, Action<Socket, DataPack<NormalProtocol>>>();

        public void InitHandler(Action handlerCall)
        {
            initOK = true;
            if (handlerCall != null)
            {
                handlerCall.Invoke();
            }
            MsgHandler.TestMsgHandler.instance.Register();
        }

        public void Register(MsgType type, Action<Socket, DataPack<NormalProtocol>> func)
        {
            if (!initOK)
            {
                Common.Log.logError("HandlerCenter 消息处理中心为初始化 或者 初始化失败！！\t注册失败");
                return;
            }
            if (handlers.ContainsKey(type))
            {
                Common.Log.logWarning("{0}类型的消息回调已经注册", type.ToString());
            }
            else
            {
                handlers.Add(type, func);
            }
        }

        public void UnRegister(MsgType type)
        {
            if (handlers.ContainsKey(type))
            {
                handlers.Remove(type);
            }
            else
            {
                Log.logWarning("{0}类型的消息未注册！！", type.ToString());
            }
        }

        public void Udpate(KeyValuePair<Socket, DataPack<NormalProtocol>> data)
        {
            if (!initOK)
            {
                Common.Log.logError("HandlerCenter 消息处理中心为初始化 或者 初始化失败！！");
                return;
            }
            if (handlers.ContainsKey(data.Value.type))
            {
                handlers[data.Value.type].Invoke(data.Key, data.Value);
            }
        }
    }
}