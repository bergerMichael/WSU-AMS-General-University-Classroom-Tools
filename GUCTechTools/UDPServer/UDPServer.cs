using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace UDPServer
{
    class UDPServer
    {
        static void Main(string[] args)
        {
            // Create a timer to track when alive test can be run
            double aliveCooldownInterval = 600000;      // set to the desired alive test interval
            Timer aliveCooldown = new Timer(aliveCooldownInterval);

            // Create a UDP client
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9999);     // what port to use?
            UdpClient newSocket = new UdpClient(ipep);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 9999);

            //data = newSocket.Receive(ref sender);

            while(true)
            {
                try
                {
                    data = newSocket.Receive(ref sender);

                    Console.WriteLine("Recieved a aliveTest request from: " + Encoding.ASCII.GetString(data, 0, data.Length));

                    data = Encoding.ASCII.GetBytes(aliveCooldown.Interval.ToString());

                    newSocket.Send(data, data.Length, sender);
                }
                catch { }
            }

            // receive()
            /// The server will receive requests for the aliveCooldown from the tool
            /// In this function, the server sends the client a response that indicates whether aliveTest is available
            /// If aliveTest is not available, the server sends a response that indicates when the next aliveTest is available.
            /// The server will then increase the cooldown to track the scheduled test.
            /// This way the server will prevent the client from having to send multiple messages
        }
    }
}
