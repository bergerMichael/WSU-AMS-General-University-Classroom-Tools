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
	public partial class CameraControls : ContentPage
	{
        private ControlsFrame ControlFrameReference;
        public CameraControls (Picker bldgPicker, Picker roompicker, Classroom SelectedRoom, ControlsFrame controlsFrame)
		{
			InitializeComponent();
            panUpButton.ImageSource = ImageSource.FromResource("MobileGUCTechTool.Icons.arrowUp.png");
            panDownButton.ImageSource = ImageSource.FromResource("MobileGUCTechTool.Icons.arrowDown.png");
            panLeftButton.ImageSource = ImageSource.FromResource("MobileGUCTechTool.Icons.arrowLeft.png");
            panRightButton.ImageSource = ImageSource.FromResource("MobileGUCTechTool.Icons.arrowRight.png");

            ControlFrameReference = controlsFrame;

            // Need to support hold/release for pan buttons

            panUpButton.Pressed += PanUpButton_Pressed;
            panLeftButton.Pressed += PanLeftButton_Pressed;
            panRightButton.Pressed += PanRightButton_Pressed;
            panDownButton.Pressed += PanDownButton_Pressed;
            panUpButton.Released += Pan_Release;
            panDownButton.Released += Pan_Release;
            panLeftButton.Released += Pan_Release;
            panRightButton.Released += Pan_Release;
            preset1Call.Clicked += Preset1Call_Clicked;
            preset1Set.Clicked += Preset1Set_Clicked;
            preset2Call.Clicked += Preset2Call_Clicked;
            preset2Set.Clicked += Preset2Set_Clicked;
            preset3Call.Clicked += Preset3Call_Clicked;
            preset3Set.Clicked += Preset3Set_Clicked;
            preset4Call.Clicked += Preset4Call_Clicked;
            preset4Set.Clicked += Preset4Set_Clicked;
            PowerOn.Clicked += PowerOn_Clicked;
            PowerOff.Clicked += PowerOff_Clicked;
        }

        private void PanDownButton_Pressed(object sender, EventArgs e)
        {            
            ControlFrameReference.ExecuteSendCommand("CameraPanTiltDown");
        }

        private void PanRightButton_Pressed(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("CameraPanTitltRight");
        }

        private void PanLeftButton_Pressed(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("CameraPanTiltLeft");
        }

        private void PanUpButton_Pressed(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("CameraPanTiltUp");
        }

        private void Pan_Release(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("CameraPanTiltStop");
        }

        private void PowerOff_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("CameraPowerOff");
        }

        private void PowerOn_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("CameraPowerOn");
        }

        private void Preset4Set_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("CameraSetPreset4");
        }

        private void Preset4Call_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("CameraPreset4");
        }

        private void Preset3Set_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("CameraSetPreset3");
        }

        private void Preset3Call_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("CameraPreset3");
        }

        private void Preset2Set_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("CameraSetPreset2");
        }

        private void Preset2Call_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("CameraPreset2");
        }

        private void Preset1Set_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("CameraSetPreset1");
        }

        private void Preset1Call_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("CameraPreset1");
        }

	}
}