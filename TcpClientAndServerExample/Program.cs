


namespace TcpClientAndServerExample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting threads");

            
            Thread tcpServerThread = new(() =>
            {
                _ = new MultipleConnectionTcpServer();
            });
            tcpServerThread.Start();


            Thread tcpClientThread = new(() =>
            {
                _ = new TCPClient();
            });
            tcpClientThread.Start();


            //Thread tcpClientThread2 = new(() =>
            //{
            //    _ = new TCPClient();
            //});
            //tcpClientThread2.Start();

        }
    }


}