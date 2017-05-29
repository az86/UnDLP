using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Network;

namespace Uedit32
{
    class Program
    {

        static void Main(string[] args)
        {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(new IPEndPoint(IPAddress.Any, 7420));
            sock.Listen(5);
            while (true)
            {
                var clientx = sock.Accept();
                ThreadPool.QueueUserWorkItem(obj => DealRequest(clientx));
            }
        }


        static void DealRequest(Socket clientx)
        {
            try
            {
                while (true)
                {
                    var fileName = clientx.ReceiveFile();
                    clientx.SendFileEx(fileName);
                    var dir = System.IO.Path.GetDirectoryName(fileName);
                    System.IO.File.Delete(fileName);
                    if (!string.IsNullOrEmpty(dir))
                        System.IO.Directory.Delete(dir, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
