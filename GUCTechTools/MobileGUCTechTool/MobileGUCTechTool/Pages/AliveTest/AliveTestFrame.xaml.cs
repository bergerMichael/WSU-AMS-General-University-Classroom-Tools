/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.

using MobileGUCTechTool.Classes;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGUCTechTool.Pages.AliveTest
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AliveTestFrame : ContentPage
	{
        ConcurrentDictionary<IPAddress, AliveTestHeartBeat> AliveTestSet;
        const int DEFAULT_TTL = 3;
        public System.Timers.Timer TTLCountdown;
        public System.Timers.Timer aliveRefreshTimer;
        Dictionary<string, string> RoomIPs;
        List<string> PendingList;
        ActivityIndicator indicator;        
        public struct AliveTestHeartBeat
        {
            public int TTL;
            public AMXController controller;
        }
        public AliveTestFrame ()
		{
			InitializeComponent ();
            BuildTestFrame();
		}
        
        private void BuildTestFrame()
        {
            AliveTestSet = new ConcurrentDictionary<IPAddress, AliveTestHeartBeat>();
            TTLCountdown = new System.Timers.Timer(600);
            aliveRefreshTimer = new System.Timers.Timer();
            aliveRefreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            aliveRefreshTimer.Interval = 600000;
            indicator = new ActivityIndicator();
            MainStack.Children.Add(indicator);
            TTLCountdown.Elapsed += (object source, System.Timers.ElapsedEventArgs e) => {
                Dictionary<IPAddress, AliveTestHeartBeat> DeadSet = new Dictionary<IPAddress, AliveTestHeartBeat>();
                foreach (KeyValuePair<IPAddress, AliveTestHeartBeat> keyValue in AliveTestSet)
                {
                    AliveTestHeartBeat temp = keyValue.Value;
                    temp.TTL -= 1;
                    AliveTestSet[keyValue.Key] = temp;
                    if (temp.TTL <= 0)
                        DeadSet.Add(keyValue.Key, keyValue.Value);
                }
                foreach (KeyValuePair<IPAddress, AliveTestHeartBeat> keyValue in DeadSet)
                {
                    AliveTestHeartBeat temp;
                    AliveTestSet.TryRemove(keyValue.Key, out temp);
                    if (null != temp.controller)
                    {
                        Task.Run(() =>
                        {
                            ((MainPage)Application.Current.MainPage)._Controllers.FindByIP(temp.controller.IPAddress).UDP = false;
                            ((MainPage)Application.Current.MainPage).SaveController();
                        });
                        temp.controller.AliveTest();
                    }
                }
            };
            TTLCountdown.Start();
        }

        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            ((MainPage)Application.Current.MainPage).udpServer.AliveTestRequested = true;
        }

        
        public async void RunAliveTest()  // called when scheduler sends command
        {
            // Sends a process request to all controllers and returns their status            
            
            DataRowCollection drc = null;
            await Task.Run(() => { 
                deadPick = new Picker();
                alivePick = new Picker();
            });
            await Task.Run(() => {
                ((MainPage)Application.Current.MainPage).LoadControllerDB();
                drc = ((MainPage)Application.Current.MainPage)._Controllers.Rows;
            });
            Task.Run(() => {
                foreach (DataRow dr in drc)
                {
                    Task.Run(() =>
                    {
                        string building = dr["Building"].ToString();
                        string room = dr["Room"].ToString();
                        string ip = dr["IP"].ToString();
                        string designation = dr["Designation"].ToString();

                        AMXController mXController = new AMXController();
                        mXController.Connect(ip, 8888);
                        mXController.ControllerHealthChanged += ((AMXController.ControllerHealthState newState) => {
                            Task.Run(() =>
                            {

                                string roomName = building + " " + room;

                                var childName = (building + room).Replace(" ", "");
                                
                                if (PendingList.Contains(ip))
                                    PendingList.Remove(ip);

                                if (newState == AMXController.ControllerHealthState.Alive)
                                {
                                    alivePick.Items.Add(roomName);
                                    RoomIPs.Add(roomName, ip);
                                }
                                else
                                {
                                    deadPick.Items.Add(roomName);
                                    RoomIPs.Add(roomName, ip);
                                }   
                                
                                dr["LastChecked"] = DateTime.Now;

                                ((MainPage)Application.Current.MainPage).SaveController();
                                if (PendingList.Count == 0)
                                    indicator.IsRunning = false;
                                    
                            });
                        });
                        AliveTestHeartBeat alive = new AliveTestHeartBeat { TTL = DEFAULT_TTL, controller = mXController };
                        AliveTestSet.TryAdd(IPAddress.Parse(ip), alive);
                        PendingList.Add(ip);
                        Task.Factory.StartNew(() => mXController.AliveTestUDP());
                    });
                }
            });
        }

        public void UDPAlive(string IP, bool IsAlive)
        {
            if (IsAlive)
            {
                AliveTestSet.TryRemove(IPAddress.Parse(IP), out AliveTestHeartBeat temp);
                Task.Run(() =>
                {
                    ((MainPage)Application.Current.MainPage).LoadControllerDB();
                    DataRowCollection drc = ((MainPage)Application.Current.MainPage)._Controllers.Rows;
                    DataRow dataRow = drc.Find(IP);
                    if (null != dataRow)
                    {
                        string searchName = dataRow["Building"].ToString() + " " + dataRow["Room"].ToString();

                        if (alivePick.Items.Contains(searchName))
                        {
                            alivePick.Items.Remove(searchName);
                        }
                        if (deadPick.Items.Contains(searchName))
                        {
                            deadPick.Items.Remove(searchName);
                        }
                        if (PendingList.Contains(IP))
                            PendingList.Remove(IP);
                        alivePick.Items.Add(searchName);
                        dataRow["UDP"] = true;
                        dataRow["LastChecked"] = DateTime.Now;
                        ((MainPage)Application.Current.MainPage).SaveController();
                    }
                });
            }
        }

        private void aliveTestButton_Clicked(object sender, EventArgs e)
        {
            ((MainPage)Application.Current.MainPage).udpServer.AliveTestRequested = true;
            Task.Run(() =>
            {                
                indicator.IsRunning = true;
            });
        }

        // TODO: Create handler for picker item selected event to navigate to controls frame
    }
}