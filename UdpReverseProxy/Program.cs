using System.Net;

namespace UdpReverseProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var address = IPAddress.Parse("0.0.0.0");
            var endpoint = new IPEndPoint(address, 1338);// This is the message stream from and to the game server

            var server = new GameServer();
            server.Listen(endpoint, listenBacklog: 7878);
        }
    }
}