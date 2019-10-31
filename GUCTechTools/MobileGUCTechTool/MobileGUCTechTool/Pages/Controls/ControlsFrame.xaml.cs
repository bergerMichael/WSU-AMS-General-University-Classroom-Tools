/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CarouselView.FormsPlugin.Abstractions;
using System.Data;
using MobileGUCTechTool.Classes;

namespace MobileGUCTechTool.Pages.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ControlsFrame : ContentPage
	{
        CarouselPage Carousel;
        private Classroom SelectedRoom;
        private AMXController ControllerGUC;
        private ProcessRequest CMDTable = new ProcessRequest();
        public ControlsFrame()
		{
			InitializeComponent();
            Carousel = new CarouselPage();
            SelectedRoom = new Classroom();
            bldgPick.SelectedIndexChanged += BuildingPicker_SelectedIndexChanged;
            bldgPick.Focused += LoadBuildingPickerData;
            roomPick.Unfocused += RoomPick_Unfocused;
            ControllerGUC = new AMXController();
        }

        private void PopulateCarouselView()     // This function will populate the CarouselView with the proper ContentViews given the building/room selection
        {
            // switch on room tag and build the corresponding controls for that case of room


            DataTable dt = ((MainPage)Application.Current.MainPage)._Controllers;
            var IPs = (from row in dt.AsEnumerable()
                       where row.Field<string>("Building").Contains(SelectedRoom.Building)
                       where row.Field<string>("Room") == SelectedRoom.Room
                       select row["IP"].ToString()).ToList();

            string controllerIP = IPs.ElementAt(0);
            string tag = ((MainPage)Application.Current.MainPage)._Controllers.FindByIP(controllerIP).Tag;
            bool UDP = ((MainPage)Application.Current.MainPage)._Controllers.FindByIP(controllerIP).UDP;

            // Initialze list so that pages can be added when they are initialized
            List<ContentPage> contentViews = new List<ContentPage>()
            {
                new MainControl(bldgPick, roomPick, SelectedRoom, this),
                new OtherControl(bldgPick, roomPick, SelectedRoom, this),
                new MediaControl(bldgPick, roomPick, SelectedRoom, this),
                new LightControls(bldgPick, roomPick, SelectedRoom, this),
                new CameraControls(bldgPick, roomPick, SelectedRoom, this),
                new VCControl(bldgPick, roomPick, SelectedRoom, this),
                new InfoControl(bldgPick, roomPick, SelectedRoom)
            };
            // TODO: create custom pages for special case rooms
            /*
            switch (tag)
            {
                case "Master":  
                    //BuildWebsterOtherTab();
                    //BuildWebsterMediaTab();
                    break;
                case "Dual":
                    //BuildDualOtherTab();
                    //BuildDualMediaTab();
                    break;
                case "ADBF_VC":
                    //BuildVCTab();
                    //BuildStandardOtherTab();
                    //BuildADBFMainTab();
                    //BuildADBFMediaTab();
                    //BuildADBFCameraTab();
                    break;
                case "BUST_VC":
                    //BuildBustMainTab();
                    //BuildVCTab();
                    //BuildStandardOtherTab();
                    //BuildStandardMediaTab();
                    //BuildBustCameraTab();
                    break;
                case "ETRL_VC":
                    //BuildVCTab();
                    //BuildStandardOtherTab();
                    //BuildStandardMediaTab();
                    break;
                case "WEB_VC":
                    //BuildVCTab();
                    //BuildStandardOtherTab();
                    //BuildStandardMediaTab();
                    break;
                case "DCB_VC":
                    //BuildVCTab();
                    //BuildDualOtherTab();
                    //BuildStandardMediaTab();
                    break;
                case "G45":
                    //BuildG45MainTab();
                    //BuildG45MediaTab();
                    break;
                default:
                    //BuildStandardMainTab();
                    if (UDP) 
                        //BuildDualOtherTab();
                    else
                        //BuildStandardOtherTab();
                    //BuildStandardMediaTab();
                    break;
            }

            if (UDP)
            {
                //BuildUDPCameraTab();
            }
            else
            {
                if (tag != "ADBF_VC" && tag != "BUST_VC")
                    //BuildNon_UDPCameraTab();
            }
            */

            foreach (ContentPage v in contentViews)
                Carousel.Children.Add(v);
            Carousel.BackgroundColor = Color.FromHex("#FF2F2F2F");
            Carousel.Title = (SelectedRoom.Building + " " + SelectedRoom.Room);
            this.Navigation.PushAsync(Carousel);
        }

        public void LoadBuildingPickerData(object sender, FocusEventArgs e)
        {
            if (bldgPick.ItemsSource == null)
            {
                DataTable dt = ((MainPage)Application.Current.MainPage)._Controllers;
                var buildings = (from row in dt.AsEnumerable() select row.Field<string>("Building").Split(' ')[0]).ToList().GroupBy(x => x.Split(' ')[0]).Select(g => g.First()).ToList();
                buildings.Sort();

                bldgPick.ItemsSource = buildings;
            }
        }

        private void BuildingPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker pCopy = (Picker)sender;
            string selectedBuilding = pCopy.SelectedItem.ToString();

            DataTable dt = ((MainPage)Application.Current.MainPage)._Controllers;
            var rooms = (from row in dt.AsEnumerable() where row.Field<string>("Building").Contains(selectedBuilding) select row.Field<string>("Room").Split(' ')[0])
                .ToList().GroupBy(x => x.Split(' ')[0]).Select(g => g.First()).ToList();
            rooms.Sort();

            roomPick.ItemsSource = rooms;            
        }

        private void RoomPick_Unfocused(object sender, FocusEventArgs e)
        {
            if (roomPick.SelectedItem == null)
                return;

            DataTable dt = ((MainPage)Application.Current.MainPage)._Controllers;

            string selectedRoomIP = (from row in dt.AsEnumerable()
                                     where row.Field<string>("Building").Contains(bldgPick.SelectedItem.ToString())
                                     where row.Field<string>("Room").Contains(roomPick.SelectedItem.ToString())
                                     select row.Field<string>("IP")).ElementAt(0);
            SelectedRoom.LoadClassroom(selectedRoomIP);           
            PopulateCarouselView();
        }

        public void ExecuteSendCommand(string request)      // the proper strings to be passed to this function can be found on previous button tags
        {
            string procReqString = CMDTable.ProcessRequests[request].ToString();

            DataTable dt = ((MainPage)Application.Current.MainPage)._Controllers;

            var IPs = (from row in dt.AsEnumerable()
                       where row.Field<string>("Building").Contains(SelectedRoom.Building.ToString())
                       where row.Field<string>("Room") == SelectedRoom.Room.ToString()
                       select row["IP"].ToString()).ToList();

            string controllerIP = IPs.ElementAt(0);
            ControllerGUC.Connect(controllerIP, 8888);

            // check if the controller is UDP
            var isUDP = (from row in dt.AsEnumerable()
                         where row.Field<string>("Building").Contains(SelectedRoom.Building.ToString())
                         where row.Field<string>("Room") == SelectedRoom.Room.ToString()
                         select row["UDP"]).ToList();

            if ((bool)isUDP.ElementAt(0))
            {
                ControllerGUC.SendCommand("!REQUEST", procReqString);
            }
            else
            {
                ControllerGUC.SendHTTPCommand(controllerIP, 8888, procReqString);
            }
        }
    }
}