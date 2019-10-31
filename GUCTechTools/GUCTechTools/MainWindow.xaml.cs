/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Net.Sockets;
using MahApps.Metro.Controls;
using System.Net;
using System.Threading;

namespace GUCTechTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public DataSet.Classroom.ControllerDBDataTable _1337 = new DataSet.Classroom.ControllerDBDataTable();
        public DataSet.Classroom.ProjectorDBDataTable _H4X0R = new DataSet.Classroom.ProjectorDBDataTable();        
        private Pages.DB dBMenu = new Pages.DB();
        private Pages.AliveTest aliveTestMenu = new Pages.AliveTest();
        public Pages.Controls controlMenu = new Pages.Controls();
        public Pages.AuthConfig authConfigMenu = new Pages.AuthConfig();
        public UDPServer udpServer = new UDPServer();

        public MainWindow()
        {
            InitializeComponent();
            udpServer.OnReceivedMessage += UdpServer_ReceivedMessage;
            udpServer.StartServer();
            LoadControllerDB();
            LoadProjectorDB();
        }

        private void UdpServer_ReceivedMessage(object sender, UDPServer.OnReceivedMessageEventArgs e)
        {
            string ipAddress = e.mIPAddress.ToString();

            if (e.mMessage.Contains("Proccessing Command"))
            {
                return;
            }

            DataRowCollection drc = _1337.Rows;

            if (drc.Contains(ipAddress))
            {
                DataRow dataRow = drc.Find(ipAddress);
                controlMenu.Log(dataRow["Building"].ToString() + " " + dataRow["Room"].ToString() + " response: " + e.mMessage);
            }

            if (e.mMessage.Contains("Alive") || e.mMessage.Contains("alive") || e.mMessage.Contains("Controler Test"))
            {
                aliveTestMenu.UDPAlive(ipAddress, true);
                // write to log the ip and message 
            }
            else if (e.mMessage.Contains("Run Test"))
            {
                aliveTestMenu.RunAliveTest();
            }
            else if (e.mMessage.Contains("!GETDEVLABELS=") || e.mMessage.Contains("!GETIP=") || e.mMessage.Contains("!GETUSER=") || e.mMessage.Contains("!GETPW="))  // Not sure these are the right responses for auth config 
            {

            }
            else
            {

            }
        }

        public void LoadControllerDB()
        {
            _1337 = new DataSet.Classroom.ControllerDBDataTable();
            if (!System.IO.File.Exists("ControllerDB.xml"))
            {
                _1337.WriteXml("ControllerDB.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            _1337.ReadXml("ControllerDB.xml");
        }

        public void LoadProjectorDB()
        {
            _H4X0R = new DataSet.Classroom.ProjectorDBDataTable();
            if (!System.IO.File.Exists("ProjectorDB.xml"))
            {
                _H4X0R.WriteXml("ProjectorDB.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            _H4X0R.ReadXml("ProjectorDB.xml");
        }

        public  void SaveController()
        {
            _1337.WriteXml("ControllerDB.xml", System.Data.XmlWriteMode.WriteSchema);
        }

        public void SaveProjector()
        {
            _H4X0R.WriteXml("ProjectorDB.xml", System.Data.XmlWriteMode.WriteSchema);
        }

        public void SaveScheduler()
        {
            udpServer._ThisIsNotAServer.WriteXml("Scheduler.xml", System.Data.XmlWriteMode.WriteSchema);
        }

        private void PageFrame_Navigated(object sender, NavigationEventArgs e)
        {
            PageFrame.NavigationService.RemoveBackEntry();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PageFrame.Navigate(dBMenu);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PageFrame.Navigate(aliveTestMenu);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            PageFrame.Navigate(controlMenu);
        }
        
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            PageFrame.Navigate(authConfigMenu);
        }        

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            aliveTestMenu.TTLCountdown.Stop();
            aliveTestMenu.TTLCountdown.Dispose();
            udpServer.StopServer();
        }
    }
}
