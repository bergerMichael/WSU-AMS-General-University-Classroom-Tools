/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace GUCTechTools.Classes
{
    class Projector : INetwork
    {
        public HttpWebRequest Request { get; set; }

        public ushort? ProjectorNumber;

        private string RequestString;

        public void Connect(string IP, int port, ushort? ProjNUmber)
        {
            RequestString = "http://" + IP + ":" + port.ToString() + "/?REQUEST=";
            ProjectorNumber = ProjNUmber;
        }

        public void Disconnect()
        {
            
        }

        public async Task<bool> ProjectorCommand(string command, string success)
        {
            Request = WebRequest.CreateHttp(new Uri(command));

            try
            {
                using (var responseAsync = await Request.GetResponseAsync())
                {
                    using (Stream dataStream = responseAsync.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string response = await reader.ReadToEndAsync();
                            if (response.Contains(success))
                                return true;
                        }
                    }
                }
            }
            catch (WebException)
            {
                return false;
            }

            return false;
        }

        

        public async Task<bool> PowerOn()
        {
            if ("" == RequestString || null == ProjectorNumber || 0 == ProjectorNumber)
                return false;

            string commandString = "";

            if (ProjectorNumber == 1)
                commandString = RequestString + "0";
            else if (ProjectorNumber == 2)
                commandString = RequestString + "0";

            return await ProjectorCommand(commandString, "");
        }

        public async Task<bool> PowerOff()
        {
            if ("" == RequestString || null == ProjectorNumber || 0 == ProjectorNumber)
                return false;

            string commandString = "";

            if (ProjectorNumber == 1)
                commandString = RequestString + "0";
            else if (ProjectorNumber == 2)
                commandString = RequestString + "0";

            return await ProjectorCommand(commandString, "");
        }

        public async Task<bool> PictureMute()
        {
            if ("" == RequestString || null == ProjectorNumber || 0 == ProjectorNumber)
                return false;

            string commandString = "";

            if (ProjectorNumber == 1)
                commandString = RequestString + "0";
            else if (ProjectorNumber == 2)
                commandString = RequestString + "0";

            return await ProjectorCommand(commandString, "");
        }

        public async Task<bool> PictureUnmute()
        {
            if ("" == RequestString || null == ProjectorNumber || 0 == ProjectorNumber)
                return false;

            string commandString = "";

            if (ProjectorNumber == 1)
                commandString = RequestString + "0";
            else if (ProjectorNumber == 2)
                commandString = RequestString + "0";

            return await ProjectorCommand(commandString, "");
        }

        public void Connect(string IP, int port)
        {
            throw new NotImplementedException();
        }
    }
}
