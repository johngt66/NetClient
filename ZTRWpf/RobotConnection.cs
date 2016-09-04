using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ZTRWpf
{
    public class RobotConnection
    {
        public string IpAddress { get; set; }
        public RobotConnection() : this(string.Empty) { }
        public RobotConnection(string ipAddress)
        {
            IpAddress = ipAddress;
        }
        public bool IsReady { get { return Regex.IsMatch(IpAddress, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"); } }
        public string Send(string msg)
        {
            var text = "Not Ready";
            if (IsReady)
                using (var net = new TcpClient())
                {
                    net.Connect(IpAddress, 8080);
                    var stream = net.GetStream();
                    var bytes = Encoding.UTF8.GetBytes(msg);
                    stream.Write(bytes, 0, msg.Length);

                    while (net.Available == 0)
                        Thread.Sleep(10);

                    var len = net.Available;
                    var read = new byte[len];
                    stream.Read(read, 0, len);
                    text = Encoding.UTF8.GetString(read);
                    Console.WriteLine(text);
                    net.Close();
                }
            return text;
        }
    }
}
