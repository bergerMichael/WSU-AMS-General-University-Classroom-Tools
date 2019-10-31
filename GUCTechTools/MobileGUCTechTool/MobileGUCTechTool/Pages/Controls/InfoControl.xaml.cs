/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MobileGUCTechTool.Classes;

namespace MobileGUCTechTool.Pages.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InfoControl : ContentPage
	{
        Classroom RoomSelection;
        Dictionary<Guid, string> RoomIPs;

		public InfoControl (Picker bldgPicker, Picker roompicker, Classroom SelectedRoom)
		{
			InitializeComponent ();
            RoomSelection = SelectedRoom;
            RoomIPs = new Dictionary<Guid, string>();
            BuildInfoButtons();
		}

        // builds info buttons
        private void BuildInfoButtons()
        {
            if (null != RoomSelection)
            {
                DataTable dt = ((MainPage)Application.Current.MainPage)._Controllers;
                var IPs = (from row in dt.AsEnumerable()
                           where row.Field<string>("Building").Contains(RoomSelection.Building)
                           where row.Field<string>("Room") == RoomSelection.Room
                           select row["IP"].ToString()).ToList();
                DataTable projDT = ((MainPage)Application.Current.MainPage)._Projectors;
                var projIPs = (from row in projDT.AsEnumerable()
                               where row.Field<string>("Building").Contains(RoomSelection.Building)
                               where row.Field<string>("Room") == RoomSelection.Room
                               select row["IP"].ToString()).ToList();

                foreach (string ip in projIPs)
                {
                    Button button = new Button();
                    button.Text = "Projector " + ((MainPage)Application.Current.MainPage)._Projectors.FindByIP(ip).ProjNumber + " Login " + ip;
                    button.Clicked += NavigateToURL;
                    mainStack.Children.Add(button);
                    RoomIPs.Add(button.Id, ip);
                }
                foreach (string ip in IPs)
                {
                    // if the controller has a designation (ex: master, slave), append to button text
                    string type = "";
                    if (!((MainPage)Application.Current.MainPage)._Controllers.FindByIP(ip).IsDesignationNull())
                    {
                        type = ((MainPage)Application.Current.MainPage)._Controllers.FindByIP(ip).Designation;
                    }                    
                    Button buttonLogIn = new Button();
                    buttonLogIn.Text = "Controller Login " + type;
                    buttonLogIn.Clicked += NavigateToURL;
                    mainStack.Children.Add(buttonLogIn);
                    RoomIPs.Add(buttonLogIn.Id, ip);
                }
                foreach (string ip in IPs)
                {
                    var controller = ((MainPage)Application.Current.MainPage)._Controllers.FindByIP(ip);
                    string type = "";
                    if (!((MainPage)Application.Current.MainPage)._Controllers.FindByIP(ip).IsDesignationNull())
                    {
                        type = ((MainPage)Application.Current.MainPage)._Controllers.FindByIP(ip).Designation;
                    }

                    Button buttonRestart = new Button();
                    buttonRestart.Text = "Restart Controller " + type;
                    buttonRestart.Clicked += RestartController;
                    mainStack.Children.Add(buttonRestart);
                    RoomIPs.Add(buttonRestart.Id, ip);
                }
            }
        }

        private void RestartController(object mSender, EventArgs e)
        {
            Task.Run(() =>
            {
                string IP = "";
                string Building = "";
                string Room = "";
                string Tag = "";
                Task.Run(() =>
                {
                    Button sender = (Button)mSender;
                    IP = RoomIPs[sender.Id];
                    Building = ((MainPage)Application.Current.MainPage)._Controllers.FindByIP(IP).Building;
                    Room = ((MainPage)Application.Current.MainPage)._Controllers.FindByIP(IP).Room;
                    Tag = ((MainPage)Application.Current.MainPage)._Controllers.FindByIP(IP).Tag;
                });
                using (Renci.SshNet.SshClient sshClient = new Renci.SshNet.SshClient(IP, "Username", "Password"))
                {
                    sshClient.HostKeyReceived += (_sender, _e) => {
                        _e.CanTrust = true;
                    };
                    sshClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(30);
                    try
                    {
                        sshClient.Connect();
                    }
                    catch
                    {
                        return;
                    }

                    var amxStream = sshClient.CreateShellStream("amxStream", 0, 0, 0, 0, 256);

                    bool streamLockToken = true;
                    var streamTTL = DateTime.Now.Add(TimeSpan.FromSeconds(30));
                    while (streamLockToken)
                    {
                        if (amxStream.DataAvailable) { streamLockToken = false; }
                        else if (DateTime.Now >= streamTTL)
                        {
                            sshClient.Disconnect();
                            return;
                        }
                        else { System.Threading.Thread.Sleep(0); }
                    }

                    amxStream.WriteLine("reboot");

                    sshClient.Disconnect();
                }
            });
        }

        private void NavigateToURL(object sender, EventArgs e)
        {
            Button sendb = (Button)sender;
            System.Diagnostics.Process.Start("http://" + RoomIPs[sendb.Id]);
        }
    }
}