using System;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;
using Pipelines.Sockets.Unofficial;

namespace UdpReverseProxy
{
    public class GameServer : SocketServer
    {
        protected override Task OnClientConnectedAsync(in ClientConnection client)
        {
            return Read(client.Transport);
        }

        private static async Task Read(IDuplexPipe transport)
        {
            ProxyServer server = null;
            try
            {
                var address = IPAddress.Parse("0.0.0.0");
                var endpoint = new IPEndPoint(address, 1337); // This is the message stream from and to the clients or load balancers ect.

                server = new ProxyServer(transport.Output);
                server.Listen(endpoint, listenBacklog: 7878);
                
                while (true)
                {
                    // We don't expect any data from game server but we need to keep message stream alive
                    await transport.Input.ReadAsync();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                transport.Output.Complete();
                server?.Stop();
            }
        }
    }
}