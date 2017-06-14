using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
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
                if (args[0] == "!clear")
                {
                    sock.SendInt32(0);
                    Console.WriteLine(">>clear server temp directory OK!");
                }
                else
                {
                    var files = FindFiles(args);
                    sock.SendInt32(files.Length);
                    foreach (var file in files)
                    {
                        sock.SendFileEx(file);
                        sock.ReceiveFile();
                    }
                    Console.WriteLine(">>{0} files ok! all files path store in files.txt", files.Length);
                    System.IO.File.WriteAllLines("files.txt", files);
                }
                sock.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        static string[] FindFiles(string[] args)
        {
            string[] files = {};
            if (args.Length == 0)
                files = System.IO.Directory.GetFiles(".", "*", SearchOption.AllDirectories);
            foreach (var arg in args)
            {
                var subfiles = System.IO.Directory.GetFiles(".", args[0], SearchOption.AllDirectories);
                
                files = files.Concat(subfiles).ToArray();
            }
            var filesE = new string[] {@".\Uedit32Client.exe", @".\Uedit32Client.exe.config", @".\Network.dll" };
            files = files.Except(filesE).ToArray();
            return files;
        }
    }
}
