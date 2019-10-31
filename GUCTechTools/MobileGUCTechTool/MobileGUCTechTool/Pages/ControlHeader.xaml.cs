﻿/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
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

namespace MobileGUCTechTool.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ControlHeader : StackLayout
	{
		public ControlHeader ()
		{
			InitializeComponent ();
		}
	}
}