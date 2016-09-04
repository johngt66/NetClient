using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            for (var i = 100; i >= 0; i -= 10)
            {
                using (var net = new TcpClient())
                {
                    net.Connect("192.168.1.103", 8080);
                    var stream = net.GetStream();
                    var msg = Encoding.UTF8.GetBytes(string.Format("L{0:000}R{0:000}", i));
                    stream.Write(msg, 0, msg.Length);

                    while (net.Available == 0) ;

                    var len = net.Available;
                    var read = new byte[len];
                    stream.Read(read, 0, len);
                    var text = Encoding.UTF8.GetString(read);
                    Console.WriteLine(text);
                    net.Close();
                }
            }
            Console.ReadLine();
        }
    }
}
