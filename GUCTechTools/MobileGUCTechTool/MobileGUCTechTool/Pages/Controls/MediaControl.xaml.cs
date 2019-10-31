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
using MobileGUCTechTool.Classes;

namespace MobileGUCTechTool.Pages.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MediaControl : ContentPage
    {
        private ControlsFrame ControlFrameReference;
        public MediaControl (Picker bldgPicker, Picker roompicker, Classroom SelectedRoom, ControlsFrame controlsFrame)
		{
			InitializeComponent ();
            VGAButton.ImageSource = ImageSource.FromResource("MobileGUCTechTool.Icons.VGAWhite2x.png");
            HDMIButton.ImageSource = ImageSource.FromResource("MobileGUCTechTool.Icons.HDMIWhite2x.png");
            LocalPCButton.ImageSource = ImageSource.FromResource("MobileGUCTechTool.Icons.LocalPCWhite2x.png");
            DocCamButton.ImageSource = ImageSource.FromResource("MobileGUCTechTool.Icons.DocCamWhite2x.png");
            AVButton.ImageSource = ImageSource.FromResource("MobileGUCTechTool.Icons.Comp2x.png");
            ControlFrameReference = controlsFrame;
            VGAButton.Clicked += VGAButton_Clicked;
            HDMIButton.Clicked += HDMIButton_Clicked;
            LocalPCButton.Clicked += LocalPCButton_Clicked;
            DocCamButton.Clicked += DocCamButton_Clicked;
            AVButton.Clicked += AVButton_Clicked;
        }

        private void AVButton_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("SingleAV");
        }

        private void DocCamButton_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("SingleDocumentCamera");
        }

        private void LocalPCButton_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("SingleComputer");
        }

        private void HDMIButton_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("SingleHDMI");
        }

        private void VGAButton_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("SingleLaptop");
        }
    }
}