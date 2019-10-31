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
	public partial class LightControls : ContentPage
    {
        private ControlsFrame ControlFrameReference;
        public LightControls (Picker bldgPicker, Picker roompicker, Classroom SelectedRoom, ControlsFrame controlsFrame)
		{
			InitializeComponent ();
            ControlFrameReference = controlsFrame;
            SolarShadesUp.Clicked += SolarShadesUp_Clicked;
            SolarShadesDown.Clicked += SolarShadesDown_Clicked;
            BlackoutShadesUp.Clicked += BlackoutShadesUp_Clicked;
            BlackoutShadesDown.Clicked += BlackoutShadesDown_Clicked;
            LightBank1On.Clicked += LightBank1On_Clicked;
            LightBank1Off.Clicked += LightBank1Off_Clicked;
            LightBank2On.Clicked += LightBank2On_Clicked;
            LightBank2Off.Clicked += LightBank2Off_Clicked;
            LightBank3On.Clicked += LightBank3On_Clicked;
            LightBank3Off.Clicked += LightBank3Off_Clicked;
		}

        private void SolarShadesUp_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("Shades1Up");
        }

        private void SolarShadesDown_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("Shades1Down");
        }

        private void BlackoutShadesUp_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("BlackoutUp");
        }

        private void BlackoutShadesDown_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("BlackoutDown");
        }

        private void LightBank1On_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("Light1On");
        }

        private void LightBank1Off_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("Light1Off");
        }

        private void LightBank2On_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("Light2On");
        }

        private void LightBank2Off_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("Light2Off");
        }

        private void LightBank3On_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("Light3On");
        }

        private void LightBank3Off_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("Light3Off");
        }
    }
}