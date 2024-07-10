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
using ABI.Windows.Devices.WiFiDirect.Services;
using WifiDirectHost;

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

        private List<DeviceConnections> ConnectedListNames = new List<DeviceConnections>();

        ConcurrentDictionary<StreamSocketListener, WiFiDirectDevice> _connections = new ConcurrentDictionary<StreamSocketListener, WiFiDirectDevice>();

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
                btnDisconnect.Enabled = true;
                txtSSID.ReadOnly = true;
                txtPassword.ReadOnly = true;

                Notify("Listening For Connections....");
            }
            else
            {
                Notify($"WifiDirect failed to start. Status is {_publisher.Status}", true);
            }
        }
        
        private void btnStart_Click(object? sender, EventArgs e)
        {
            StartListening();


        }


        public void NotifyReceiveMessage(string message)
        {
            this.Invoke((System.Action)(() =>
            {
                string currentDate = DateTime.Now.ToString("u");
                txtMessage.Text += " RECEIVED : [" + currentDate + "] - " + message + "\n";

            }));
            Notify( message);

        }
        public void NotifySentMessage(string message)
        {
            this.Invoke((System.Action)(() =>
            {
                string currentDate = DateTime.Now.ToString("u");
                txtMessage.Text += " SENT : [" + currentDate + "] - " + message + "\n";

            }));
            Notify(message);

        }

        public void Notify(string message, bool isError = false)
        {
            this.Invoke((System.Action)(() =>
            {
                toolStripStatusLabel1.Text = message;
                if (isError)
                {
                    string currentDate = DateTime.Now.ToString("u");
                    txtLogs.Text += "[" + currentDate + "] - " + message + "\n";

                }
                else
                {
                    txtLogs.ForeColor = Color.Black;
                    string currentDate = DateTime.Now.ToString("u");
                    txtLogs.Text += "[" + currentDate + "] - " + message + "\n";
                }



            }));


        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            StopAdvertisement();
            btnStop.Enabled = false;
            btnStart.Enabled = true;
            btnDisconnect.Enabled = false;
            txtSSID.ReadOnly = false;
            txtPassword.ReadOnly = false;

            ConnectedDevices.Clear();


        }

        private void WifiDirect_Load(object sender, EventArgs e)
        {
            _randomPassword = GetMACAddress().ToLower().Replace(":", "");
            _ssid = "IPSOS_" + System.Environment.MachineName;
            txtPassword.Text = _randomPassword;
            txtSSID.Text = _ssid;
            bindingSource1.DataSource = ConnectedListNames;
            listConnectedDevices.SelectionMode = SelectionMode.One;
            listConnectedDevices.DataSource = bindingSource1;
            listConnectedDevices.DisplayMember = "DisplayName";
            listConnectedDevices.ValueMember = "DisplayName";
            bindingSource1.ResetBindings(false);

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

            _publisher.Advertisement.LegacySettings.IsEnabled = true;
            _publisher.Advertisement.IsAutonomousGroupOwnerEnabled = true;

            if (!String.IsNullOrEmpty(txtPassword.Text))
            {
                var creds = new Windows.Security.Credentials.PasswordCredential();
                creds.Password = txtPassword.Text;
                _publisher.Advertisement.LegacySettings.Passphrase = creds;
            }

            if (!String.IsNullOrEmpty(txtSSID.Text))
            {
                _publisher.Advertisement.LegacySettings.Ssid = txtSSID.Text;
            }


            _publisher.Advertisement.ListenStateDiscoverability = discoverability;
            _publisher.Start();
        }

        private async void OnConnectionRequested(WiFiDirectConnectionListener sender, WiFiDirectConnectionRequestedEventArgs connectionEventArgs)
        {
            WiFiDirectConnectionRequest connectionRequest = connectionEventArgs.GetConnectionRequest();
            bool success = await HandleConnectionRequestAsync(connectionRequest);

            if (!success)
            {

                Notify($"Connection request from {connectionRequest.DeviceInformation.Name} failed", true);

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
                Notify("DeviceInformation.CreateFromIdAsync threw an exception: " + ex.Message, true);
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

        public void AddConnection(StreamSocketListener listenerSocket, WiFiDirectDevice wfdDevice, string deviceName)
        {


            DeviceConnections deviceConnections =new DeviceConnections();

            deviceConnections.Id = wfdDevice.DeviceId;
            deviceConnections.DisplayName = deviceName;
            
            ConnectedListNames.Add(deviceConnections);

            this.Invoke((System.Action)(() =>
            {

                listConnectedDevices.Refresh();
                listConnectedDevices.Update();
                bindingSource1.ResetBindings(false);

            }));


        }


        private async Task<bool> HandleConnectionRequestAsync(WiFiDirectConnectionRequest connectionRequest)
        {
            string deviceName = connectionRequest.DeviceInformation.Name;
            string deviceId = connectionRequest.DeviceInformation.Id;

            Notify("Connecting to " + deviceName + "....");

            bool isPaired = (connectionRequest.DeviceInformation.Pairing?.IsPaired == true) ||
                            (await IsAepPairedAsync(deviceId));

            /*
            if (isPaired || _publisher.Advertisement.LegacySettings.IsEnabled)
            {
                string message = $"Connection request received from {deviceName}. Do you want to continue ?";
                var result = MessageBox.Show(message, "ConnectionRequest", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                if (result == DialogResult.No)
                {
                    return false;
                }


            }
            */

            WiFiDirectDevice wfdDevice = null;
            try
            {
                // IMPORTANT: FromIdAsync needs to be called from the UI thread
                wfdDevice = await WiFiDirectDevice.FromIdAsync(connectionRequest.DeviceInformation.Id);
            }
            catch (Exception ex)
            {
                Notify($"Exception in FromIdAsync: {ex.Message},{ex.StackTrace}", true);
                return false;
            }

            // Register for the ConnectionStatusChanged event handler
            wfdDevice.ConnectionStatusChanged += OnConnectionStatusChanged;

            var listenerSocket = new StreamSocketListener();

            // Save this (listenerSocket, wfdDevice) pair so we can hook it up when the socket connection is made.

            _connections[listenerSocket] = wfdDevice;


            var EndpointPairs = wfdDevice.GetConnectionEndpointPairs();

            listenerSocket.ConnectionReceived += OnSocketConnectionReceived;


            try
            {
                await listenerSocket.BindEndpointAsync(EndpointPairs[0].LocalHostName, Globals.strServerPort);
            }
            catch (Exception ex)
            {
                Notify($"Connect operation threw an exception: {ex.Message}", true);
                return false;
            }

            Notify($"Device connected : {deviceName}. listening on IP Address: {EndpointPairs[0].LocalHostName}" +
                                $" Port: {Globals.strServerPort}");
            Notify("Ready to send and receive message...");

            AddConnection(listenerSocket, wfdDevice, deviceName);

            return true;

        }


        private async void OnSocketConnectionReceived(StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {

            StreamSocket serverSocket = args.Socket;

            // Look up the WiFiDirectDevice associated with this StreamSocketListener.
            WiFiDirectDevice wfdDevice=_connections[sender];
            
/*            if (!_connections.TryRemove(sender, out wfdDevice))
            {
                Notify("Unexpected connection ignored.");
                serverSocket.Dispose();
                return;
            }
*/
            SocketReaderWriter socketRW = new SocketReaderWriter(serverSocket, this);



            // The first message sent is the name of the connection.
            string message = await socketRW.ReadMessageAsync();


            ConnectedDevices.Add(new ConnectedDevice(message ?? "(unnamed)", wfdDevice, socketRW));

            // Add this connection to the list of active connections.



            while (message != null)
            {
                message = await socketRW.ReadMessageAsync();
            }

        }

        private void OnConnectionStatusChanged(WiFiDirectDevice sender, object arg)
        {
            Notify($"Connection status changed: {sender.ConnectionStatus}");
            ConnectedDevice forDisconnection = null;
            if (sender.ConnectionStatus == WiFiDirectConnectionStatus.Disconnected)
            {

                DeviceConnections dcDevice = new DeviceConnections();
                foreach (var device in ConnectedListNames)
                {
                    if (device.Id == sender.DeviceId)
                    {
                        dcDevice = device;
                    }
                }

                foreach (var device in _connections)
                {
                    if (device.Value.DeviceId == dcDevice.Id)
                    {
                        _connections.TryRemove(device.Key, out var t);
                        t.Dispose();
                    }
                }

                foreach (var device in ConnectedDevices)
                {
                    if (device.WfdDevice.DeviceId == dcDevice.Id)
                    {
                        ConnectedDevices.Remove(device);
                        device.Dispose();
                    }
                }



                ConnectedListNames.Remove(dcDevice);
                
                
            }

            this.Invoke((System.Action)(() =>
            {

                listConnectedDevices.Refresh();
                listConnectedDevices.Update();
                bindingSource1.ResetBindings(false);

            }));
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



        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            
            var connectedDevice = (DeviceConnections)listConnectedDevices.SelectedItem;
            if (connectedDevice == null) return;

            foreach (var device in ConnectedDevices)
            {
                if (device.WfdDevice.DeviceId == connectedDevice.Id)
                {
                    ConnectedDevices.Remove(device);
                    device.Dispose();
                }
            }

            foreach (var device in _connections)
            {
                if (device.Value.DeviceId == connectedDevice.Id)
                {
                    _connections.TryRemove(device.Key, out var t);
                    t.Dispose();
                }
            }

            ConnectedListNames.Remove(connectedDevice);

            this.Invoke((System.Action)(() =>
            {

                listConnectedDevices.Refresh();
                listConnectedDevices.Update();
                bindingSource1.ResetBindings(false);

            }));

        }

        private void txtLogs_TextChanged(object sender, EventArgs e)
        {
            txtLogs.SelectionStart = txtLogs.Text.Length;

            txtLogs.ScrollToCaret();
        }

        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            var connectedDevice = (DeviceConnections)listConnectedDevices.SelectedItem;
            if (connectedDevice == null) return;

            foreach (var device in ConnectedDevices)
            {
                if (device.WfdDevice.DeviceId == connectedDevice.Id)
                {
                    await device.SocketRW.WriteMessageAsync(txtSendData.Text);
                }
            }

         
  
        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
            txtMessage.SelectionStart = txtMessage.Text.Length;

            txtMessage.ScrollToCaret();
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {

        }
    }
}
