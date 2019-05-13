using System;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Pipelines.Sockets.Unofficial;

namespace UdpReverseProxy
{
    public static class SocketConnectionUdp
    {
        public static async Task<SocketConnection> ConnectAsync(
            EndPoint endpoint,
            ProtocolType protocolType,
            PipeOptions sendPipeOptions = null, PipeOptions receivePipeOptions = null,
            SocketConnectionOptions connectionOptions = SocketConnectionOptions.None,
            Func<SocketConnection, Task> onConnected = null,
            Socket socket = null, string name = null)
        {
            var addressFamily = endpoint.AddressFamily == AddressFamily.Unspecified ? AddressFamily.InterNetwork : endpoint.AddressFamily;
            //var protocolType = addressFamily == AddressFamily.Unix ? ProtocolType.Unspecified : ProtocolType.Tcp;
            if (socket == null)
            {
                socket = new Socket(addressFamily, SocketType.Stream, protocolType);
            }
            if (sendPipeOptions == null) sendPipeOptions = PipeOptions.Default;
            if (receivePipeOptions == null) receivePipeOptions = PipeOptions.Default;

            SocketConnection.SetRecommendedClientOptions(socket);

            using (var args = new SocketAwaitableEventArgs((connectionOptions & SocketConnectionOptions.InlineConnect) == 0 ? PipeScheduler.ThreadPool : null))
            {
                args.RemoteEndPoint = endpoint;

                if (!socket.ConnectAsync(args)) args.Complete();
                await args;
            }

            var connection = SocketConnection.Create(socket, sendPipeOptions, receivePipeOptions, connectionOptions, name);

            if (onConnected != null) await onConnected(connection).ConfigureAwait(false);

            return connection;
        }
    }
}