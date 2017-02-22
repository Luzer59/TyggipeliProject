using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantiCode.Netwokking
{
    [Serializable]
    class NetworkDataPacket
    {
        public NetworkDataPacket(string message, object data)
        {
            this.message = message;
            this.data = data;
        }
        public string message;
        public object data;
    }
}
