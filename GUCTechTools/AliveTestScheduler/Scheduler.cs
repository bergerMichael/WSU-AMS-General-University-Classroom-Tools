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
    class Scheduler
    {
        private Queue<IPEndPoint> AliveTestQueue { get; set; }
        private Timer AliveTestTimer;
        public UdpClient server;

        public Scheduler()
        {
            server = new UdpClient(9009);
            AliveTestQueue = new Queue<IPEndPoint>(); // holds endpoints for sending AliveTestUpdate messages
            AliveTestTimer = new Timer();
            AliveTestTimer.Interval = 6000;
            AliveTestTimer.Elapsed += OnTimerElapsed;
            AliveTestTimer.Start();            
        }

        private void OnTimerElapsed(Object source, System.Timers.ElapsedEventArgs e)
        {
            // if there is an alive test scheduled in the aliveTestQueue, send the command to run the test. If not, do nothing
            if (AliveTestQueue.Count != 0)
            {
                IPEndPoint endPoint = AliveTestQueue.Dequeue();

                try
                {
                    string data = "";
                    byte[] sendBytes = new byte[1024];
                    data = "Run Test";
                    sendBytes = Encoding.ASCII.GetBytes(data);
                    //client.Connect(endPoint.Address, endPoint.Port);
                    server.Send(sendBytes, sendBytes.GetLength(0), endPoint);
                    Console.WriteLine("Sending Run Test command to: " + endPoint.ToString());
                }
                catch
                {

                }
            }
            return;
        }

        public void AddTestIPEndPoint(IPEndPoint ep)
        {
            AliveTestQueue.Enqueue(ep);
        }
    }
}
