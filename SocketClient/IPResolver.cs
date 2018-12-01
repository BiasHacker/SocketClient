using System.Net;
using System.Net.Sockets;

namespace SocketClient
{
    public class IPResolver
    {
        public static IPAddress Get(string host)
        {
            try
            {
                var addressList = Dns.GetHostEntry(host).AddressList;

                for (var i = 0; i < addressList.Length; i++)
                {
                    if (addressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return addressList[i];
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static IPEndPoint GetEndPoint(string server, int port)
        {
            return new IPEndPoint(Get(server), port);
        }
    }
}
