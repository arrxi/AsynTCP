using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Message {

    public abstract class IMessage {
    }

    public class Req_Login : IMessage {
    }

    public class Req_Test : IMessage {
        public string id;
        public string name;

        public Req_Test()
        {
        }

        public Req_Test(string id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

    public class Res_Test : IMessage {
        public string name;
        public int source;

        public Res_Test()
        {
        }

        public Res_Test(string name, int source)
        {
            this.name = name;
            this.source = source;
        }
    }
}