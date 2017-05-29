using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    public static class NetworkTCP
    {
        static int RecvBuf(this Socket sock, byte[] buf)
        {
            int recvLength = 0;
            while (recvLength < buf.Length)
            {
                var r = sock.Receive(buf, recvLength, buf.Length - recvLength, SocketFlags.None);
                recvLength += r;
            }
            return recvLength;
        }
        static int SendBuf(this Socket sock, byte[] buf)
        {
            int sentLength = 0;
            while (sentLength < buf.Length)
            {
                var r = sock.Send(buf, sentLength, buf.Length - sentLength, SocketFlags.None);
                sentLength += r;
            }
            return sentLength;
        }
        static int RecvInt32(this Socket sock)
        {
            var buf = new byte[4];
            sock.RecvBuf(buf);
            return BitConverter.ToInt32(buf, 0);
        }

        static int SendInt32(this Socket sock, int val)
        {
            return sock.SendBuf(BitConverter.GetBytes(val));
        }
        static int SendLong(this Socket sock, long val)
        {
            return sock.SendBuf(BitConverter.GetBytes(val));
        }
        static long RecvLong(this Socket sock)
        {
            var buf = new byte[sizeof(long)];
            sock.RecvBuf(buf);
            return BitConverter.ToInt64(buf, 0);
        }
        static string RecvString(this Socket sock)
        {
            var stringLength = sock.RecvInt32();
            var buf = new byte[stringLength];
            sock.RecvBuf(buf);
            return System.Text.Encoding.Unicode.GetString(buf);
        }

        static int SendString(this Socket sock, string str)
        {
            var fileNameBuf = System.Text.Encoding.Unicode.GetBytes(str);
            sock.SendInt32(fileNameBuf.Length);
            return sock.SendBuf(fileNameBuf);
        }

        public static string ReceiveFile(this Socket sock)
        {
            var fileName = sock.RecvString();
            try
            {
                var dir = System.IO.Path.GetDirectoryName(fileName);
                if (dir != null)
                {
                    Console.WriteLine("  >> create dir {0}", dir);
                    System.IO.Directory.CreateDirectory(dir);
                }
            }
            catch (Exception ex)
            {
               // Console.WriteLine(ex.Message);
            }


            Console.WriteLine("  >> file name received: {0}", fileName);
            var fileLength = sock.RecvLong();
            Console.WriteLine(" >> file length received： {0}", fileLength);
            var fileContent = new byte[fileLength];
            sock.RecvBuf(fileContent);
            Console.WriteLine(" >> file received：{0}", fileName);

            File.WriteAllBytes(fileName, fileContent);
            return fileName;
        }

        public static void SendFileEx(this Socket sock, string fileName)
        {
            sock.SendString(fileName);
            Console.WriteLine("  >> file name sent: {0}", fileName);
            var fileInfo = new FileInfo(fileName);
            sock.SendLong(fileInfo.Length);
            Console.WriteLine(" >> file length sent： {0}", fileInfo.Length);
            sock.SendFile(fileName);
            Console.WriteLine(" >> file sent：{0}", fileName);
        }
    }
}
