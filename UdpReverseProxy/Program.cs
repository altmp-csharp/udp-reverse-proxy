using System.Net;
using System.Net.Sockets;

namespace UdpReverseProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectToGameServer();
        }

        private static async void ConnectToGameServer()
        {
            var address = IPAddress.Parse("127.0.0.1"); //TODO: configurable game server ip
            var endpoint = new IPEndPoint(address, 7788);

            var conn = await SocketConnectionUdp.ConnectAsync(endpoint, ProtocolType.Udp);

            var proxyServerIp = IPAddress.Parse("0.0.0.0"); //TODO: make proxy accept ip configurable
            var proxyServerEndPoint = //TODO: make proxy port configurable
                new IPEndPoint(proxyServerIp,
                    1337); // This is the message stream from and to the clients or load balancers ect.

            var server = new ProxyServer(conn.Output);
            server.Listen(proxyServerEndPoint, protocolType: ProtocolType.Udp, listenBacklog: 7878);
        }
    }
}