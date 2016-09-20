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
    public class RobotConnection : IDisposable
    {
        public string IpAddress { get; set; }
        private TcpClient Net { get; set; }

        public RobotConnection() : this(string.Empty)
        { }
        public RobotConnection(string ipAddress)
        {
            IpAddress = ipAddress;
            Net = new TcpClient();
        }
        public bool IsReady { get { return Regex.IsMatch(IpAddress, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"); } }
        public bool IsConnected { get { return Net.Connected; } }
        public string Send(string msg)
        {
            var text = "Not Ready";
            if (IsReady)
            {
                if (!IsConnected)
                    Net.Connect(IpAddress, 8080);

                var stream = Net.GetStream();
                var bytes = Encoding.UTF8.GetBytes(msg);
                stream.Write(bytes, 0, msg.Length);

                while (Net.Available == 0)
                    Thread.Sleep(10);

                var len = Net.Available;
                var read = new byte[len];
                stream.Read(read, 0, len);
                text = Encoding.UTF8.GetString(read);
                Console.WriteLine(text);
            }
            return text;
        }

        public void Dispose()
        {
            if (Net.Connected)
                Net.Close();
        }
    }
}
