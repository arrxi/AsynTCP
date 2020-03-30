using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Protocol;
using Common.Message;
using System.Net.Sockets;

namespace AsyncTCPServer {
    public static class InitHandler {
        public static void InitMsgHandlerCallBack()
        {
            UnityMsgHandler_Test.instance.Register();
        }
    }
    public class UnityMsgHandler_Test :Singleton<UnityMsgHandler_Test>, IMessageHandler {
        public void Register()
        {
            HandlerCenter.instance.Register(Common.Message.MsgType.Req_UnityTest, onReqUnityTest);
        }

        private void onReqUnityTest(Socket arg1, DataPack<NormalProtocol> arg2)
        {
            Req_UnityTest req = DataPack<NormalProtocol>.protocol.Deserialze<Req_UnityTest>(arg2.data);
            if (req==null)
            {
                Common.Log.logError("解析失败");
            }
            Common.Log.log("a: {0}\t b:{1}", req.a, req.b);
            Common.Log.log(arg1.RemoteEndPoint.ToString());
            var data = DataPack<NormalProtocol>.protocol.Serialize(MsgType.Res_UnityTest, new Res_UnityTest("hfek", "jfiodddddd"));
            arg1.Send(data);
        }
    }
}
