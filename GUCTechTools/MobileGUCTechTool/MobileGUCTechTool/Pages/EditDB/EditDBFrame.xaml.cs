/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.    

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MobileGUCTechTool.Classes;

namespace MobileGUCTechTool.Pages.EditDB
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditDBFrame : ContentPage
	{
        private string SelectedRoom;

		public EditDBFrame ()
		{
			InitializeComponent ();
            SelectedRoom = "";
		}

        private void LoadDB_Clicked(object sender, EventArgs e)
        {
            DataTable dt = ((MainPage)Application.Current.MainPage)._Controllers;
            var buildings = (from row in dt.AsEnumerable() select row.Field<string>("Building").Split(' ')[0]).ToList().GroupBy(x => x.Split(' ')[0]).Select(g => g.First()).ToList();
            buildings.Sort();

            buildingListView.ItemsSource = buildings;

            //this.LoadDB.Text = "Loaded " + ((MainPage)Application.Current.MainPage)._Controllers.Count + " data entries";
        }

        private void BuildingListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            DataTable dt = ((MainPage)Application.Current.MainPage)._Controllers;
            var rooms = (from row in dt.AsEnumerable() where row.Field<string>("Building").Contains(buildingListView.SelectedItem.ToString()) select row.Field<string>("Room").Split(' ')[0])
                .ToList().GroupBy(x => x.Split(' ')[0]).Select(g => g.First()).ToList();
            rooms.Sort();

            ListView roomListView = new ListView();
            roomListView.SeparatorColor = Color.White;
            roomListView.ItemsSource = rooms;
            roomListView.ItemTapped += RoomListView_ItemTapped;

            ContentPage roomPageContent = new ContentPage();
            roomPageContent.Content = new StackLayout { Children = { roomListView } };
            roomPageContent.BackgroundColor = Color.FromHex("#FF2F2F2F");
            roomPageContent.SetValue(NavigationPage.HasNavigationBarProperty, false);
            
            Navigation.PushAsync(roomPageContent);
            SelectedRoom = e.Item.ToString();
        }

        private async void RoomListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            DataTable dt = ((MainPage)Application.Current.MainPage)._Controllers;
            
            var IP = (from row in dt.AsEnumerable()
                       where row.Field<string>("Building").Contains(buildingListView.SelectedItem.ToString())
                       where row.Field<string>("Room").Contains(e.Item.ToString())
                       select row["IP"].ToString()).ToList();

            string capturedIP = IP.ElementAt(0);

            Classroom selectedRoom = new Classroom();
            selectedRoom.LoadClassroom(capturedIP);

            string data = "Building: " + selectedRoom.Building + "\n" +
                "Room: " + selectedRoom.Room + "\n" +
                "IP: " + selectedRoom.IP + "\n" +
                "IsUDP: " + selectedRoom.UDP.ToString() + "\n" +
                "Last Checked: " + selectedRoom.LastChecked + "\n" +
                "Tag: " + selectedRoom.Tag + "\n" +
                "Designation: " + selectedRoom.Designation;

            bool selectedAction = await DisplayAlert("Classroom Data", data, "Edit", "Back");   // Edit == true, Back == false

            if(selectedAction)
                EditXMLEntry(selectedRoom);

            //roomInfoContent.BackgroundColor = Color.FromHex("#FF2F2F2F");
            //roomInfoContent.SetValue(NavigationPage.HasNavigationBarProperty, false);

            //Navigation.PushAsync(roomInfoContent);
        }

        private void EditXMLEntry(Classroom selectedRoom)
        {
            EditXMLEntryFrame selection = new EditXMLEntryFrame(selectedRoom);

            selection.Title = (selectedRoom.Building + " " + selectedRoom.Room);
            Navigation.PushAsync(selection);
        }
    }
}