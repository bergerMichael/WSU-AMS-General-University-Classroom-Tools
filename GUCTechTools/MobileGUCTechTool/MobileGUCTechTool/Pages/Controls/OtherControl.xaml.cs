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
	public partial class OtherControl : ContentPage
    {
        private ControlsFrame ControlFrameReference;
        public OtherControl(Picker bldgPicker, Picker roompicker, Classroom SelectedRoom, ControlsFrame controlsFrame)
		{
			InitializeComponent();
            proj2PwrOn.Clicked += Proj2PwrOn_Clicked;
            proj2PwrOff.Clicked += Proj2PwrOff_Clicked;
            proj2Mute.Clicked += Proj2Mute_Clicked;
            proj2Unmute.Clicked += Proj2Unmute_Clicked;
            proj2screenUp.Clicked += Proj2screenUp_Clicked;
            proj2screenDown.Clicked += Proj2screenDown_Clicked; 
        }

        private void Proj2screenDown_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("Screen2Down");
        }

        private void Proj2screenUp_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("Screen2Up");
        }

        private void Proj2Unmute_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("Projector2PictureUnmute");
        }

        private void Proj2Mute_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("Projector2PictureMute");
        }

        private void Proj2PwrOff_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("Projector2PowerOff");
        }

        private void Proj2PwrOn_Clicked(object sender, EventArgs e)
        {
            ControlFrameReference.ExecuteSendCommand("Projector2PowerOn");            
        }
    }
}