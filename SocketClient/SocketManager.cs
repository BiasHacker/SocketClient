using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketClient
{
    public class SocketManager
    {
        public Socket Socket { get; set; }

        public delegate void Error(Exception e);

        public event Error OnError;

        public event Action OnClose;

        public event Action<byte[]> OnRecv;

        public SocketManager()
        {
            Socket = new Socket(
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
                    Socket.BeginConnect,
                    Socket.EndConnect, ip, null);
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
                    (callback, state) => Socket.BeginSend(
                        buffer, 0, buffer.Length, 0, callback, state),
                    Socket.EndSend, null);
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

        public void ReceiveStart()
        {
            var bytes = new byte[0xffff];
            Receive(bytes, size => {
                var copy = new byte[size];
                Array.Copy(bytes, 0, copy, 0, size);
                if (copy.Length == 0)
                {
                    OnClose?.Invoke();
                    return;
                }
                OnRecv?.Invoke(copy);
                ReceiveStart();
            });
        }

        private Task<int> Receive(byte[] buffer, Action<int> callback)
        {
            try
            {
                var result = Task.Factory.FromAsync(
                    (c, s) => Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, c, s),
                    Socket.EndReceive, null);
                result.ContinueWith(
                    task => OnError?.Invoke(task.Exception),
                    TaskContinuationOptions.OnlyOnFaulted);
                result.ContinueWith(task => callback(task.Result), TaskContinuationOptions.NotOnFaulted)
                    .ContinueWith(task => OnError?.Invoke(task.Exception), TaskContinuationOptions.OnlyOnFaulted);
                result.ContinueWith(task => OnError?.Invoke(task.Exception), TaskContinuationOptions.OnlyOnFaulted);
                return result;
            }
            catch (Exception e)
            {
                OnError?.Invoke(e);
                return null;
            }
        }

    }
}