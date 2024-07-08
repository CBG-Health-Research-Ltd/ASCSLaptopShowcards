using System.Management;
using System.Security.Policy;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFiDirect;
using Windows.Networking.Sockets;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Windows.UI.Core;
using System.Collections.ObjectModel;
using Windows.Devices.HumanInterfaceDevice;
using System.Windows.Forms;

namespace WifiDirect
{
    public partial class WifiDirect : Form
    {

        private string _ssid = "IPSOS";
        private string _randomPassword = "rnd57890123";

        private WiFiDirectAdvertisementPublisher _publisher = null;
        private WiFiDirectConnectionListener _listener;
        private bool _connected = false;
        private List<ConnectedDevice> ConnectedDevices = new List<ConnectedDevice>();

        ConcurrentDictionary<StreamSocketListener, WiFiDirectDevice> _pendingConnections = new ConcurrentDictionary<StreamSocketListener, WiFiDirectDevice>();

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
                Notify("Listening For Connections....");
            }
            else
            {
                Notify($"WifiDirect failed to start. Status is {_publisher.Status}", true, true);
            }
        }
        private void btnStart_Click(object? sender, EventArgs e)
        {
            StartListening();


        }

        public void Notify(string message, bool showMessage = false, bool isError = false)
        {
            toolStripStatusLabel1.Text = message;
            if (showMessage)
            {
                if (isError)
                {
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                else
                {
                    MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
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
            _ssid = "IPSOS_" + System.Environment.MachineName;
          //  txtPassword.Text = _randomPassword;
          //  txtSSID.Text = _ssid;
            listConnectedDevices.DataSource = ConnectedDevices;
            btnStop.Enabled = true;
            Notify("Loaded Wifi Direct.");
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

            _publisher.Advertisement.ListenStateDiscoverability = discoverability;
            _publisher.Start();
        }

        private async void OnConnectionRequested(WiFiDirectConnectionListener sender, WiFiDirectConnectionRequestedEventArgs connectionEventArgs)
        {
            WiFiDirectConnectionRequest connectionRequest = connectionEventArgs.GetConnectionRequest();
            bool success = await HandleConnectionRequestAsync(connectionRequest);

            if (success)
            {


            }
            else
            {
                Notify($"Connection request from {connectionRequest.DeviceInformation.Name} failed", true, true);

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
                Notify("DeviceInformation.CreateFromIdAsync threw an exception: " + ex.Message, true, true);
            }

            if (devInfo == null)
            {
                Notify("Device Information is null");
                return false;
            }

            deviceSelector = $"System.Devices.Aep.DeviceAddress:=\"{devInfo.Properties["System.Devices.Aep.DeviceAddress"]}\"";
            DeviceInformationCollection pairedDeviceCollection = await DeviceInformation.FindAllAsync(deviceSelector, null, DeviceInformationKind.Device);
            return pairedDeviceCollection.Count > 0;
        }

        public void AddConnection(StreamSocketListener listenerSocket, WiFiDirectDevice wfdDevice)
        {
            _pendingConnections[listenerSocket] = wfdDevice;
        }
        

        private async Task<bool> HandleConnectionRequestAsync(WiFiDirectConnectionRequest connectionRequest)
        {
            string deviceName = connectionRequest.DeviceInformation.Name;
            string deviceId = connectionRequest.DeviceInformation.Id;

            Notify("Connecting to " + deviceName + "....");

            bool isPaired = (connectionRequest.DeviceInformation.Pairing?.IsPaired == true) ||
                            (await IsAepPairedAsync(deviceId));

            if (isPaired)
            {
                string message = $"Connection request received from {deviceName}. Do you want to continue ?";
                var result = MessageBox.Show(message, "ConnectionRequest", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return false;
                }

               
            }

            WiFiDirectDevice wfdDevice = null;
            try
            {
                // IMPORTANT: FromIdAsync needs to be called from the UI thread
                wfdDevice = await WiFiDirectDevice.FromIdAsync(connectionRequest.DeviceInformation.Id);
            }
            catch (Exception ex)
            {
                Notify($"Exception in FromIdAsync: {ex}", true, true);
                return false;
            }

            // Register for the ConnectionStatusChanged event handler
            wfdDevice.ConnectionStatusChanged += OnConnectionStatusChanged;


            var listenerSocket = new StreamSocketListener();

            // Save this (listenerSocket, wfdDevice) pair so we can hook it up when the socket connection is made.
           
            AddConnection(listenerSocket, wfdDevice);
            

            var EndpointPairs = wfdDevice.GetConnectionEndpointPairs();

             listenerSocket.ConnectionReceived += OnSocketConnectionReceived;


            try
            {
                await listenerSocket.BindServiceNameAsync(Globals.strServerPort);
            }
            catch (Exception ex)
            {
                Notify($"Connect operation threw an exception: {ex.Message}", true, true);
                return false;
            }

            Notify($"Devices connected on L2, listening on IP Address: {EndpointPairs[0].LocalHostName}" +
                                $" Port: {Globals.strServerPort}");

            ConnectedDevices.Add(new ConnectedDevice(deviceName ?? "(unnamed)", wfdDevice));


            return true;

        }


        private void OnSocketConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            var task = async () =>
            {
                Notify("Connecting to remote side on L4 layer...");
                StreamSocket serverSocket = args.Socket;

                // Look up the WiFiDirectDevice associated with this StreamSocketListener.
                WiFiDirectDevice wfdDevice;
                if (!_pendingConnections.TryRemove(sender, out wfdDevice))
                {
                    Notify("Unexpected connection ignored.");
                    serverSocket.Dispose();
                    return;
                }

                SocketReaderWriter socketRW = new SocketReaderWriter(serverSocket, this);

                // The first message sent is the name of the connection.
                string message = await socketRW.ReadMessageAsync();

                // Add this connection to the list of active connections.



                while (message != null)
                {
                    message = await socketRW.ReadMessageAsync();
                }
            };
        }

        private void OnConnectionStatusChanged(WiFiDirectDevice sender, object arg)
        {
            Notify($"Connection status changed: {sender.ConnectionStatus}");

            if (sender.ConnectionStatus == WiFiDirectConnectionStatus.Disconnected)
            {
                // TODO: Should we remove this connection from the list?
                // (Yes, probably.)
            }
        }

        void OnStatusChanged(WiFiDirectAdvertisementPublisher sender, WiFiDirectAdvertisementPublisherStatusChangedEventArgs statusEventArgs)
        {
            // *** 1 ***
            Notify(string.Format("{0}", statusEventArgs.Status.ToString()));

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

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
