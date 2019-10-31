/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.    

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using Xamarin.Forms;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace MobileGUCTechTool.Classes
{
    public class Classroom
    {
        // Building, Room, IP, UDP, Last checked, Tag, Designation
        public string Building { get; private set; }
        public string Room { get; private set; }
        public string IP { get; private set; }
        public bool UDP { get; private set; }
        public string LastChecked { get; private set; }
        public string Tag { get; private set; }
        public string Designation { get; private set; }

        public Classroom(){ }

        public void LoadClassroom(string ip) // this function populates the fields of this class with Linq statements
        {
            DataTable dt = ((MainPage)Application.Current.MainPage)._Controllers;

            var selectedBldg = (from row in dt.AsEnumerable()
                                   where row.Field<string>("IP").Contains(ip)
                                   select row["Building"].ToString()).ToList();

            var selectedRoom = (from row in dt.AsEnumerable()
                                   where row.Field<string>("IP").Contains(ip)
                                   select row["Room"].ToString()).ToList();

            var selectedIP = (from row in dt.AsEnumerable()
                              where row.Field<string>("IP").Contains(ip)
                              select row["IP"].ToString()).ToList();

            var selectedUDP = (from row in dt.AsEnumerable()
                              where row.Field<string>("IP").Contains(ip)
                              select row["UDP"]).ToList();

            var selectedLastChecked = (from row in dt.AsEnumerable()
                              where row.Field<string>("IP").Contains(ip)
                              select row["LastChecked"].ToString()).ToList();

            var selectedTag = (from row in dt.AsEnumerable()
                              where row.Field<string>("IP").Contains(ip)
                              select row["Tag"].ToString()).ToList();

            var selectedDesignation = (from row in dt.AsEnumerable()
                              where row.Field<string>("IP").Contains(ip)
                              select row["Designation"].ToString()).ToList();

            Building = selectedBldg.ElementAt(0);
            Room = selectedRoom.ElementAt(0);
            IP = selectedIP.ElementAt(0).ToString();
            UDP = Convert.ToBoolean(selectedUDP.ElementAt(0).ToString());
            LastChecked = selectedLastChecked.ElementAt(0);
            Tag = selectedTag.ElementAt(0);
            Designation = selectedDesignation.ElementAt(0);
        }

    }
}
