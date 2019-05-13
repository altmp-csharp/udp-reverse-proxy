using System;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Pipelines.Sockets.Unofficial;

namespace UdpReverseProxy
{
    public class ProxyServer : SocketServer
    {
        private readonly PipeWriter destination;
        
        public ProxyServer(PipeWriter destination)
        {
            this.destination = destination;
        }
        
        protected override Task OnClientConnectedAsync(in ClientConnection client)
        {
            return Echo(client.Transport);
        }
        
        private async Task Echo(IDuplexPipe transport)
        {
            try
            {
                while (true)
                {
                    var readResult = await transport.Input.ReadAsync();
                    await destination.WriteAsync(readResult.Buffer.First);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                transport.Output.Complete();
            }
        }
    }
}