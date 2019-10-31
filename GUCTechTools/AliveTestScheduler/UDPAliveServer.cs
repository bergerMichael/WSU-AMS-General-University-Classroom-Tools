/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace AliveTestScheduler
{
    class UDPAliveServer
    {
        static void Main(string[] args)
        {
            Scheduler AliveTestScheduler = new Scheduler();

            string data = "";            

            IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("Server started");
            Console.WriteLine("Waiting for client...");
            while (true)
            {
                byte[] receivedBytes = AliveTestScheduler.server.Receive(ref remoteIPEndPoint);
                data = Encoding.ASCII.GetString(receivedBytes);
                Console.WriteLine("Handling client at: " + remoteIPEndPoint);
                Console.WriteLine("Message Received: " + data.TrimEnd());

                AliveTestScheduler.AddTestIPEndPoint(remoteIPEndPoint);
            }
        }      
    }
}
