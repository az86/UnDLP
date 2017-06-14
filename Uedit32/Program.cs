using System;
using System.Collections.Generic;
using System.IO;
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
                var sum = clientx.RecvInt32();
                if (sum == 0)
                {
                    System.IO.Directory.Delete("TempCleartext", true);
                    Console.WriteLine(">>clear server temp directory OK!");
                }
                else
                {
                    for (var i = 0; i < sum; i++)
                    {
                        var fileName = clientx.ReceiveFile();
                        clientx.SendFileEx(fileName);
                    }
                    Console.WriteLine(">>{0} files ok!", sum);
                }
                clientx.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
