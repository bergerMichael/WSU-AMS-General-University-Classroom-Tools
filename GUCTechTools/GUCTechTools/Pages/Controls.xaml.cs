/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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
using System.Net.Sockets;
using System.IO;
using GUCTechTools.Classes;

namespace GUCTechTools.Pages
{
    /// <summary>
    /// Interaction logic for Controls.xaml
    /// </summary>

    public partial class Controls : Page
    {
        private IPAddress activeIPAdress;
        private AMXController ControllerGuc;
        private AMXController ControllerVC;

        public Controls()
        {
            InitializeComponent();
            ControllerGuc = new AMXController();
        }

        public void SetComboBoxes(string building, string room)
        {
            InstantiateBuildingComboBox();
            cbBuilding.SelectedItem = building;
            InstantiateRoomComboBox();
            cbRoom.SelectedItem = room;
        }

        private void InstantiateBuildingComboBox()
        {
            DataTable dt = ((MainWindow)Application.Current.MainWindow)._1337;
            var buildings = (from row in dt.AsEnumerable() select row.Field<string>("Building").Split(' ')[0]).ToList().GroupBy(x => x.Split(' ')[0]).Select(g => g.First()).ToList();
            buildings.Sort();
            cbBuilding.ItemsSource = buildings;
        }

        private void InstantiateRoomComboBox()
        {
            DataTable dt = ((MainWindow)Application.Current.MainWindow)._1337;
            if (cbBuilding.SelectedItem == null)
                return;
            var rooms = (from row in dt.AsEnumerable() where row.Field<string>("Building").Contains(cbBuilding.SelectedItem.ToString()) select row.Field<string>("Room").Split(' ')[0]).ToList().GroupBy(x => x.Split(' ')[0]).Select(g => g.First()).ToList();
            rooms.Sort();
            cbRoom.ItemsSource = rooms;
        }

        private void Logs_Button_Click(object sender, RoutedEventArgs e)
        {
            spLogs.Children.Clear();
            spLogs.Children.Add(new TextBlock { Text="Logs", VerticalAlignment=VerticalAlignment.Stretch,
                HorizontalAlignment=HorizontalAlignment.Stretch, TextAlignment=TextAlignment.Center, FontSize=16,
                Foreground=Brushes.White, Background=new SolidColorBrush(Color.FromArgb(0xFF, 0x2F, 0x2F, 0x2F)) });
        }

        public void Log(string message)
        {
            spLogs.Dispatcher.Invoke(() =>
            {
                spLogs.Children.Add(new TextBox { Text=DateTime.Now.ToString() + ": " + message, TextWrapping=TextWrapping.Wrap });
            });
        }

        private void CbBuilding_DropDownOpened(object sender, EventArgs e)
        {
            InstantiateBuildingComboBox();
        }

        private void CbRoom_DropDownOpened(object sender, EventArgs e)
        {
            InstantiateRoomComboBox();
        }

        private void CbBuilding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbRoom.ItemsSource = null;
        }

        private void RestartController(object mSender, RoutedEventArgs mE)
        {
            Task.Run(() =>
            {
                string IP = "";
                string Building = "";
                string Room = "";
                string Tag = "";
                (mSender as Button).Dispatcher.Invoke(() =>
                {
                    IP = (mSender as Button).Tag.ToString();
                    Building = ((MainWindow)Application.Current.MainWindow)._1337.FindByIP(IP).Building;
                    Room = ((MainWindow)Application.Current.MainWindow)._1337.FindByIP(IP).Room;
                    Tag = ((MainWindow)Application.Current.MainWindow)._1337.FindByIP(IP).Tag;
                });
                using (Renci.SshNet.SshClient sshClient = new Renci.SshNet.SshClient(IP, "username", "password"))
                {
                    sshClient.HostKeyReceived += (_sender, _e) => {
                        _e.CanTrust = true;
                    };
                    Log("Attempting to connect to " + Building + " " + Room + " " + Tag + " controller");
                    sshClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(30);
                    try
                    {
                        sshClient.Connect();
                    }
                    catch
                    {
                        Log("Controller " + Building + " " + Room + " " + Tag + " connection timed out");
                        return;
                    }
                    Log("Connected to " + Building + " " + Room + " " + Tag + " controller");
                    Log("Attempting to reboot " + Building + " " + Room + " " + Tag + " controller");

                    var amxStream = sshClient.CreateShellStream("amxStream", 0, 0, 0, 0, 256);

                    bool streamLockToken = true;
                    var streamTTL = DateTime.Now.Add(TimeSpan.FromSeconds(30));
                    while (streamLockToken)
                    {
                        if (amxStream.DataAvailable) { streamLockToken = false; }
                        else if (DateTime.Now >= streamTTL)
                        {
                            Log("Controller " + Building + " " + Room + " " + Tag + " connection nonresponsive");
                            sshClient.Disconnect();
                            return;
                        }
                        else { System.Threading.Thread.Sleep(0); }
                    }

                    amxStream.WriteLine("reboot");

                    Log("Reboot " + Building + " " + Room + " " + Tag + " command sent");

                    sshClient.Disconnect();
                }
            });
        }

        private void NavigateToURL(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://" + (sender as Button).Tag.ToString());
        }

        private void CbRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != cbRoom.SelectedItem)
            {
                spInfo.Children.Clear();
                DataTable dt = ((MainWindow)Application.Current.MainWindow)._1337;
                var IPs = (from row in dt.AsEnumerable()
                           where row.Field<string>("Building").Contains(cbBuilding.SelectedItem.ToString())
                           where row.Field<string>("Room") == cbRoom.SelectedItem.ToString()
                           select row["IP"].ToString()).ToList();
                DataTable projDT = ((MainWindow)Application.Current.MainWindow)._H4X0R;
                var projIPs = (from row in projDT.AsEnumerable()
                               where row.Field<string>("Building").Contains(cbBuilding.SelectedItem.ToString())
                               where row.Field<string>("Room") == cbRoom.SelectedItem.ToString()
                               select row["IP"].ToString()).ToList();
                foreach(string ip in projIPs)
                {
                    Button button = new Button
                    {
                        Content = "Projector " + ((MainWindow)Application.Current.MainWindow)._H4X0R.FindByIP(ip).ProjNumber + " Login",
                        Tag = ip,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Margin = new Thickness(10, 2, 10, 2)
                    };
                    button.Click += NavigateToURL;
                    spInfo.Children.Add(button);
                }
                foreach (string ip in IPs)
                {
                    GUCTechTools.DataSet.Classroom.ControllerDBRow controller = ((MainWindow)Application.Current.MainWindow)._1337.FindByIP(ip);
                    string type = "";
                    if (!controller.IsDesignationNull())
                    {
                        type = controller.Designation + " ";
                    }                    

                    Button buttonLogIn = new Button
                    {
                        Content = "Controller Login " + type,
                        Tag = ip,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Margin = new Thickness(10,2,10,2)
                    };
                    buttonLogIn.Click += NavigateToURL;
                    spInfo.Children.Add(buttonLogIn);
                }
                foreach (string ip in IPs)
                {
                    var controller = ((MainWindow)Application.Current.MainWindow)._1337.FindByIP(ip);
                    string type = "";
                    if (!controller.IsDesignationNull())
                    {
                        type = controller.Designation + " ";
                    }

                    Button buttonRestart = new Button
                    {
                        Content = "Restart Controller " + type,
                        Tag = ip,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Background = Brushes.DarkRed,
                        Margin = new Thickness(10,30,10,0)
                    };
                    buttonRestart.Click += RestartController;
                    spInfo.Children.Add(buttonRestart);
                }
                BuildControls();
            }
        }

        public virtual void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cbRoom.SelectedItem == null)
                return;

            ProcessRequest CMDTable = new ProcessRequest();

            string buttonCMD = (sender as Button).Tag.ToString();
            string buttonCMDCaseConst = CMDTable.ProcessRequests[buttonCMD].ToString();

            DataTable dt = ((MainWindow)Application.Current.MainWindow)._1337;
            var IPs = (from row in dt.AsEnumerable()
                       where row.Field<string>("Building").Contains(cbBuilding.SelectedItem.ToString())
                       where row.Field<string>("Room") == cbRoom.SelectedItem.ToString()
                       select row["IP"].ToString()).ToList();        

            string controllerIP = IPs.ElementAt(0);
            ControllerGuc.Connect(controllerIP, 8888);

            // check if the controller is UDP
            var isUDP = (from row in dt.AsEnumerable()
                       where row.Field<string>("Building").Contains(cbBuilding.SelectedItem.ToString())
                       where row.Field<string>("Room") == cbRoom.SelectedItem.ToString()
                       select row["UDP"]).ToList();

            if ((bool)isUDP.ElementAt(0))
            {
                ControllerGuc.SendCommand("!REQUEST", buttonCMDCaseConst);
            }
            else
            {
                ControllerGuc.SendHTTPCommand(controllerIP, 8888, buttonCMDCaseConst);
            }
        }

        private void BuildControls()
        {
            // check selected room's tag
            // if the tag is unique then build the controls to that room's spec
            // otherwise, build the room like normal
            DataTable dt = ((MainWindow)Application.Current.MainWindow)._1337;
            var IPs = (from row in dt.AsEnumerable()
                       where row.Field<string>("Building").Contains(cbBuilding.SelectedItem.ToString())
                       where row.Field<string>("Room") == cbRoom.SelectedItem.ToString()
                       select row["IP"].ToString()).ToList();

            string controllerIP = IPs.ElementAt(0);
            string tag = ((MainWindow)Application.Current.MainWindow)._1337.FindByIP(controllerIP).Tag;
            bool UDP = ((MainWindow)Application.Current.MainWindow)._1337.FindByIP(controllerIP).UDP;

            switch (tag)
            {
                case "Master":
                    BuildWebsterOtherTab();
                    BuildWebsterMediaTab();
                    break;
                case "Dual":
                    BuildDualOtherTab();
                    BuildDualMediaTab();
                    break;
                case "ADBF_VC":
                    BuildVCTab();
                    BuildStandardOtherTab();
                    BuildADBFMainTab();
                    BuildADBFMediaTab();
                    BuildADBFCameraTab();
                    break;
                case "BUST_VC":
                    BuildBustMainTab();
                    BuildVCTab();
                    BuildStandardOtherTab();
                    BuildStandardMediaTab();
                    BuildBustCameraTab();
                    break;
                case "ETRL_VC":
                    BuildVCTab();
                    BuildStandardOtherTab();
                    BuildStandardMediaTab();
                    break;
                case "WEB_VC":
                    BuildVCTab();
                    BuildStandardOtherTab();
                    BuildStandardMediaTab();
                    break;
                case "DCB_VC":
                    BuildVCTab();
                    BuildDualOtherTab();
                    BuildStandardMediaTab();
                    break;
                case "G45":
                    BuildG45MainTab();
                    BuildG45MediaTab();
                    break;
                default:
                    BuildStandardMainTab();
                    if (UDP)
                        BuildDualOtherTab();
                    else
                        BuildStandardOtherTab();
                    BuildStandardMediaTab();
                    break;
            }

            if (UDP)
            {
                BuildUDPCameraTab();
            }
            else
            {
                if (tag != "ADBF_VC" && tag != "BUST_VC")
                    BuildNon_UDPCameraTab();
            }
        }

        private void BuildStandardMainTab()
        {
            GUCTechTools.Pages.CustomRoomControls.MainTab stdMainTab = new GUCTechTools.Pages.CustomRoomControls.MainTab();

            GroupBox capturedGB = new GroupBox();
            Grid capturedGrid = new Grid();

            foreach (var item in stdMainTab.controlGrid.Children)
            {
                if (item.GetType() == typeof(GroupBox))
                {
                    continue;
                }
                else
                    ((Button)item).Click += Button_Click;
            }

            mainTab.Content = stdMainTab;
        }

        private void BuildADBFCameraTab()
        {
            CustomRoomControls.CameraControls_non_UDP non_UDP_CC = new CustomRoomControls.CameraControls_non_UDP();

            foreach (var item in non_UDP_CC.controlGrid.Children)
            {
                if (item.GetType() == typeof(Button))
                {
                    if (((Button)item).Content.ToString() == "Preset 1")
                    {
                        ((Button)item).Click += ((sender, e) =>
                        {
                            ControllerGuc.SendHTTPCommand("", 8888, "");
                        });
                        continue;
                    }
                    else if (((Button)item).Content.ToString() == "Preset 2")
                    {
                        ((Button)item).Click += ((sender, e) =>
                        {
                            ControllerGuc.SendHTTPCommand("", 8888, "");
                        });
                        continue;
                    }
                    else if (((Button)item).Content.ToString() == "Preset 3")
                    {
                        ((Button)item).Click += ((sender, e) =>
                        {
                            ControllerGuc.SendHTTPCommand("", 8888, "");
                        });
                        continue;
                    }
                    else if (((Button)item).Content.ToString() == "Preset 4")
                    {
                        ((Button)item).Click += ((sender, e) =>
                        {
                            ControllerGuc.SendHTTPCommand("", 8888, "");
                        });
                        continue;
                    }
                    else
                        ((Button)item).Click += Button_Click;
                }
                    
            }

            cameraTab.Content = non_UDP_CC;
        }

        private void BuildBustCameraTab()
        {
            CustomRoomControls.CameraControls_non_UDP non_UDP_CC = new CustomRoomControls.CameraControls_non_UDP();

            foreach (var item in non_UDP_CC.controlGrid.Children)
            {
                if (item.GetType() == typeof(Button))
                {
                    if (((Button)item).Content.ToString() == "Preset 1")
                    {
                        ((Button)item).Click += ((sender, e) =>
                        {
                            ControllerGuc.SendHTTPCommand("", 8888, "");
                        });
                    }
                    else if (((Button)item).Content.ToString() == "Preset 2")
                    {
                        ((Button)item).Click += ((sender, e) =>
                        {
                            ControllerGuc.SendHTTPCommand("", 8888, "");
                        });
                    }
                    else if (((Button)item).Content.ToString() == "Preset 3")
                    {
                        ((Button)item).Click += ((sender, e) =>
                        {
                            ControllerGuc.SendHTTPCommand("", 8888, "");
                        });
                    }
                    else
                        ((Button)item).Click += Button_Click;
                }

            }

            cameraTab.Content = non_UDP_CC;
        }

        private void BuildADBFMainTab()
        {
            // screen up and screen down are special case constants
            GUCTechTools.Pages.CustomRoomControls.MainTab stdMainTab = new GUCTechTools.Pages.CustomRoomControls.MainTab();

            GroupBox capturedGB = new GroupBox();
            Grid capturedGrid = new Grid();

            DataTable dt = ((MainWindow)Application.Current.MainWindow)._1337;
            var IPs = (from row in dt.AsEnumerable()
                       where row.Field<string>("Building").Contains(cbBuilding.SelectedItem.ToString())
                       where row.Field<string>("Room") == cbRoom.SelectedItem.ToString()
                       select row["IP"].ToString()).ToList();

            string controllerIP = IPs.ElementAt(0);
            string tag = ((MainWindow)Application.Current.MainWindow)._1337.FindByIP(controllerIP).Tag;
            ControllerGuc.Connect(controllerIP, 8888);

            foreach (var item in stdMainTab.controlGrid.Children)
            {
                if (item.GetType() == typeof(GroupBox))
                {
                    continue;
                }
                else if (((Button)item).Content.ToString() == "Screen Up")
                {
                    ((Button)item).Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand(controllerIP, 8888, "");
                    });
                }
                else if (((Button)item).Content.ToString() == "Screen Down")
                {
                    ((Button)item).Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand(controllerIP, 8888, "");
                    });
                }
                else
                    ((Button)item).Click += Button_Click;
            }

            mainTab.Content = stdMainTab;
        }

        private void BuildADBFMediaTab()
        {
            // screen up and screen down are special case constants
            GUCTechTools.Pages.CustomRoomControls.StandardMediatab stdMediaTab = new GUCTechTools.Pages.CustomRoomControls.StandardMediatab();

            GroupBox capturedGB = new GroupBox();
            Grid capturedGrid = new Grid();

            DataTable dt = ((MainWindow)Application.Current.MainWindow)._1337;
            var IPs = (from row in dt.AsEnumerable()
                       where row.Field<string>("Building").Contains(cbBuilding.SelectedItem.ToString())
                       where row.Field<string>("Room") == cbRoom.SelectedItem.ToString()
                       select row["IP"].ToString()).ToList();

            string controllerIP = IPs.ElementAt(0);
            string tag = ((MainWindow)Application.Current.MainWindow)._1337.FindByIP(controllerIP).Tag;
            ControllerGuc.Connect(controllerIP, 8888);

            foreach (Button b in stdMediaTab.controlGrid.Children)
            {
                if (b.Content.ToString() == "Local PC")
                {
                    b.Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand(controllerIP, 8888, "");
                    });
                }
                else if (b.Content.ToString() == "HDMI")
                {
                    b.Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand(controllerIP, 8888, "");
                    });
                }
                else if (b.Content.ToString() == "VGA")
                {
                    b.Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand(controllerIP, 8888, "");
                    });
                }
                else if (b.Content.ToString() == "Doc Cam")
                {
                    b.Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand(controllerIP, 8888, "");
                    });
                }
                else if (b.Content.ToString() == "AV")
                {
                    b.Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand(controllerIP, 8888, "");
                    });
                }
                else
                    b.Click += Button_Click;
            }

            mediaTab.Content = stdMediaTab;
        }

        private void BuildBustMainTab()
        {
            // screen up and screen down are special case constants
            GUCTechTools.Pages.CustomRoomControls.MainTab stdMainTab = new GUCTechTools.Pages.CustomRoomControls.MainTab();

            GroupBox capturedGB = new GroupBox();
            Grid capturedGrid = new Grid();

            DataTable dt = ((MainWindow)Application.Current.MainWindow)._1337;
            var IPs = (from row in dt.AsEnumerable()
                       where row.Field<string>("Building").Contains(cbBuilding.SelectedItem.ToString())
                       where row.Field<string>("Room") == cbRoom.SelectedItem.ToString()
                       select row["IP"].ToString()).ToList();

            string controllerIP = IPs.ElementAt(0);
            string tag = ((MainWindow)Application.Current.MainWindow)._1337.FindByIP(controllerIP).Tag;
            ControllerGuc.Connect(controllerIP, 8888);

            foreach (var item in stdMainTab.controlGrid.Children)
            {
                if (item.GetType() == typeof(GroupBox))
                {
                    continue;
                }
                else if (((Button)item).Content.ToString() == "Screen Up")
                {
                    ((Button)item).Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand("", 8888, "");
                    });
                }
                else if (((Button)item).Content.ToString() == "Screen Down")
                {
                    ((Button)item).Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand("", 8888, "");
                    });
                }
                else
                    ((Button)item).Click += Button_Click;
            }

            mainTab.Content = stdMainTab;
        }

        private void BuildStandardOtherTab()
        {
            GUCTechTools.Pages.CustomRoomControls.StandardOtherTab stdOtherTab = new GUCTechTools.Pages.CustomRoomControls.StandardOtherTab();

            GroupBox capturedGB = new GroupBox();
            Grid capturedGrid = new Grid();

            foreach (var item in stdOtherTab.controlGrid.Children)
            {
                if (item.GetType() == typeof(GroupBox))
                {
                    capturedGB = (GroupBox)item;
                    capturedGrid = (Grid)capturedGB.Content;
                    foreach (Button b in capturedGrid.Children)
                    {
                        b.Click += Button_Click;
                    }
                }
            }

            otherTab.Content = stdOtherTab;
        }

        private void BuildStandardMediaTab()
        {
            GUCTechTools.Pages.CustomRoomControls.StandardMediatab stdMediaTab = new GUCTechTools.Pages.CustomRoomControls.StandardMediatab();

            GroupBox capturedGB = new GroupBox();
            Grid capturedGrid = new Grid();

            foreach (Button b in stdMediaTab.controlGrid.Children)
            {
                b.Click += Button_Click;
            }

            mediaTab.Content = stdMediaTab;
        }

        private void BuildDualOtherTab()
        {
            GUCTechTools.Pages.CustomRoomControls._2Proj2Screen dualOtherTab = new GUCTechTools.Pages.CustomRoomControls._2Proj2Screen();
            
            GroupBox capturedGB = new GroupBox();
            Grid capturedGrid = new Grid();

            foreach (var item in dualOtherTab.controlGrid.Children)
            {
                if (item.GetType() == typeof(GroupBox))
                {
                    capturedGB = (GroupBox)item;
                    capturedGrid = (Grid)capturedGB.Content;
                    foreach (Button b in capturedGrid.Children)
                    {
                        b.Click += Button_Click;
                    }
                }
            }

            otherTab.Content = dualOtherTab;
        }

        private void BuildDualMediaTab()
        {
            GUCTechTools.Pages.CustomRoomControls.DualMedia dualMediaTab = new GUCTechTools.Pages.CustomRoomControls.DualMedia();
            
            GroupBox capturedGB = new GroupBox();
            Grid capturedGrid = new Grid();

            foreach (var item in dualMediaTab.controlGrid.Children)
            {
                if (item.GetType() == typeof(GroupBox))
                {
                    capturedGB = (GroupBox)item;
                    capturedGrid = (Grid)capturedGB.Content;
                    foreach (Button b in capturedGrid.Children)
                    {
                        b.Click += Button_Click;
                    }
                }
            }

            mediaTab.Content = dualMediaTab;
        }

        private void BuildVCTab()
        {
            GUCTechTools.Pages.CustomRoomControls.VCMode vcTab = new GUCTechTools.Pages.CustomRoomControls.VCMode();

            DataTable dt = ((MainWindow)Application.Current.MainWindow)._1337;
            var IPs = (from row in dt.AsEnumerable()
                       where row.Field<string>("Building").Contains(cbBuilding.SelectedItem.ToString())
                       where row.Field<string>("Room") == cbRoom.SelectedItem.ToString()
                       select row["IP"].ToString()).ToList();

            string controllerIP = IPs.ElementAt(0);
            string tag = ((MainWindow)Application.Current.MainWindow)._1337.FindByIP(controllerIP).Tag;
            ControllerGuc.Connect(controllerIP, 8888);

            switch (tag)
            {
                case "ADBF_VC":
                    ((Button)vcTab.controlGrid.Children[0]).Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand(controllerIP, 8888, "");
                        ControllerGuc.SendHTTPCommand("", 8888, "");
                    });
                    ((Button)vcTab.controlGrid.Children[1]).Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand(controllerIP, 8888, "");
                        ControllerGuc.SendHTTPCommand("", 8888, "");
                    });
                    break;
                case "ETRL_VC":
                    ((Button)vcTab.controlGrid.Children[0]).Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand(controllerIP, 8888, "");
                    });
                    ((Button)vcTab.controlGrid.Children[1]).Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand(controllerIP, 8888, "");
                    });
                    break;
                case "BUST_VC":
                    ((Button)vcTab.controlGrid.Children[0]).Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand("", 8888, "");
                    });
                    ((Button)vcTab.controlGrid.Children[1]).Click += ((sender, e) =>
                    {
                        ControllerGuc.SendHTTPCommand("", 8888, "");
                    });
                    break;
                case "WEB_VC":
                    ((Button)vcTab.controlGrid.Children[0]).Click += ((sender, e) =>
                    {
                        ControllerGuc.SendCommand("!REQUEST", "");
                    });
                    ((Button)vcTab.controlGrid.Children[1]).Click += ((sender, e) =>
                    {
                        ControllerGuc.SendCommand("!REQUEST", "");
                    });
                    break;
                default:
                    return;
            }

            VC.Content = vcTab;
        }

        private void BuildWebsterOtherTab()
        {
            GUCTechTools.Pages.CustomRoomControls.WebsterOtherControl websterOtherTab = new GUCTechTools.Pages.CustomRoomControls.WebsterOtherControl();
            

            foreach (GroupBox item in websterOtherTab.controlGrid.Children)
            {
                foreach (Button b in ((Grid)item.Content).Children)
                {
                    b.Click += Button_Click;
                }
            }

            otherTab.Content = websterOtherTab;
        }

        private void BuildWebsterMediaTab()
        {
            GUCTechTools.Pages.CustomRoomControls.WebsterMediatab webMediaTab = new GUCTechTools.Pages.CustomRoomControls.WebsterMediatab();
            GroupBox capturedGB = new GroupBox();
            Grid capturedGrid = new Grid();

            foreach (var item in webMediaTab.controlGrid.Children)
            {
                if (item.GetType() == typeof(GroupBox))
                {
                    capturedGB = (GroupBox)item;
                    capturedGrid = (Grid)capturedGB.Content;
                    foreach (Button b in capturedGrid.Children)
                    {
                        b.Click += Button_Click;
                    }
                }                    
            }

            mediaTab.Content = webMediaTab;
        }

        private void Button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ProcessRequest CMDTable = new ProcessRequest();
            DataTable dt = ((MainWindow)Application.Current.MainWindow)._1337;
            var IPs = (from row in dt.AsEnumerable()
                       where row.Field<string>("Building").Contains(cbBuilding.SelectedItem.ToString())
                       where row.Field<string>("Room") == cbRoom.SelectedItem.ToString()
                       select row["IP"].ToString()).ToList();

            string controllerIP = IPs.ElementAt(0);
            ControllerGuc.Connect(controllerIP, 8888);

            // check if the controller is UDP
            var isUDP = (from row in dt.AsEnumerable()
                         where row.Field<string>("Building").Contains(cbBuilding.SelectedItem.ToString())
                         where row.Field<string>("Room") == cbRoom.SelectedItem.ToString()
                         select row["UDP"]).ToList();

            if ((bool)isUDP.ElementAt(0))
            {
                if (((Button)sender).Content.ToString() == "Zoom In" || ((Button)sender).Content.ToString() == "Zoom Out")
                {
                    ControllerGuc.SendCommand("!REQUEST", CMDTable.ProcessRequests["CameraZoomStop"].ToString());
                }
                else
                    ControllerGuc.SendCommand("!REQUEST", CMDTable.ProcessRequests["CameraPanTiltStop"].ToString());
            }
            else
            {
                ControllerGuc.SendHTTPCommand(controllerIP, 8888, CMDTable.ProcessRequests["CameraPanTiltStop"].ToString());
            }
        }

        private void BuildNon_UDPCameraTab()
        {
            CustomRoomControls.CameraControls_non_UDP non_UDP_CC = new CustomRoomControls.CameraControls_non_UDP();

            foreach (var item in non_UDP_CC.controlGrid.Children)
            {
                if (item.GetType() == typeof(Button))
                    ((Button)item).Click += Button_Click;
            }

            cameraTab.Content = non_UDP_CC;
        }

        private void BuildUDPCameraTab()
        {
            CustomRoomControls.CameraControls UDP_CC = new CustomRoomControls.CameraControls();

            foreach (var item in UDP_CC.controlGrid.Children)
            {
                if (item.GetType() == typeof(Button))
                {
                    if (((Button)item).Name != null)
                    {
                        ((Button)item).PreviewMouseLeftButtonDown += Button_Click;
                        ((Button)item).PreviewMouseLeftButtonUp += Button_MouseLeftButtonUp;
                    }
                    else
                        ((Button)item).Click += Button_Click;
                }                    
            }

            cameraTab.Content = UDP_CC;
        }

        private void BuildG45MainTab()
        {
            CustomRoomControls.G45MainTab G45_Main = new CustomRoomControls.G45MainTab();

            foreach (var item in G45_Main.controlGrid.Children)
            {
                if (item.GetType() == typeof(Button))
                {
                    ((Button)item).Click += Button_Click;
                }
            }

            mainTab.Content = G45_Main;
        }

        private void BuildG45MediaTab()
        {
            GUCTechTools.Pages.CustomRoomControls.G45MediaTab g45MediaTab = new GUCTechTools.Pages.CustomRoomControls.G45MediaTab();
            mediaTab.Content = g45MediaTab;
        }
    }
}
