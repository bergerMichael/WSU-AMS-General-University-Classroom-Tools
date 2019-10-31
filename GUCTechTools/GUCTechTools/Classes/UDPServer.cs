/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using GUCTechTools.Pages;
using System.Windows;
using System.Data;

namespace GUCTechTools
{
    public class UDPServer
    {
        private Task serverTask;
        private CancellationTokenSource ServerTokenSource = new CancellationTokenSource();
        private CancellationToken ServerCancelationToken;
        public DataSet.SchedulerAddress.SchedulerIPDataTable _ThisIsNotAServer = new DataSet.SchedulerAddress.SchedulerIPDataTable();

        public bool AliveTestRequested;

        public enum ServerState { Unkown, Started, Stopped}
        //public delegate void ServerStateChangeEventHandler(ServerState newState);
        public event EventHandler<ServerState> ServerStateChange;

        public struct OnReceivedMessageEventArgs { public IPAddress mIPAddress; public string mMessage; }
        public event EventHandler<OnReceivedMessageEventArgs> OnReceivedMessage;

        public void LoadScheduler()
        {
            _ThisIsNotAServer = new DataSet.SchedulerAddress.SchedulerIPDataTable();
            if (!System.IO.File.Exists("Scheduler.xml"))
            {
                _ThisIsNotAServer.WriteXml("Scheduler.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            _ThisIsNotAServer.ReadXml("Scheduler.xml");
        }

        private void RaiseServerStateChangeEvent(ServerState newState)
        {
            EventHandler<ServerState> handler = ServerStateChange;
            ServerState state = newState;
            handler?.Invoke(this, state);
        }
         
        public void RaiseServerMessageReceived(IPAddress ipAddress, string message)
        {
            EventHandler<OnReceivedMessageEventArgs> handler = OnReceivedMessage;
            OnReceivedMessageEventArgs e = new OnReceivedMessageEventArgs { mIPAddress = ipAddress, mMessage = message };
            handler?.Invoke(this, e);
        }

        public void StartServer()
        {
            if(null == serverTask)
            {
                serverTask = ServerThread();
                AliveTestRequested = false;
            }
            
        }
        public void StopServer()
        {
            if (null != serverTask)
                ServerTokenSource.Cancel();
        }
        private Task ServerThread()
        {
            ServerCancelationToken = ServerTokenSource.Token;
            return (Task.Factory.StartNew(() =>
            {
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 8888);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Bind(iPEndPoint);

                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remote = sender;

                byte[] data = new byte[1024];
                int recv;                                

                RaiseServerStateChangeEvent(ServerState.Started);

                while (!ServerCancelationToken.IsCancellationRequested)
                {
                    if (socket.Available > 0)
                    {
                        try
                        {
                            data = new byte[1024];
                            recv = socket.ReceiveFrom(data, ref remote);
                            string eventString = ASCIIEncoding.ASCII.GetString(data).TrimEnd('\0');
                            RaiseServerMessageReceived((remote as IPEndPoint).Address, eventString);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    if (AliveTestRequested)
                    {
                        try
                        {
                            string aliveData = "";
                            byte[] sendBytes = new byte[1024];
                            byte[] rcvPacket = new byte[1024];

                            string serverAddress = "";   // When SchedulerAddress data table is created this address will be obtained from there                                

                            LoadScheduler();
                            DataRowCollection drc = _ThisIsNotAServer.Rows;    // keep track of Scheduler for alive test

                            foreach (DataRow dr in drc)
                            {
                                serverAddress = dr["IP"].ToString();
                            }

                            IPAddress address = IPAddress.Parse(serverAddress);
                            IPEndPoint aliveRemote = new IPEndPoint(IPAddress.Any, 0);
                            //socket.Connect(address, 9009);      // and so will this port. For now it is localhost for testing purposes
                            IPEndPoint remoteServerIPEnd = new IPEndPoint(address, 9009);
                            EndPoint remoteServer = remoteServerIPEnd;
                            aliveData = "Request Alive Test";
                            sendBytes = Encoding.ASCII.GetBytes(DateTime.Now.ToString() + " " + aliveData);
                            socket.SendTo(sendBytes, remoteServer);

                            /*
                            rcvPacket = client.Receive(ref aliveRemote);

                            aliveData = Encoding.ASCII.GetString(rcvPacket);
                            RaiseServerMessageReceived(aliveRemote.Address, aliveData);
                            */
                            AliveTestRequested = false;                    
                        }
                        catch
                        {

                        }
                    }
                    Thread.Sleep(0);
                }

                socket.Close();
                RaiseServerStateChangeEvent(ServerState.Stopped);

                if (!ServerCancelationToken.IsCancellationRequested)
                    ServerCancelationToken.ThrowIfCancellationRequested();

            }, ServerCancelationToken));
        }
    }
}
