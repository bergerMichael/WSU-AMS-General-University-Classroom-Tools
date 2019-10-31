/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GUCTechTools
{
    public class AMXController : INetwork, IUPD
    {

        public HttpWebRequest Request { get; set; }
        public string IPAddress { get; set; }
        public bool IsUDPCompatible { get; set; }

        public enum ControllerHealthState { Unkown, Alive, Dead};
        public delegate void ControllerEventHealthChangeHandler(ControllerHealthState newState);
        public event ControllerEventHealthChangeHandler ControllerHealthChanged;

        public void RaiseControllerHealthChange(ControllerHealthState newState)
        {
            if (null != ControllerHealthChanged)
                ControllerHealthChanged(newState);
        }

        public void Connect(string IP, int port)
        {
            Request = WebRequest.CreateHttp(new Uri( "http://"+IP + ":" + port.ToString() + "/?REQUEST=0"));
            IPAddress = IP;
        }

        public void Disconnect()
        {
            
        }

        public async void AliveTest()
        {
            if (null == Request)
                throw new NullReferenceException("Controller IP is not instantiated");
            try
            {
                using (var responseAsync = await Request.GetResponseAsync())
                {
                    using (Stream dataStream = responseAsync.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string response = await reader.ReadToEndAsync();
                            if (response.Contains("Alive") || response.Contains("alive") || response.Contains("Controler Test"))
                            {
                                RaiseControllerHealthChange(ControllerHealthState.Alive);
                            }
                            else
                                RaiseControllerHealthChange(ControllerHealthState.Dead);
                        }
                    }
                }
            }
            catch (WebException)
            {
                RaiseControllerHealthChange(ControllerHealthState.Dead);
            }
        }

        public async void SendCommand(string command, string value)
        {
            try
            {
                using (UdpClient client = new UdpClient(IPAddress, 8888))
                {   
                    byte[] buffer = Encoding.ASCII.GetBytes(command + "=" + value);
                    await client.SendAsync(buffer, buffer.Length);

                }
            }
            catch
            {

            }
        }

        public async void SendHTTPCommand(string IP, int port, string caseConst)
        {
            Request = WebRequest.CreateHttp(new Uri("http://" + IP + ":" + port.ToString() + "/?REQUEST=" + caseConst));
            // Construct process request

            if (null == Request)
                throw new NullReferenceException("Controller IP is not instantiated");
            try
            {
                using (var responseAsync = await Request.GetResponseAsync())
                {
                    using (Stream dataStream = responseAsync.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string response = await reader.ReadToEndAsync();
                            ((MainWindow)Application.Current.MainWindow).Dispatcher.Invoke(() => {
                                ((MainWindow)Application.Current.MainWindow).controlMenu.Log(response);
                            });
                        }
                    }
                }
            }
            catch (WebException)
            {
                RaiseControllerHealthChange(ControllerHealthState.Dead);
            }
        }

        public void AliveTestUDP()
        {
            SendCommand("!REQUEST", "0");
        }
    }
}
