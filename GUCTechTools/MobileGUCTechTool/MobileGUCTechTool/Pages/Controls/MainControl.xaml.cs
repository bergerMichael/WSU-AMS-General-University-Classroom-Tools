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
	public partial class MainControl : ContentPage
    {
        private ControlsFrame ControlFrameReference;

		public MainControl(Picker bldgPicker, Picker roomPicker, Classroom SelectedRoom, ControlsFrame controlsFrame)
		{
			InitializeComponent();
            projPwrOn.Clicked += ProjPwrOn_Clicked;
            projPwrOff.Clicked += ProjPwrOff_Clicked;
            projMute.Clicked += ProjMute_Clicked;
            projUnmute.Clicked += ProjUnmute_Clicked;
            projDown.Clicked += ProjDown_Clicked;
            projUp.Clicked += ProjUp_Clicked;
            ControlFrameReference = controlsFrame;
        }

        private void ProjPwrOn_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("SystemPowerOn");
        }
        private void ProjPwrOff_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("SystemPowerOff");
        }

        private void ProjDown_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("ScreenDown");
        }

        private void ProjUp_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("ScreenUp");
        }

        private void ProjMute_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("ProjectorPictureMute");
        }

        private void ProjUnmute_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("ProjectorPictureUnmute");
        }

    }
}