/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.

using System.Threading.Tasks;

namespace GUCTechTools
{
    interface IUPD
    {
        string IPAddress { get; set; }
        bool IsUDPCompatible { get; set; }
        void SendCommand(string command, string value);
    }
}
