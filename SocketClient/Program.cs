namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // TEST CODE
            var sock = new SocketManager();
            sock.Connect("hoge.pl", 19975).Wait();
            sock.Send(new byte[] { 0, 1, 2, 3 });
        }
    }
}
