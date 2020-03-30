using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Message {

    public abstract class IDataPack {
    }

    public class Req_Login : IDataPack {
    }

    public class Req_Test : IDataPack {
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

    public class Res_Test : IDataPack {
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

    public class Res_UnityTest:IDataPack {
        public string name;
        public string name1;
        public Res_UnityTest()
        {

        }
        public Res_UnityTest(string name, string name1)
        {
            this.name = name;
            this.name1 = name1;
        }
    }
    public class Req_UnityTest:IDataPack {
        public int a;
        public int b;
        public Req_UnityTest() { }
        public Req_UnityTest(int a, int b)
        {
            this.a = a;
            this.b = b;
        }
    }
}