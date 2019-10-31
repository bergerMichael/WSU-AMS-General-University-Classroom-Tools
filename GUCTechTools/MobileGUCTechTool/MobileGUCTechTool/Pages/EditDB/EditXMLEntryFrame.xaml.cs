/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.    

using MobileGUCTechTool.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGUCTechTool.Pages.EditDB
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditXMLEntryFrame : ContentPage
	{
        //XDocument ControllerDT;
        DataTable ControllerDT;
        DataRow SelectedRow;
        private string StoredField;

        public EditXMLEntryFrame (Classroom roomSelection)
		{
			InitializeComponent ();
            ControllerDT = ((MainPage)Application.Current.MainPage)._Controllers;
            StoredField = "";

            LoadClassroomRow(roomSelection);
            BuildContent(roomSelection);
        }

        private void CloseKeyBoard()
        {
            // This function will be called when the enter key is pressed by the user.
            // This will force the keyboard to close confirming the edits to the text
        }

        private void LoadClassroomRow(Classroom roomSelection)  // selects the target row from the data table and saves it to the SelectedRow field
        {
            var capture = (from row in ControllerDT.AsEnumerable()
                              where row["Building"].ToString() == roomSelection.Building
                              where row["Room"].ToString() == roomSelection.Room
                              select row).ToList();
            SelectedRow = (DataRow)capture.ElementAt(0);

        }

        private void DesignationBox_Completed(object sender, EventArgs e)
        { 
            Entry entryCopy = (Entry)sender;
            SelectedRow["Designation"] = entryCopy.Text;

            ((MainPage)Application.Current.MainPage).SaveController();
        }

        private void TagBox_Completed(object sender, EventArgs e)
        {
            Entry entryCopy = (Entry)sender;
            SelectedRow["Tag"] = entryCopy.Text;

            ((MainPage)Application.Current.MainPage).SaveController();
        }

        private void LastCheckedBox_Completed(object sender, EventArgs e)
        {
            Entry entryCopy = (Entry)sender;
            SelectedRow["LastChecked"] = entryCopy.Text;

            ((MainPage)Application.Current.MainPage).SaveController();
        }

        private void IpBox_Completed(object sender, EventArgs e)
        {
            Entry entryCopy = (Entry)sender;
            SelectedRow["IP"] = entryCopy.Text;

            ((MainPage)Application.Current.MainPage).SaveController();
        }

        private void RoomName_Completed(object sender, EventArgs e)
        {
            Entry entryCopy = (Entry)sender;
            SelectedRow["Room"] = entryCopy.Text;

            ((MainPage)Application.Current.MainPage).SaveController();
        }

        private void BuildingName_Completed(object sender, EventArgs e)
        {
            Entry entryCopy = (Entry)sender;
            SelectedRow["Building"] = entryCopy.Text;

            ((MainPage)Application.Current.MainPage).SaveController();
        }

        private void UdpSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            Switch switchCopy = (Switch)sender;
            SelectedRow["UDP"] = switchCopy.IsToggled;

            ((MainPage)Application.Current.MainPage).SaveController();
        }

        private void Entry_Selected(object sender, FocusEventArgs e)
        {
            Entry capturedFocus = (Entry)sender;
            StoredField = capturedFocus.Text;
            
            capturedFocus.Text = "";
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            Entry entryCopy = (Entry)sender;
            if (entryCopy.Text == "")
            {
                entryCopy.Text = StoredField;
            }
        }

        private void BuildContent(Classroom roomSelection)  // Builds a ContentPage with controls for editing classroom data entries
        {
            // need 6 editors and one switch

            // Building stack
            StackLayout bldgStack = new StackLayout();
            bldgStack.Orientation = StackOrientation.Horizontal;
            Label bldgLabel = new Label();
            bldgLabel.Text = "Building:";
            bldgLabel.TextColor = Color.White;
            bldgLabel.FontSize = 20;
            Entry buildingName = new Entry();
            buildingName.Text = roomSelection.Building;
            buildingName.TextColor = Color.White;
            buildingName.Completed += BuildingName_Completed;
            buildingName.Focused += Entry_Selected;
            buildingName.Unfocused += Entry_Unfocused;
            buildingName.HorizontalOptions = LayoutOptions.FillAndExpand;
            buildingName.HorizontalTextAlignment = TextAlignment.End;
            bldgStack.Children.Add(bldgLabel);
            bldgStack.Children.Add(buildingName);

            // Room stack
            StackLayout roomStack = new StackLayout();
            roomStack.Orientation = StackOrientation.Horizontal;
            Label roomLabel = new Label();
            roomLabel.Text = "Room:";
            roomLabel.TextColor = Color.White;
            roomLabel.FontSize = 20;
            Entry roomName = new Entry();
            roomName.Text = roomSelection.Room;
            roomName.TextColor = Color.White;
            roomName.Completed += RoomName_Completed;
            roomName.Focused += Entry_Selected;
            roomName.Unfocused += Entry_Unfocused;
            roomName.HorizontalOptions = LayoutOptions.FillAndExpand;
            roomName.HorizontalTextAlignment = TextAlignment.End;
            roomStack.Children.Add(roomLabel);
            roomStack.Children.Add(roomName);

            // IP stack
            StackLayout ipStack = new StackLayout();
            ipStack.Orientation = StackOrientation.Horizontal;
            Label ipLabel = new Label();
            ipLabel.Text = "IP:";
            ipLabel.TextColor = Color.White;
            ipLabel.FontSize = 20;
            Entry ipBox = new Entry();
            ipBox.Text = roomSelection.IP;
            ipBox.TextColor = Color.White;
            ipBox.Completed += IpBox_Completed;
            ipBox.Focused += Entry_Selected;
            ipBox.Unfocused += Entry_Unfocused;
            ipBox.HorizontalOptions = LayoutOptions.FillAndExpand;
            ipBox.HorizontalTextAlignment = TextAlignment.End;
            ipStack.Children.Add(ipLabel);
            ipStack.Children.Add(ipBox);

            // UDP stack
            StackLayout udpStack = new StackLayout();
            udpStack.Orientation = StackOrientation.Horizontal;
            Label udpLabel = new Label();
            udpLabel.Text = "UDP Enabled";
            udpLabel.TextColor = Color.White;
            udpLabel.FontSize = 20;
            Switch udpSwitch = new Switch();
            udpSwitch.IsToggled = roomSelection.UDP;
            udpSwitch.Toggled += UdpSwitch_Toggled;
            udpSwitch.HorizontalOptions = LayoutOptions.EndAndExpand;
            udpStack.Children.Add(udpLabel);
            udpStack.Children.Add(udpSwitch);

            // Last Checked stack
            StackLayout lcStack = new StackLayout();
            lcStack.Orientation = StackOrientation.Horizontal;
            Label lcLabel = new Label();
            lcLabel.Text = "Last Checked:";
            lcLabel.TextColor = Color.White;
            lcLabel.FontSize = 20;
            Entry lastCheckedBox = new Entry();
            lastCheckedBox.Text = roomSelection.LastChecked;
            lastCheckedBox.TextColor = Color.White;
            lastCheckedBox.Completed += LastCheckedBox_Completed;
            lastCheckedBox.Focused += Entry_Selected;
            lastCheckedBox.Unfocused += Entry_Unfocused;
            lastCheckedBox.HorizontalOptions = LayoutOptions.FillAndExpand;
            lastCheckedBox.HorizontalTextAlignment = TextAlignment.End;
            lcStack.Children.Add(lcLabel);
            lcStack.Children.Add(lastCheckedBox);

            // Tag stack
            StackLayout tagStack = new StackLayout();
            tagStack.Orientation = StackOrientation.Horizontal;
            Label tagLabel = new Label();
            tagLabel.Text = "Tag:";
            tagLabel.TextColor = Color.White;
            tagLabel.FontSize = 20;
            Entry tagBox = new Entry();
            if (roomSelection.Tag == "")
                tagBox.Text = "No Tag";
            else
                tagBox.Text = roomSelection.Tag;
            tagBox.TextColor = Color.White;
            tagBox.Completed += TagBox_Completed;
            tagBox.Focused += Entry_Selected;
            tagBox.Unfocused += Entry_Unfocused;
            tagBox.HorizontalOptions = LayoutOptions.FillAndExpand;
            tagBox.HorizontalTextAlignment = TextAlignment.End;
            tagStack.Children.Add(tagLabel);
            tagStack.Children.Add(tagBox);

            // Designation stack
            StackLayout desStack = new StackLayout();
            desStack.Orientation = StackOrientation.Horizontal;
            Label desLabel = new Label();
            desLabel.Text = "Designation:";
            desLabel.TextColor = Color.White;
            desLabel.FontSize = 20;
            Entry designationBox = new Entry();
            if (roomSelection.Designation == "")
                designationBox.Text = "No Designation";
            else
                designationBox.Text = roomSelection.Designation;
            designationBox.TextColor = Color.White;
            designationBox.Completed += DesignationBox_Completed;
            designationBox.Focused += Entry_Selected;
            designationBox.Unfocused += Entry_Unfocused;
            designationBox.HorizontalOptions = LayoutOptions.FillAndExpand;
            designationBox.HorizontalTextAlignment = TextAlignment.End;
            desStack.Children.Add(desLabel);
            desStack.Children.Add(designationBox);

            mainContent.Children.Add(bldgStack);
            mainContent.Children.Add(roomStack);
            mainContent.Children.Add(ipStack);
            mainContent.Children.Add(udpStack);
            mainContent.Children.Add(lcStack);
            mainContent.Children.Add(tagStack);
            mainContent.Children.Add(desStack);
        }
    }
}