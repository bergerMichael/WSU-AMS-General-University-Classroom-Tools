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

namespace GUCTechTools.Pages
{
    /// <summary>
    /// Interaction logic for AuthConfig.xaml
    /// </summary>
    public partial class AuthConfig : Page
    {
        private AMXController ControllerGuc;
        public DataSet.AuthConfigSet.ConnectedDevTableDataTable ConnectedDevSet = new DataSet.AuthConfigSet.ConnectedDevTableDataTable();
        private DataTable DevTable;

        public AuthConfig()
        {
            InitializeComponent();
            ControllerGuc = new AMXController();
            DevTable = new DataTable();
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
            spLogs.Children.Add(new TextBlock
            {
                Text = "Logs",
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TextAlignment = TextAlignment.Center,
                FontSize = 16,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x2F, 0x2F, 0x2F))
            });
        }

        public void Log(string message)
        {
            spLogs.Dispatcher.Invoke(() =>
            {
                spLogs.Children.Add(new TextBox { Text = DateTime.Now.ToString() + ": " + message, TextWrapping = TextWrapping.Wrap });
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

        private void CbRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != cbRoom.SelectedItem)
            {
                // First I will need to clear the ConnectedDevSet and table
                ConnectedDevSet.Clear();
                DevTable.Clear();
                DevTable = ConnectedDevSet;     // Dev table is now of the the same structure as ConnectedDevSet
                                                // The ConnectedDevSet will be built from UDP responses to the four auth config Get commands. This is done within ParseResponseToDataGrid()                

                // Connect to the selected controller
                DataTable dt = ((MainWindow)Application.Current.MainWindow)._1337;
                var IPs = (from row in dt.AsEnumerable()
                           where row.Field<string>("Building").Contains(cbBuilding.SelectedItem.ToString())
                           where row.Field<string>("Room") == cbRoom.SelectedItem.ToString()
                           select row["IP"].ToString()).ToList();

                foreach (string ip in IPs)
                {
                    GUCTechTools.DataSet.Classroom.ControllerDBRow controller = ((MainWindow)Application.Current.MainWindow)._1337.FindByIP(ip);
                    string type = "";
                    if (!controller.IsDesignationNull())
                    {
                        type = controller.Designation + " ";
                    }

                    if (!controller.UDP)
                    {
                        // display "Controller is not compatible with Authentication Configurator"
                        Log("Selected controller is not compatible with Authentication Configurator");
                    }
                    else
                    {
                        // Create a thread and send get commands
                        Log("Fetching data...");
                        Dispatcher.Invoke(() =>
                        {
                            string controllerIP = IPs.ElementAt(0);
                            ControllerGuc.IPAddress = controllerIP;
                            //ControllerGuc.Connect(controllerIP, 8888);
                            
                            ControllerGuc.SendCommand("!GETDEVLABELS", "");
                            
                            //ControllerGuc.SendCommand("!GETIP", "");
                            //ControllerGuc.SendCommand("!GETUSER", "");
                            //ControllerGuc.SendCommand("!GETPW", "");
                        });
                    }
                }            
            }
        }

        private void ParseResponseToDataGrid(string response)       // This function takes a response from MainWindow.UdpServer_ReceivedMessage, parses it and adds it to the data grid
        {
            if (ConnectedDevSet.Rows.Count == 0)    // If there is not data in ConnectedDevSet
            {
                // Initialize the rows
                string[] splitRows =  response.Split(',');
                int maxIdx = Convert.ToInt32(splitRows[splitRows.Length - 1][0].ToString());
                for (int i = 0; i < maxIdx; i++)
                {
                    
                    // create a new data row where Index = i
                    // save the row to ConnectedDevSet
                }
            }

            // response comes in the form of <field indicator 1-4>;<Device index>:<Content>,
            // fields are 1: Label, 2: IP, 3: Username, 4: Password            

            // split on ";" to separate field indicator from indicies and content
            // Save the field indicator
            // Split on "," to separate each <index>:<content> pair
            // For each <index>:<content> pair, split on ":"

            // pull the appropriate rows, one at a time based on index and change their relevant fields save the changes once finished

            // Each response will correspond to a data column, but all responses will indicate the index
            // Using this index I can find a data row by index in the ConnectedDevSet DataSet

            // Insert data into the correct columns of rows indicated by index
        }

        private void InstantiateAuthConfigDataTable()
        {
            connectedDevGrid.DataContext = ConnectedDevSet.DefaultView;
            connectedDevGrid.ItemsSource = ConnectedDevSet;
        }
    }
}
