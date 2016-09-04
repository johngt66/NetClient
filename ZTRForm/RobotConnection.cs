using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZTRForm
{
    public class RobotConnection
    {
        public string address { get; set; }
        public RobotConnection() : this(string.Empty) { }
        public RobotConnection(string ipAddress)
        {
            address = ipAddress;
        }
        public bool IsReady { get { return Regex.IsMatch(address, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"); } }
        public string Send(string msg)
        {
            var text = "Not Ready";
            if (IsReady)
                using (var net = new TcpClient())
                {
                    net.Connect(address, 8080);
                    var stream = net.GetStream();
                    var bytes = Encoding.UTF8.GetBytes(msg);
                    stream.Write(bytes, 0, msg.Length);

                    while (net.Available == 0) ;

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
