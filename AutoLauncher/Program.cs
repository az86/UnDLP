using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Process
            {
                StartInfo =
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "Uedit32.exe"
                }
            };
            p.Start();
        }
    }
}
