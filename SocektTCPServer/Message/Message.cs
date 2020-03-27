using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocektTCPServer.Message
{
    public abstract class Message
    {
        public string fromIP;
        public int fromPort;
        public string toIP;
        public int toPort;


    }
}
