/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
using System.Net.Sockets;

namespace GUCTechTools.Pages
{
    /// <summary>
    /// Interaction logic for AliveTest.xaml
    /// </summary>
    public partial class AliveTest : Page
    {
        ConcurrentDictionary<IPAddress, AliveTestHeartBeat> AliveTestSet;
        const int DEFAULT_TTL = 3;
        public System.Timers.Timer TTLCountdown;
        public System.Timers.Timer aliveRefreshTimer;
        public struct AliveTestHeartBeat
        {
            public int TTL;
            public AMXController controller;
        }

        public AliveTest()
        {
            InitializeComponent();
            AliveTestSet = new ConcurrentDictionary<IPAddress, AliveTestHeartBeat>();
            TTLCountdown = new System.Timers.Timer(600);
            aliveRefreshTimer = new System.Timers.Timer();
            aliveRefreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            aliveRefreshTimer.Interval = 600000;
            TTLCountdown.Elapsed += (object source, System.Timers.ElapsedEventArgs e) => {
                Dictionary<IPAddress, AliveTestHeartBeat> DeadSet = new Dictionary<IPAddress, AliveTestHeartBeat>();
                foreach(KeyValuePair<IPAddress, AliveTestHeartBeat> keyValue in AliveTestSet)
                {
                    AliveTestHeartBeat temp = keyValue.Value;
                    temp.TTL -= 1;
                    AliveTestSet[keyValue.Key] = temp;
                    if (temp.TTL <= 0)
                        DeadSet.Add(keyValue.Key, keyValue.Value);
                }
                foreach(KeyValuePair<IPAddress, AliveTestHeartBeat> keyValue in DeadSet)
                {
                    AliveTestHeartBeat temp;
                    AliveTestSet.TryRemove(keyValue.Key, out temp);
                    if (null != temp.controller)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ((MainWindow)Application.Current.MainWindow)._1337.FindByIP(temp.controller.IPAddress).UDP = false;
                            ((MainWindow)Application.Current.MainWindow).SaveController();
                        });
                        temp.controller.AliveTest();
                    }
                }
            };
            TTLCountdown.Start();

        }

        public void RunAliveTest()
        {
            Spinny.Dispatcher.Invoke(() =>
            {
                Spinny.IsActive = false;
            });
            DataRowCollection drc = null;
            spPass.Dispatcher.Invoke(() => { spPass.Children.Clear(); });
            spFail.Dispatcher.Invoke(() => { spFail.Children.Clear(); });
            Dispatcher.Invoke(() => {
                ((MainWindow)Application.Current.MainWindow).LoadControllerDB();
                drc = ((MainWindow)Application.Current.MainWindow)._1337.Rows;
            });
            Task.Run(() => {
                foreach (DataRow dr in drc)
                {
                    Task.Run(() =>
                    {
                        string building = dr["Building"].ToString();
                        string room = dr["Room"].ToString();
                        string ip = dr["IP"].ToString();

                        AMXController mXController = new AMXController();
                        mXController.Connect(ip, 8888);
                        mXController.ControllerHealthChanged += ((AMXController.ControllerHealthState newState) => {
                            Dispatcher.Invoke(() =>
                            {
                                
                                Button tb = new Button
                                {
                                    Name = (building + room).Replace(" ", ""),
                                    Content = building + " " + room,
                                    Tag = ip,
                                    Foreground = Brushes.White,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center
                                };

                                var childName = (building + room).Replace(" ", "");
                                TextBlock child = PendingFindChildByName(childName);
                                if (null != child)
                                    spPending.Children.Remove(child);

                                if (newState == AMXController.ControllerHealthState.Alive)
                                    AddToStackPanel(spPass, tb);//spPass.Children.Add(tb);
                                else
                                    AddToStackPanel(spFail, tb);//spFail.Children.Add(tb);
                                dr["LastChecked"] = DateTime.Now;

                                ((MainWindow)Application.Current.MainWindow).SaveController();
                            });
                        });
                        AliveTestHeartBeat alive = new AliveTestHeartBeat { TTL = DEFAULT_TTL, controller = mXController };
                        AliveTestSet.TryAdd(IPAddress.Parse(ip), alive);
                        Dispatcher.Invoke(() =>
                        {
                            TextBlock tbPending = new TextBlock
                            {
                                Name = (building + room).Replace(" ", ""),
                                Text = building + " " + room,
                                Foreground = Brushes.White,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                            spPending.Children.Add(tbPending);
                            spPending.ApplyTemplate();
                        });
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
                Dispatcher.Invoke(() =>
                {
                    ((MainWindow)Application.Current.MainWindow).LoadControllerDB();
                    DataRowCollection drc = ((MainWindow)Application.Current.MainWindow)._1337.Rows;
                    DataRow dataRow = drc.Find(IP);
                    if (null != dataRow)
                    {
                        string searchName = (dataRow["Building"].ToString() + dataRow["Room"].ToString()).Replace(" ", "");
                        string displayName = dataRow["Building"].ToString() + " " + dataRow["Room"].ToString();
                        
                        Button foundBlock = null;

                        if (spPass.FindName(searchName) != null)
                        {
                            foundBlock = spPass.FindName(searchName) as Button;
                            spPass.Children.Remove(foundBlock);
                        }
                        if (spFail.FindName(searchName) != null)
                        {
                            foundBlock = spFail.FindName(searchName) as Button;
                            spFail.Children.Remove(foundBlock);
                        }
                        Button search = foundBlock ?? new Button
                        {
                            Name = searchName,
                            Content = displayName,
                            Foreground = Brushes.White,
                            Tag = dataRow["IP"].ToString(),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                        };
                        TextBlock child = PendingFindChildByName(searchName);
                        if (null != child)
                            spPending.Children.Remove(child);
                        AddToStackPanel(spPass, search);
                        //spPass.Children.Add(search);
                        dataRow["UDP"] = true;
                        dataRow["LastChecked"] = DateTime.Now;
                        ((MainWindow)Application.Current.MainWindow).SaveController();
                    }
                });
            }
        }

        private TextBlock PendingFindChildByName(string name)
        {
            foreach (TextBlock uie in spPending.Children)
            {
                if (name == uie.Name)
                {
                    return uie;
                }
            }
            return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).udpServer.AliveTestRequested = true;
            Spinny.Dispatcher.Invoke(() =>
            {
                Spinny.IsActive = true;
            });
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            aliveRefreshTimer.Enabled = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            aliveRefreshTimer.Enabled = false;
        }

        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).udpServer.AliveTestRequested = true;
        }

        private void AddToStackPanel(StackPanel panel, Button button)
        {
            if (!panel.Children.Contains(button))
            {
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.HorizontalContentAlignment = HorizontalAlignment.Center;
                button.VerticalAlignment = VerticalAlignment.Center;
                button.Margin = new Thickness(2);
                button.Click += ((object sender, RoutedEventArgs e) => {
                    var name = (sender as Button).Content.ToString().Split(' ');
                    var building = name[0];
                    var room = name[1];
                    ((MainWindow)Application.Current.MainWindow).controlMenu.SetComboBoxes(building, room);
                    ((MainWindow)Application.Current.MainWindow).PageFrame.Navigate(((MainWindow)Application.Current.MainWindow).controlMenu);
                });//System.Diagnostics.Process.Start("http://" + (sender as Button).Tag.ToString()); });

                SortedList<string, char> uiButtonsSorted = new SortedList<string, char>();
                int maxSize = panel.Children.Count;
                for (int i = 0; i < maxSize; i++)
                {
                    uiButtonsSorted.Add((panel.Children[i] as Button).Content.ToString(), ' ');
                }
                string key = button.Content.ToString();
                if (!uiButtonsSorted.ContainsKey(key))
                {
                    uiButtonsSorted.Add(key, ' ');
                    panel.Children.Insert(uiButtonsSorted.IndexOfKey(key), button);
                }
            }
        }
    }
}
