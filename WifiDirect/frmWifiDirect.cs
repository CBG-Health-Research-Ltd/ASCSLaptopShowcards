using System.Management;
using System.Security.Policy;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFiDirect;
using Windows.Networking.Sockets;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using System.Threading.Tasks;

namespace WifiDirect
{
    public partial class WifiDirect : Form
    {

        private string _ssid = "IPSOS";
        private string _randomPassword = "rnd57890123";

        private WiFiDirectAdvertisementPublisher _publisher = null;
        private WiFiDirectConnectionListener _listener;
        private bool _connected = false;
        public WifiDirect()
        {
            InitializeComponent();
        }


        private void StartListening()
        {
            StartAdvertisement(WiFiDirectAdvertisementListenStateDiscoverability.Normal);

            if (_publisher.Status == WiFiDirectAdvertisementPublisherStatus.Started)
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                toolStripStatusLabel1.Text = "Listening For Connections....";
            }
            else
            {
                toolStripStatusLabel1.Text = $"WifiDirect failed to start. Status is {_publisher.Status}";
            }
        }
        private void btnStart_Click(object? sender, EventArgs e)
        {
            StartListening();
      
          
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
           StopAdvertisement();
           btnStop.Enabled = false;
           btnStart.Enabled = true;

        }

        private void WifiDirect_Load(object sender, EventArgs e)
        {
            _randomPassword = GetMACAddress().ToLower().Replace(":", "");
            _ssid ="IPSOS_"+ System.Environment.MachineName;
            txtPassword.Text = _randomPassword;
            txtSSID.Text = _ssid;
            toolStripStatusLabel1.Text = "Loaded Wifi Direct.";
            StartListening();
        }



        void StartAdvertisement(WiFiDirectAdvertisementListenStateDiscoverability discoverability)
        {
            if (_publisher == null)
            {
                _publisher = new WiFiDirectAdvertisementPublisher();
            }
            _listener = new WiFiDirectConnectionListener();

            _listener.ConnectionRequested += OnConnectionRequested;

            _publisher.StatusChanged += OnStatusChanged;
            _publisher.Advertisement.IsAutonomousGroupOwnerEnabled = true;
            _publisher.Advertisement.LegacySettings.IsEnabled = true;
            _publisher.Advertisement.LegacySettings.Ssid = txtSSID.Text;

            PasswordCredential lCred = new PasswordCredential();
            lCred.Password = txtPassword.Text;
            _publisher.Advertisement.LegacySettings.Passphrase = lCred;

            _publisher.Advertisement.ListenStateDiscoverability = discoverability;
            _publisher.Start();
        }

        private async void OnConnectionRequested(WiFiDirectConnectionListener sender, WiFiDirectConnectionRequestedEventArgs connectionEventArgs)
        {
            WiFiDirectConnectionRequest connectionRequest = connectionEventArgs.GetConnectionRequest();
            bool success = await  HandleConnectionRequestAsync(connectionRequest);

            if (success)
            {
                
               
            }
            else
            {
                MessageBox.Show($"Connection request from {connectionRequest.DeviceInformation.Name} failed",
                    "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = string.Format($"Connection request from {connectionRequest.DeviceInformation.Name} failed");
                
                connectionRequest.Dispose();
            }
        }

        private async Task<bool> IsAepPairedAsync(string deviceId)
        {
            List<string> additionalProperties = new List<string>();
            additionalProperties.Add("System.Devices.Aep.DeviceAddress");
            String deviceSelector = $"System.Devices.Aep.AepId:=\"{deviceId}\"";
            DeviceInformation devInfo = null;

            try
            {
                devInfo = await DeviceInformation.CreateFromIdAsync(deviceId, additionalProperties);
            }
            catch (Exception ex)
            {
               toolStripStatusLabel1.Text="DeviceInformation.CreateFromIdAsync threw an exception: " + ex.Message;
            }

            if (devInfo == null)
            {
                toolStripStatusLabel1.Text = "Device Information is null";
                return false;
            }

            deviceSelector = $"System.Devices.Aep.DeviceAddress:=\"{devInfo.Properties["System.Devices.Aep.DeviceAddress"]}\"";
            DeviceInformationCollection pairedDeviceCollection = await DeviceInformation.FindAllAsync(deviceSelector, null, DeviceInformationKind.Device);
            return pairedDeviceCollection.Count > 0;
        }

        private async Task<bool> HandleConnectionRequestAsync(WiFiDirectConnectionRequest connectionRequest)
        {
            string deviceName = connectionRequest.DeviceInformation.Name;
            string deviceId=connectionRequest.DeviceInformation.Id;

            toolStripStatusLabel1.Text = "Connecting to " + deviceName + "....";

            bool isPaired = (connectionRequest.DeviceInformation.Pairing?.IsPaired == true) ||
                            (await IsAepPairedAsync(deviceId));

            // Show the prompt only in case of WiFiDirect reconnection or Legacy client connection.
            return isPaired;
            
        }

        
        void OnStatusChanged(WiFiDirectAdvertisementPublisher sender, WiFiDirectAdvertisementPublisherStatusChangedEventArgs statusEventArgs)
        {
            // *** 1 ***
            toolStripStatusLabel1.Text=string.Format("{0}", statusEventArgs.Status.ToString());
            
        }

        void StopAdvertisement()
        {

            _publisher.Stop();
            _publisher.StatusChanged -= OnStatusChanged;

        }

        public string GetMACAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();

            string MACAddress = String.Empty;

            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty)
                { // only return MAC Address from first card
                    if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }

            return MACAddress;
        }
        
        


    }
}
