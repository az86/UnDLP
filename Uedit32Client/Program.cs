using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Network;

namespace Uedit32Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(IPAddress.Parse(ConfigurationManager.AppSettings["srvIP"]), 7420);

                var files = FileFiles(args);
                foreach (var file in files)
                {
                    sock.SendFileEx(file);
                    sock.ReceiveFile();
                }
                sock.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        static string[] FileFiles(string[] args)
        {
            string[] files = {};
            if (args.Length == 0)
                files = System.IO.Directory.GetFiles(".", "*", SearchOption.AllDirectories);
            foreach (var arg in args)
            {
                var subfiles = System.IO.Directory.GetFiles(".", args[0], SearchOption.AllDirectories);
                
                files = files.Concat(subfiles).ToArray();
            }
            var filesE = System.IO.Directory.GetFiles(".", "*.dll", SearchOption.AllDirectories);
            files = files.Except(filesE).ToArray();
            filesE = System.IO.Directory.GetFiles(".", "*.exe", SearchOption.AllDirectories);
            files = files.Except(filesE).ToArray();
            return files.Select(str=>str.Remove(0, ".".Length+1)).ToArray();
        }
    }
}
