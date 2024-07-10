using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.WiFiDirect;

namespace WifiDirectHost
{
    public class DeviceConnections
    {
        public string DisplayName { get; set; }

        public string Id { get; set; }
        
        public WiFiDirectDevice WfdDevice { get; set; }

    }
}
