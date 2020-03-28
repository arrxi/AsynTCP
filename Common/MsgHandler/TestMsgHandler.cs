using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common.Protocol;
using Common.Message;
namespace Common.MsgHandler {
    public class TestMsgHandler : Singleton<TestMsgHandler>, IMessageHandler {
        public void Register()
        {
            HandlerCenter.instance.Register(Message.MsgType.Req_Test, TestCallBack);
            HandlerCenter.instance.Register(MsgType.Res_Test, TestreqCallBack);
        }

        private void TestreqCallBack(Socket arg1, DataPack<NormalProtocol> arg2)
        {
            Res_Test res = DataPack<NormalProtocol>.protocol.Deserialze<Res_Test>(arg2.data);
            Log.log("name:{0}\tsource:{1}",res.name,res.source);
        }

        private void TestCallBack(Socket arg1, DataPack<NormalProtocol> arg2)
        {

            Req_Test req =  DataPack<NormalProtocol>.protocol.Deserialze<Req_Test>(arg2.data);
            Common.Log.log("id:{0}\tname:{1}",req.id,req.name);
            var data = DataPack<NormalProtocol>.protocol.Serialize<Res_Test>(MsgType.Res_Test, new Res_Test("arrxi", 435));
            arg1.Send(data);
        }
    }
}
