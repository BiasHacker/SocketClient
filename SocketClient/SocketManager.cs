using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketClient
{
    public class SocketManager
    {
        private Socket socket { get; set; }

        public delegate void Error(Exception e);

        public event Error OnError;

        public SocketManager()
        {
            socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
        }

        public Task Connect(string server, int port)
        {
            try
            {
                var ip = IPResolver.GetEndPoint(server, port);
                var result = Task.Factory.FromAsync(
                    socket.BeginConnect,
                    socket.EndConnect, ip, null);
                result.ContinueWith(
                    (task) => OnError?.Invoke(task.Exception),
                    TaskContinuationOptions.OnlyOnFaulted);
                return result;
            }
            catch (Exception e)
            {
                OnError?.Invoke(e);
                return null;
            }
        }

        public Task Send(byte[] buffer)
        {
            try
            {
                var result = Task.Factory.FromAsync(
                    (callback, state) => socket.BeginSend(
                        buffer, 0, buffer.Length, 0, callback, state),
                    socket.EndSend, null);
                result.ContinueWith(
                    task => OnError?.Invoke(task.Exception),
                    TaskContinuationOptions.OnlyOnFaulted);
                return result;
            }
            catch (Exception e)
            {
                OnError?.Invoke(e);
                return null;
            }
        }

        private void Read()
        {
            throw new NotImplementedException();
        }
    }
}