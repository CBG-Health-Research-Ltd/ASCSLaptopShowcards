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
using System.Net.Sockets;
using Windows.Devices.HumanInterfaceDevice;
using System.Windows.Forms;
using ABI.Windows.Devices.WiFiDirect.Services;
using WifiDirectHost;
using System.Net;
using System.Diagnostics;
using Windows.Devices.WiFiDirect.Services;

namespace WifiDirect
{
    public partial class WifiDirect : Form
    {

        private string _ssid = "IPSOS";
        private string _randomPassword = "rnd57890123";

        private WiFiDirectAdvertisementPublisher _publisher = null;
        private WiFiDirectConnectionListener _listener;
        private bool _connected = false;

        private List<DeviceConnections> ConnectedListNames = new List<DeviceConnections>();
        private ConcurrentDictionary<string, CommunicationServer> CommunicationServers = new ConcurrentDictionary<string, CommunicationServer>();

        private TcpListener _server = null;

       
        private Thread _thread1 = null;
        private Thread _thread2 = null;
        public ShowCardManager CardManager = null;


        public WifiDirect()
        {
            CloseFirstInstance();
            CardManager = new ShowCardManager();
            InitializeComponent();
            CardManager.Initialize(this);



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
                Notify(txtMessage.Text);

                CardManager.ReadStream(message);
                // pass to ShowCardManager


            }));




        }
        public void NotifySentMessage(string message)
        {
            this.Invoke((System.Action)(() =>
            {
                string currentDate = DateTime.Now.ToString("u");
                txtMessage.Text += " SENT : [" + currentDate + "] - " + message + "\n";
                Notify(txtMessage.Text);
            }));


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



        }

        private void WifiDirect_Load(object sender, EventArgs e)
        {
            pollTextTimer.Enabled = true;

            _server = new TcpListener(IPAddress.Parse("0.0.0.0"), Convert.ToInt32(Globals.strServerPort));
            _server.Start();


            this.TopMost = false;

            Notify($"Listening to 0.0.0.0, Port {Globals.strServerPort}...");

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
            qrCodeControl1.TextData = string.Format(Globals.QrCodeData, _ssid, _randomPassword);
            btnStop.Enabled = true;
            Notify("Loaded Wifi Direct.");
            StartListening();


        }

        private void CloseFirstInstance()
        {
            Process[] pname = Process.GetProcessesByName(AppDomain.CurrentDomain.FriendlyName.Remove(AppDomain.CurrentDomain.FriendlyName.Length - 4));
            if (pname.Length > 1)
            {
                pname[1].Kill();
            }
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

        public void AddConnection(WiFiDirectDevice wfdDevice, string deviceName)
        {


            DeviceConnections deviceConnections = new DeviceConnections();

            deviceConnections.Id = wfdDevice.DeviceId;
            deviceConnections.DisplayName = deviceName;
            deviceConnections.WfdDevice = wfdDevice;

            ConnectedListNames.Add(deviceConnections);


            this.Invoke((System.Action)(() =>
            {

                listConnectedDevices.Refresh();
                listConnectedDevices.Update();
                bindingSource1.ResetBindings(false);

            }));


        }


        private void TcpThread(string deviceId, TcpListener tcpListener)
        {
            while (!Globals.AppCancellationTokenSource.IsCancellationRequested)
            {

                CommunicationServer communicationServer = new CommunicationServer(tcpListener.AcceptTcpClient(), this, CardManager);
                _thread2 = new Thread(new ThreadStart(communicationServer.ReadFromClient));
                _thread2.Start();
                CommunicationServers[deviceId] = communicationServer;
            }
        }

        private async Task<bool> HandleConnectionRequestAsync(WiFiDirectConnectionRequest connectionRequest)
        {
            string deviceName = connectionRequest.DeviceInformation.Name;
            string deviceId = connectionRequest.DeviceInformation.Id;

            Notify("Connecting to " + deviceName + "....");

            bool isPaired = (connectionRequest.DeviceInformation.Pairing?.IsPaired == true) ||
                            (await IsAepPairedAsync(deviceId));


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


            if (wfdDevice == null)
            {
                Notify($"Connection to {deviceId} failed", true);
                return false;
            }

            // Register for the ConnectionStatusChanged event handler
            wfdDevice.ConnectionStatusChanged += OnConnectionStatusChanged;



            var EndpointPairs = wfdDevice.GetConnectionEndpointPairs();




            Notify($"Device connected : {deviceName}. listening on IP Address:{EndpointPairs[0].LocalHostName} " +
                                $" Port: {Globals.strServerPort}");
            Notify($"Ready to receive socket connection at {EndpointPairs[0].LocalHostName}  Port: {Globals.strServerPort}...");

            AddConnection(wfdDevice, deviceName);

            _thread1 = new Thread(() => TcpThread(wfdDevice.DeviceId, _server));
            _thread1.Start();

            return true;

        }




        private void OnConnectionStatusChanged(WiFiDirectDevice sender, object arg)
        {
            Notify($"Connection status changed: {sender.ConnectionStatus}");
            CommunicationServers[sender.DeviceId].CloseConnection();
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



                dcDevice.WfdDevice.Dispose();

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


            CommunicationServers[connectedDevice.Id].CloseConnection();

            connectedDevice.WfdDevice.Dispose();
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

        public void SendMessage(string message)
        {
            try
            {
                foreach (var device in CommunicationServers.Values)
                {
                    device.WriteToClient(message);
                }
                NotifySentMessage(message);
            }
            catch (Exception exp)
            {

            }
        }


        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                var connectedDevice = (DeviceConnections)listConnectedDevices.SelectedItem;
                if (connectedDevice == null) return;

                var commServer = CommunicationServers[connectedDevice.Id];
                commServer.WriteToClient(txtSendData.Text);
                NotifySentMessage(txtSendData.Text);
            }
            catch (Exception exp)
            {

            }

        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
            txtMessage.SelectionStart = txtMessage.Text.Length;

            txtMessage.ScrollToCaret();
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Log File|*.log";
            saveFileDialog1.Title = "Save a Text File";
            saveFileDialog1.FileName = System.Environment.MachineName + "_" + DateTime.Now.ToString("yyyyMMdd_hhss") + ".log";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {

                this.txtLogs.SaveFile(saveFileDialog1.FileName, RichTextBoxStreamType.PlainText);
            }
        }

        private void WifiDirect_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Globals.AppCancellationTokenSource.Cancel();
                try
                {
                    _thread1.Interrupt();
                }

                catch (Exception ex)
                {

                }

                try
                {
                    _thread2.Interrupt();
                }
                catch (Exception ex)
                {

                }

                try
                {
                    _server.Stop();
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Application.Exit();
            }
        }

        private void WifiDirect_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Globals.AppCancellationTokenSource.Cancel();
                try
                {
                    _thread1.Interrupt();
                }

                catch (Exception ex)
                {

                }

                try
                {
                    _thread2.Interrupt();
                }
                catch (Exception ex)
                {

                }

                try
                {
                    _server.Stop();
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Application.Exit();
            }
        }

        private void pollTextTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                pollTextTimer.Enabled = false;
                CardManager.PollTxtFile();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                pollTextTimer.Enabled = true;
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Help.ShowHelp(this, "file://C:\\LaptopShowcards\\HelpFile.chm",
                HelpNavigator.Topic, "About.htm");
        }
    }
}
