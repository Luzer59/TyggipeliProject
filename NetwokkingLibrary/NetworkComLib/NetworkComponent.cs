using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace QuantiCode.Netwokking
{
    public class NetworkComponent
    {
        public const int defaultBufferSize = 1024;
        public const int defaultPort = 7777;

        protected int _bufferSize = 1024;
        public int bufferSize
        {
            get
            {
                return _bufferSize;
            }
        }
        protected int _port = 7777;
        public int port
        {
            get
            {
                return _port;
            }
        }

        public static IPAddress GetLocalIPAddress()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] ipAddressList = ipHostInfo.AddressList;
            int ipAddressIndex = 0;

            for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
            {
                if (ipAddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddressIndex = i;
                }
            }
            return ipAddressList[ipAddressIndex];
        }
    }
}
