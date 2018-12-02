using System;
using System.Globalization;
using System.Text;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var hex = "736f636b657420636c69656e74";
            var bytes = HexStringToBytes(hex);

            // TEST CODE
            var sock = new SocketManager();
            sock.Connect("hoge.pl", 19975).Wait();
            sock.Send(bytes);
            sock.OnRecv += Sock_OnRecv;
            sock.ReceiveStart();
            sock.Close();

            while (true) Console.ReadLine();
        }

        private static void Sock_OnRecv(byte[] data)
        {
            Console.WriteLine(BitConverter.ToString(data));
        }

        static byte[] HexStringToBytes(string input)
        {
            var b = new byte[input.Length / 2];
            for (int i = 0; i < input.Length; i += 2)
                b[i / 2] = byte.Parse(
                    input.Substring(i, 2), 
                    NumberStyles.HexNumber);
            return b;
        }
    }
}
