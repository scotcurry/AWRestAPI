using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestAPIApp
{
    public class AirWatchDevices
    {
        public string FriendlyName { get; set; }
        public int LocationGroupId { get; set; }
        public string Ownership { get; set; }
        public int PlatformId { get; set; }
        public int ModelId { get; set; }
        public int OperatingSystemId { get; set; }
        public string Udid { get; set; }
        public string SerialNumber { get; set; }
        public string AssetNumber { get; set; }
        public string MessageType { get; set; }
        public string SIM { get; set; }
        public string ToEmailAddress { get; set; }
    }
}
