using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarrenLee.Bluetooth;
using System.IO;
using System.Diagnostics;
using InTheHand;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;
using System.Threading;
using System.Net.Sockets;
using NAudio.Wave;
using System.Media;
using NAudio;
using System.Net;
using System.Runtime.InteropServices;

namespace BluetoothTestClient
{
    public partial class Laptop_BT_link : Form
    {
        //Abundance of global variables is due to multi-threading nature of program.
        //Update new proj/year area
        public static List<string> deviceItems;
        Guid mUUID;
        DeviceSelection devSelect;
        public static string currentDeviceName;
        Stream stream;
        string[] subStrings;
        List<string[]> adultY14ShowcardList;
        List<string[]> childY14ShowcardList;
        List<string[]> nzcvsy8ShowcardList;
        List<string[]> adultY15ShowcardList;
        List<string[]> childY15ShowcardList;


        string desiredShowcard;
        bool recording;
        string latestFile;
        string record;//Used as a global to determine if the voice recording is still hapening in secret.
        string savedDevice;
        bool newDevice;
        string mostRecentDevice = null;
        bool userInputting = false; //Determines if the current question demands user input.
        bool successfulConnection;
        

        //Global variables to be updated on separate selection form.
        public static string selectedDevice; //The device selected from opened select device form.
        public static int selectIndex; //The index of this selected device so it may be relayed to the device list constructed for this form.
        public static bool deviceIsSelected = false;//Helps for instances of no device-selection from device form.

        public Laptop_BT_link()
        {
  
            Array.ForEach(Directory.GetFiles(@"C:\nzhs\questioninformation\QuestionLog\"), File.Delete);//Clears question log contents
            deviceItems = new List<string>();
            InitializeComponent();
            closeFirstInstance();
            mostRecentDevice = null;
            receiveShowcardLists();
            waveSource = new WaveInEvent();//Needs to be initialised for event firing inr ecording function.
            recording = false; //needs to be initialised orignially to false for correct functionality.
            currentDeviceName = null; //Used to avoid multisocket exceptions. More detail in BeginConnect().
            mUUID = new Guid("8a63d9e7-ab03-4fd1-b835-9fa143b02c10");//Used on both laptop and tablet for specified conenction.
            showInitialText();
            startScan();        
            //this.TopMost = true;

        }
        
        //Update new proj/year area
        #region initialisation
        private void showInitialText()
        {
            txtStatus.Text = "Not doing anything..";
            sentMessage.Text = "First make sure CBG Laptop and Tablet are paired." + Environment.NewLine + Environment.NewLine
                + "Open TabletShowcards on show-card display tablet, then click connect on the Laptop.";
            sentMessage.AppendText(Environment.NewLine + Environment.NewLine + "You will be notified here when to begin the survey.");
        }

        private void receiveShowcardLists()
        {
            //This method could be cleaned up in the future
            childY14ShowcardList = GetShowcardPageList("CHILDY14");
            adultY14ShowcardList = GetShowcardPageList("ADULTY14");
            nzcvsy8ShowcardList = GetShowcardPageList("NZCVSY8");
            childY15ShowcardList = GetShowcardPageList("CHILDY15");
            adultY15ShowcardList = GetShowcardPageList("ADULTY15");
        }

        private void closeFirstInstance()
        {
            Process[] pname = Process.GetProcessesByName(AppDomain.CurrentDomain.FriendlyName.Remove(AppDomain.CurrentDomain.FriendlyName.Length - 4));
            if (pname.Length > 1)
            {
                pname[1].Kill();
            }
        }

        #endregion

        #region controls/UI

        private void button_connect_Click(object sender, EventArgs e)
        {
            startScan();
        }

        private void enableBluetooth()
        {

        }

        //Updates the "message to surveyor" area on form - this is generally transmitted to tablet as well if connection available.
        private void updateSent(string message)
        {
            Func<int> del = delegate ()
            {
                sentMessage.Text = (System.Environment.NewLine + message + System.Environment.NewLine);
                return 0;
            }; Invoke(del);
        }

        //Updates the status area on form
        private void updateStatus(string text)
        {
            Func<int> del = delegate ()
            {
                txtStatus.Text = (text + System.Environment.NewLine);
                return 0;
            }; Invoke(del);
        }


        private void EnableConnect()
        {
            Func<int> del = delegate ()
            { button_connect.Enabled = true; return 0; }; Invoke(del);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            System.IO.DirectoryInfo di = new DirectoryInfo(@"C:\CBGshared\currentdevice");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            MessageBox.Show(new Form { TopMost = true }, "Previously connected devices cleared." + Environment.NewLine +
                                                          Environment.NewLine + "You may connect your new device.");

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Help.ShowHelp(this, "file://C:\\LaptopShowcards\\HelpFile.chm",
                           HelpNavigator.Topic, "About.htm");
        }

        #endregion

        #region scan and select device scenarios
        private void startScan()
        {
            button_connect.Enabled = false;//Prevents muliple connection thread instances and therefore app crashing.
            deviceItems.Clear();
            Thread bluetoothScanThread = new Thread(new ThreadStart(scanDevices));//Operation must run on separate thread.
            bluetoothScanThread.Start();
        }

        BluetoothDeviceInfo[] devices; //Only store SCTAB devices in here eventually. Others -> null.
        private void scanDevices()
        {
            //Scan all devices then set devices to only allow for CBG naming convention.
            updateStatus("Scanning for Bluetooth devices..");
            updateSent("Now scanning for show-card display devices...");
            //New chrome window manager to ensure minimisation for prompts
            WindowControl chrome = new WindowControl();
            chrome.AppName = "chrome";

            int i = 0;
            try //Only works if bluetooth enabled, user is told to turn on bluetooth if doesn't work.
            {
                i = 0;
                BluetoothClient bluClient = new BluetoothClient();
                devices = bluClient.DiscoverDevicesInRange();
                foreach (BluetoothDeviceInfo d in devices)
                {
                    if (d.DeviceName.Contains("SCTAB") | d.DeviceName.Contains("DESKTOP-"))
                    {
                        //Add wanted initial devices to device array.
                        devices[i] = d;
                        i++; //final index wil be logged as +1 to the element where last specified valid
                            //address is
                    }
                }
                int j = i; //Keepp i in order to reference acceptable device indexes.
                while (j <  devices.Length)
                {
                    //This sets any invlaid device to null, it is accounted for when adding to deviceItems list, for device selection,
                    devices[j] = null;
                    j++;
                }
                
            }
            catch (Exception e)
            {
                chrome.Minimize();
                //User is asked to make sure bluetooth is connected. Or they can proceed without showcards. Chrome
                //and therefore survey will close automatically if they proceed to reconnect BT.
                if (MessageBox.Show(new Form { TopMost = true }, "Bluetooth is disabled. Please Enable Bluetooth in windows settings for electronic show-card use." + Environment.NewLine + Environment.NewLine
                     + "Clicking OK will close the survey. Please re-launch the survey once Bluetooth is activated." + Environment.NewLine + Environment.NewLine +
                     "If you wish to proceed without using electronic show-cards, click cancel.", "Bluetooth connection lost", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    CloseChrome();
                    Application.Exit();
                }
                else
                {
                    chrome.Restore();
                    Application.Exit();
                    
                }

            }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Using 'i' from non-null acceptable device index. So selection only lists SCTAB and test name..
            //Accounts for all wording of device selection possibilities.
            if (i == 1)
            {
               
                updateStatus("Scan Complete. " + i.ToString() + " SC device discovered."
                    + Environment.NewLine + "Please choose CBG tablet.");
            }
            else if (i == 0)
            {
                
                updateStatus("Scan Complete. " + i.ToString() + " SC devices discovered.");
            }
            else
            {
    
                updateStatus("Scan Complete. " + i.ToString() + " SC devices discovered."
                    + Environment.NewLine + "Please choose CBG tablet.");
            }

            foreach (BluetoothDeviceInfo d in devices)
            {
                //If device is not null then add it.try get a proper array first though. This allows the
                //list to only represent CBG devices.
                if (d != null)
                {
                    deviceItems.Add(d.DeviceName);
                }
            }
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //Below sets current device if it is equal to SCTAB or DESKTOP-.. DESKTOP- for testing purposes.
            if (Directory.GetFileSystemEntries(@"C:\CBGShared\currentdevice").Length != 0)
            {
                string tempDev = getLatest(@"C:\CBGShared\currentdevice");
                if (tempDev.Contains("SCTAB") | tempDev.Contains("DESKTOP-"))//Only search for SCTAB.. DESKTOP- for testing.
                    {
                    mostRecentDevice = getLatest(@"C:\CBGShared\currentdevice");
                    }
            }

            //if currentdevice exists in folder and matches a potential device in device selection then choose that device.
            if (mostRecentDevice != null && deviceItems.Contains(mostRecentDevice))//Checks to see if previously used device is available.
            {
                newDevice = false;
                Thread connectionThread = new Thread(new ThreadStart(BeginConnection));//Operation must run on separate thread.
                connectionThread.Start();
                savedDevice = mostRecentDevice;
                selectIndex = deviceItems.FindIndex(x => x.StartsWith(savedDevice));
            }
            else
            //Show the select devices dialog so user may select manually..
            {
                chrome.Minimize();
                try
                {
                    this.WindowState = FormWindowState.Normal;
                }
                catch { /*Exception thrown if chrome is already minimised.. this is foor testing purposes*/}
                
                newDevice = true;
                
                if (deviceItems.Count > 0)
                {
                    Thread bluetoothSelectThread = new Thread(new ThreadStart(selectDevices));//Operation must run on separate thread.
                    bluetoothSelectThread.Start();
                }
                else //Happens in the cases of BT not being switched on TABLET, or TabletShocards not running.
                {
                    updateStatus("Ensure device is switched on, discoverable, paired and in range, then re-launch survey.");
                    updateSent("No devices discovered. Please ensure your show-card display device has BlueTooth enabled.");
                    MessageBox.Show(new Form { TopMost = true }, "No Bluetooth Devices were discovered" + Environment.NewLine + Environment.NewLine +
                    "Make sure show-card display device is switched on, in range, bluetooth enabled and discoverable. Then re-launch survey.",
                    "No devices discovered", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Application.Exit();
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        private void selectDevices()
        {
            deviceIsSelected = false;
            selectedDevice = null; //ensures reset of device so it may be set by device selection form.
            devSelect = new DeviceSelection(); //Creates device selection form class
            devSelect.ShowDialog(); //Opens form to select device. This form continues upon closing event.
            if (deviceIsSelected == true) //This global is determined by device selection form.
            {
                Thread bluetoothSelectThread = new Thread(new ThreadStart(BeginConnection));//Operation must run on separate thread.
                bluetoothSelectThread.Start();
            }
            else //deviceIsSelected is false because the selection menu was closed.
            {
                //No device was selected, close button was hit. Begin selection thread again.
                updateStatus("No device was selected.");
                updateSent("You did not select a device. Please click CONNECT to attempt the connection again.");
                
                Func<int> del = delegate ()//Has to run on a separate thread.
                {
                    this.TopMost = true;
                    button_connect.Enabled = true;
                    return 0;
                }; Invoke(del);
               
            }

        }

        #endregion

        #region initial connection + re-connect attempt connection (for cases of Bluetooth drop out for re-connection)

        BluetoothDeviceInfo deviceInfo;
        private void BeginConnection()
        {
            //if previous device could not be found in selection list
            //Gets the device that was accessed in the selection form.
            if (newDevice == true)
            {
                deviceInfo = devices.ElementAt(selectIndex); //Uses the same index given on selection form. (they are identical).
                updateStatus(deviceInfo.DeviceName + " was selected." + Environment.NewLine
                    + "Attempting Bluetooth connection...");
                savedDevice = deviceInfo.DeviceName;
                var myFile = File.Create(@"C:\CBGShared\currentdevice\" + savedDevice + ".txt");
                myFile.Close();
                updateSent("Attempting connection to SC device");
            }
            else //This is the case used if a previous device was found.
            {
                deviceInfo = devices.ElementAt(selectIndex);
                updateStatus("Previous device " + savedDevice + " was found." + Environment.NewLine
                + "Attempting to connect...");
                var myFile = File.Create(@"C:\CBGShared\currentdevice\" + savedDevice + ".txt");
                myFile.Close();
                updateSent("Attempting connection to previously used SC device");
            }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //All possibilities for connected and paired devices. Or non-paired user alerts.

            if (PairDevice() == true)
            {
                if (deviceInfo.DeviceName != currentDeviceName && newDevice == true)//Avoids adding multiple sockets for an already connected device.
                {
                    updateStatus("Device is paired." + Environment.NewLine + "Attempting to connect.");
                    Thread bluClientThread = new Thread(new ThreadStart(ClientConnectionThread));
                    bluClientThread.Start();
                }
                else if (deviceInfo.DeviceName != currentDeviceName && newDevice == false)
                {
                    updateStatus("Previous paired device found." + Environment.NewLine + "Attempting to connect...");
                    Thread bluClientThread = new Thread(new ThreadStart(ClientConnectionThread));
                    bluClientThread.Start();
                }
                else
                {
                    //Will only occurr when trying to connect to a device that is already connected.
                    Func<int> del = delegate ()//Seperate threads so invoked delegation is required.
                    {
                        txtStatus.Text = "Device is already connected." + Environment.NewLine + "Turn off current device if you wish to use a new one, or Clear device History.";
                        return 0;
                    }; Invoke(del);
                }
            }
            else
            {
                updateStatus("Device is not paired." + Environment.NewLine
                    + "Pair devices and try again.");
                    updateSent("Make sure previous device is off if you wish to change devices. Also Clear Device History."
                    + Environment.NewLine +Environment.NewLine + "If problem persists, try switching bluetooth off then on again on SC tablet");
            }

            EnableConnect();//Allows the user to re-scan for devices again by enabling connection buttion.

        }


        private void ClientConnectionThread()
        {
            BluetoothClient bluClient = new BluetoothClient();
            bluClient.BeginConnect(deviceInfo.DeviceAddress, mUUID, this.BluetoothClientConnectCallBack, bluClient);
        }


        private void BluetoothClientConnectCallBack(IAsyncResult ar)//This process establishes and confirms connection.
        {   

            //Asynchronous call back for connection scheme. This is the initial connection scheme. BT drop-outs
            //use the alternative connection scheme.

            BluetoothClient client = (BluetoothClient)ar.AsyncState;
            int retries = 0;
            successfulConnection = false;
            WindowControl chrome = new WindowControl();
            chrome.AppName = "chrome";

            //Tries to establish a connection to GUID end-point, i.e. tablet showcards. Retries an threadsleep
            //determines how long the time-out will be.
            while (!successfulConnection && (retries <= 1500))//W thread sleep, gives 15 second time-out.
            {
                try
                {
                    client.EndConnect(ar);
                    successfulConnection = true;                 
                }
                catch (Exception e)
                {
                    successfulConnection = false;
                    Thread.Sleep(10);
                    retries++;
                }
            }

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //Accounting for all situations connection success or failure.

            if (successfulConnection == false && deviceInfo.Connected == false)
            {
                chrome.Minimize();
                MessageBox.Show(new Form { TopMost = true }, "Connection has timed out." + Environment.NewLine + Environment.NewLine +
                    "Make sure BlueTooth is enabled on both devices, then re-launch survey."
                    + Environment.NewLine + Environment.NewLine + "If problem persists, contact IT support."
                     , "Connection time-out", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Application.Exit();
            }

            //Initialises the stream for BT transmission of show-card specific strings. This stream will
            //stay valid once the device is re-connected upon momentary BT drop-out as well.

            if (successfulConnection == true)
            {
                stream = client.GetStream(); //Global BT stream variable is set so the stream may be used in other functions.
                stream.ReadTimeout = 1000;

                if (deviceInfo.DeviceName != currentDeviceName && newDevice == true)//Avoids adding multiple sockets for an already connected device.
                {
                    MessageBox.Show(new Form { TopMost = true }, "SC Device Connection Succesful!" + Environment.NewLine + Environment.NewLine +
                  "Click OK to begin/continue with the survey."
                  + Environment.NewLine + Environment.NewLine + "If problem persists, contact IT support."
                   , "Connection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    chrome.Restore();
                }

                Func<int> del = delegate () //Must use delegate method to avoid cross-threading exception.
                {
                    txtStatus.Text = "Tablet now connected. Do not close this program or TabletShowcards until survey is complete.";
                    button_connect.Text = "Connected";
                    button_connect.Enabled = false;
                    sentMessage.Text = Environment.NewLine + "You may now begin the survey." + Environment.NewLine + Environment.NewLine
                                           + "The correct showcard and media will be displayed once you begin." + Environment.NewLine + Environment.NewLine
                                           + "Hand tablet to respondent once survey title page is displayed." +
                                            Environment.NewLine + Environment.NewLine + "Close this progam on laptop when finished.";
                    transmitText(sentMessage.Text, stream);
                    return 0;
                    
                }; Invoke(del);

                currentDeviceName = deviceInfo.DeviceName; //Stores device being used currently. (avoids multisocket exception).

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Polltxtfile() to continuously monitor show-cards. Uses stream global variable to avoid 
                //certain exceptions.

                PollTxtFile(/*stream*/);
                
                //contains the while true loop which continuously monitors question number written by pageturner.
                //note that stream is commented out. This is because it is better used as a global so it can be
                //reset for re-instantiated streams in case of bluetooth drop-outs.

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            }
        }

        //These are for cases when bluetooth has been momentarily lost.
        //Multi-attemps due to socket exception with laptop BT drop-out exclusively.
        private void RestartConnectionThread()
        {
            Thread.Sleep(2000);//HERE TO ALLOW TABLETSHOWCARDS ENOGUGH TIME TO BOOT
            BluetoothClient bluClient = new BluetoothClient();
            int retries = 0;
            bool connect = false;
            while (retries <= 1000 && connect == false)
            {
                try
                {
                    bluClient.BeginConnect(deviceInfo.DeviceAddress, mUUID, this.RestartClientConnectCallBack, bluClient);
                    connect = true;
                }
                catch
                {
                    connect = false;
                    retries++;
                    Thread.Sleep(17);
                }
            }
        }

        //This separate callback is used to avoid generating separate thread instances, due to the re-establishment
        //of Polltxtfile(). 
        private void RestartClientConnectCallBack(IAsyncResult ar)//This process establishes and confirms connection.
        {

            //BluetoothClient newClient = (BluetoothClient)ar.AsyncState;

            //Ensure that the old client isn't preserved. Compiler cleans this up. May be unnecessary.
            BluetoothClient newClient = new BluetoothClient();
            newClient = (BluetoothClient)ar.AsyncState;
            int retries = 0;
            successfulConnection = false;
           

            while (!successfulConnection && (retries <= 2000))//W thread sleep, gives 20 second time-out.
                //greater time-out period than the intial connection protocol.
            {
                try
                {
                    newClient.EndConnect(ar);
                    successfulConnection = true;
                }
                catch (Exception e)
                {
                    successfulConnection = false;
                    Thread.Sleep(10);
                    retries++;
                }
            }

            if (successfulConnection == true)
            {
                try
                {
                    stream = newClient.GetStream(); //Global BT stream variable is set so the stream may be used in other functions.
                    stream.ReadTimeout = 1000;
                }
                catch (Exception e)
                {
                    //Manual re-connection attempt causes a stream setting exception. Because the stream
                    //Remains the same, this avoids it. Due to previosuly saved device being connected
                    //by default.
                }
                finally
                {
                    //Finally is used for safety purposes and to account for other exceptions. Allows
                    //re-setting of ReadTimeout?
                }

                Func<int> del = delegate () //Must use delegate method to avoid cross-threading exception.
                {
                    txtStatus.Text = "Tablet now connected. Do not close this program or TabletShowcards until survey is complete.";
                    button_connect.Text = "Connected";
                    button_connect.Enabled = false;
                    sentMessage.Text = Environment.NewLine + "You may now continue the survey." + Environment.NewLine + Environment.NewLine
                                           + "The correct showcard and media will be displayed when you click NEXT in survey.";
                    transmitText(sentMessage.Text, stream);
                    //Stores device being used currently. (avoids multisocket exception).
                    currentDeviceName = deviceInfo.DeviceName;
                    return 0;
                }; Invoke(del);

                 
            }
        }


        //Just sets a default pin to avoid user input. Could be updated for UI in other apps.
        string trialPin = "1234";
        private bool PairDevice()
        {
            if (!deviceInfo.Authenticated)//if there are security problems betwen devices.
            {
                if (!BluetoothSecurity.PairRequest(deviceInfo.DeviceAddress, trialPin))//arbritary pin attempt.
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region polling questions to display appropriate showcard from QuestionLog folder
        //Function used for sending strings regarding SC information.
        private void transmitText(string text, Stream stream)
        {
            try
            {
                byte[] message = Encoding.ASCII.GetBytes(text);
                stream.Write(message, 0, message.Length);
            }
            catch
            {

            }
            finally
            {

            }
        }

        //Key part f app. Continuously monitoring information being sent from TSS and stored in QuestionLog folder.
        public void PollTxtFile(/*Stream stream*/)//Checks the most recent .txt file update in QuestionLog folder. Handles null exceptions.
        {

            string Username = Environment.UserName;
            bool newFile = false;
            bool changedFile = false;
            string pageNum;

            //New file watcher initialised to monitor the QuesitonLog folder and raise created/updated events.
            //This process is used along side page turner ex to retrieve the current survey question name as a .txt file.
            FileSystemWatcher fileWatcher = new FileSystemWatcher();
            fileWatcher.Path = @"C:\nzhs\questioninformation\QuestionLog\";
            fileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                        | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            fileWatcher.Filter = "*.txt";


            while (true)
            {
                System.Threading.Thread.Sleep(20);//Significantly lowers CPU usage
                MethodInvoker AwaitTextChange = delegate ()
               {

                   //Event handling for given filewatcher filters. Allows detection of file update/creation and
                   //modification detection of POST info sent by bluetooth link.
                   fileWatcher.EnableRaisingEvents = true;
                   fileWatcher.Created += delegate { newFile = true; };
                   fileWatcher.Changed += delegate { changedFile = true; };
                   CheckConnection(); 

                   if (newFile == true || changedFile == true)//Un-comment else for user-interactivity
                   {
                       //Below only occurrs on the event of a .txt file update or creation withing QuestionLog folder.
                       //Open a dictionary which contains the relevant bookmark for each question.

                       latestFile = getLatest(@"C:\nzhs\questioninformation\QuestionLog\");
                       pageNum = ObtainShowcard(latestFile);
                       sentMessage.Text = latestFile + " corresponds to: " + "Page number " + pageNum;
                       transmitText(("page" + pageNum), stream); //NOTE: reciever side hardcoded to handle 'question'
                       //format. This can be changed to make things more coherent if someone was to look at code.

                       System.Threading.Thread.Sleep(50); //Prevent any chance of firing multiple strings via bluetooth.

                       //Reset booleans so they may be deemed true upon a new update/creation i.e. loop->if iteration.
                       changedFile = false;
                       newFile = false;
                   }
                   else if (userInputting == true){ readStream(stream); }//reads incoming stream for user-input

               };

                try { this.Invoke(AwaitTextChange); }
                catch (ObjectDisposedException e) { } //Cannot perform form control if form is closed. This catches that exception.
            }

        }

        //Reads the stream that is sent by the tablet. This will be used to gain the simulated post-data acquired
        //by user-input on the tablet side. Tablet side will need to continuously poll for latest in the same manner
        //that question-log is polled.
        private void readStream(Stream stream)
        {
            try
            {
                byte[] received = new byte[1024];
                stream.Read(received, 0, received.Length); //Reads stream in it's entirety.
                string incoming = Encoding.ASCII.GetString(received);
                //Writes all incoming stream data to post.txt in surveyinstructions if any exists. While true then detects
                //this and fires a web-response in accordance to the post-data.
                File.WriteAllText(@"C:\CBGShared\postRetrieve\post.txt", incoming);
            }
            catch(Exception e) { }
        }

        private string getLatest(string directory)//Gets the name of the latest file created/updated in QuestionLog directory.
        {
            string Username = Environment.UserName;
            DirectoryInfo questionDirectory = new DirectoryInfo(directory);
            string latestFile = Path.GetFileNameWithoutExtension(FindLatestFile(questionDirectory).Name);
            return latestFile;

        }

        private static FileInfo FindLatestFile(DirectoryInfo directoryInfo)//Gets file info of latest file updated/created in directory.
        {
            if (directoryInfo == null || !directoryInfo.Exists)
                return null;

            FileInfo[] files = directoryInfo.GetFiles();
            DateTime lastWrite = DateTime.MinValue;
            TimeSpan lastWriteMiliseconds = lastWrite.TimeOfDay;
            FileInfo lastWrittenFile = null;

            foreach (FileInfo file in files)
            {
                if (file.LastWriteTime.TimeOfDay > lastWriteMiliseconds)
                {
                    lastWriteMiliseconds = file.LastWriteTime.TimeOfDay;
                    lastWrittenFile = file;
                }
            }
            return lastWrittenFile;
        }

        #endregion

        //Update new proj/year area
        #region showcard loading functions and retrieval functions. firstpageindex function obsolete for askia use, for tss it is used in ReturnDesiredshowcard()

        private string ObtainShowcard(string inputTxt)
        {

            //Obtain the question number and then find the corresponding showcard from look up.
            if (string.Equals(inputTxt.Substring(0, 8), "question", StringComparison.CurrentCultureIgnoreCase)) ; //Makesure question&QN& CHILD/ADULT
            {
                char Qsplitter = ' ';//Splitting at the space between e.g. "question14 CHILD/ADULT" (the raw survey args).
                string[] subStrings = inputTxt.Split(Qsplitter);
                string questionNum = subStrings[0].Substring(8); //Page number hardcoded to correspond to question number.
                string surveyInfo = subStrings[1];//Either CHILD or ADULT (determines which survey look up and PDF to use).
                return ReturnDesiredShowcard(surveyInfo, questionNum);
            }

            return "non question specific detail from TSS. Check pageturner.txt";

        }

        bool firstShowcardPresented = false;
        private string ReturnDesiredShowcard(string survey, string questionNum)  //Quite an ugly function but no other option really.
        {
            int i = 0;
            string surveyType = survey.ToLower();
            List<string[]> showcardList = getShowcardList(surveyType);
            int firstPageIndex = FirstPageIndex(surveyType);
                while (i < showcardList.Count)
                {
                if ((showcardList[i])[0] == questionNum)//Page num is first element i.e. 0 index of showcard list entries.
                {

                    //IMPORTANT: IF showcard-list[i] contains 'user-input', then desiredShowcard = user-input(ID). Tablet will have to
                    //open in full-screen chrome environment to gain user input. "user-input questionNum".
                    if (showcardList[i].Contains("user-input"))
                    { desiredShowcard = "user-input" + questionNum; userInputting = true; break;
                    }

                    desiredShowcard = (showcardList[i])[1];//Because desired showcard is found in 2nd index of list.
                    firstShowcardPresented = true;
                    break;
                }
                //Automatically returns to title page for the case that showcards haven't started being displayed.
                //EXTRMEMELY IMPORTANT: 20 is hardcoded for NZCHSY7 survey!!!
                else if (firstShowcardPresented == false)
                    {
                        desiredShowcard = "0"; //Because on tablet side, a +1 is added to 'desiredShowcard'. So actual display at "1"
                    }
                    else
                    {
                        desiredShowcard = "1";//Because on tablet side, a +1 is added to 'desiredShowcard'. "2" shown if non-existant showcard.
                    }
                    i++;
            }
            return (desiredShowcard + " " + survey);
            //return "Non specific survey detail, check pageturner.txt";
        }

        
        //Two functions below must be changed to accomodate new surveys, also the global variable list instantiation
        //must be updated so that showcard reference lists are generated upon application launch.
        private List<string[]> getShowcardList(string survey)
        {
            survey = survey.ToLower();
            List<string[]> showcardList = new List<string[]>();
            switch(survey)
            {
                case ("nhc14"):
                    showcardList = childY14ShowcardList;//UPDATE FOR ASKIA SURVEY
                    break;
                case ("nha14"):
                    showcardList = adultY14ShowcardList;//UPDATE FOR ASKIA SURVEY
                    break;
                case ("y8cvs"):
                    showcardList = nzcvsy8ShowcardList;
                    break;
                case ("nhc15"):
                    showcardList = childY15ShowcardList;//UPDATE FOR ASKIA SURVEY
                    break;
                case ("nha15"):
                    showcardList = adultY15ShowcardList;//UPDATE FOR ASKIA SURVEY
                    break;

            }
            return showcardList;
        }

        //Function returns the first page index of a given set of showcards. These all need to be hardcoded into the system.
        //This function is obsolete for askia driven surveys
        private int FirstPageIndex(string survey)
        {
            survey = survey.ToLower();
            int pageIndex = 0;
            switch (survey)
            {
                case ("nha14"):
                    pageIndex = 3;//UPDATE FOR ASKIA SURVEY first seen QID that has a showcard
                    break;
                case ("nhc14"):
                    pageIndex = 3;//UPDATE FOR ASKIA SURVEY
                    break;
                case ("y8cvs"):
                    pageIndex = 2;//UPDATE!!!!
                    break;
                case ("nha15"):
                    pageIndex = 3;//UPDATE FOR ASKIA SURVEY first seen QID that has a showcard
                    break;
                case ("nhc15"):
                    pageIndex = 3;//UPDATE FOR ASKIA SURVEY
                    break;
            }
            return pageIndex;
        }




        //Gets the list which associates question number to showcard i.e. page number. 
        //Currently uses the TSS&QN& -> ShowcardNum format delimited by a space in a .txt file. 
        //Can add more cells such as bookmark relationships, but obtainShowcard() indexing would also need to be changed
        //to access bookmark at correct index.
        //Try-catch-finally adresses any surveyinstructions.txt they may not be in the system.

        private List<string[]> GetShowcardPageList(string survey)//takes the type of survey CHILD or ADULT to dtetermine list to load.
        {
            string User = Environment.UserName;
            string[] ShowcardPageArray = new string[0];
            try
            {
                switch (survey)
                {
                    case ("CHILDY14"):
                        ShowcardPageArray = File.ReadAllLines(@"C:\CBGShared\surveyinstructions\NZHSY14ChildInstructions.txt", Encoding.Default);
                        break;
                    case ("ADULTY14"):
                        ShowcardPageArray = File.ReadAllLines(@"C:\CBGShared\surveyinstructions\NZHSY14AdultInstructions.txt", Encoding.Default);
                        break;
                    case ("NZCVSY8"):
                        ShowcardPageArray = File.ReadAllLines(@"C:\CBGShared\surveyinstructions\NZCVSY8Instructions.txt", Encoding.Default);
                        break;
                    case ("CHILDY15"):
                        ShowcardPageArray = File.ReadAllLines(@"C:\CBGShared\surveyinstructions\NZHSY15ChildInstructions.txt", Encoding.Default);
                        break;
                    case ("ADULTY15"):
                        ShowcardPageArray = File.ReadAllLines(@"C:\CBGShared\surveyinstructions\NZHSY15AdultInstructions.txt", Encoding.Default);
                        break;
                }
            }
            catch (Exception e)
            {
                //Missing some survey insructions files
                //Do nothing
            }
            finally
            {
                //Continue with app as expected. This allows code that was successfully executed to be stored
                //so that relevant show-cards will still be displayed.
            }
            

            List<string[]> shoPageList = new List<string[]>();
            char splitter = ' ';

            for (int i = 0; i < ShowcardPageArray.Length; i++)
            {
                //NOTE: Uses TSSQuestionNum -> RequiredShowcard rleationship. "&QN&\t&SN&"
                ShowcardPageArray[i] = ShowcardPageArray[i].Replace("\t", " ");//Replacing tab with space for ease of processing.
                //Could get rid of the above line if we have space delimited .txt file.
                subStrings = ShowcardPageArray[i].Split(splitter);//Forming array into sub strings so it may be added to list
                shoPageList.Add(subStrings);//Generating the pageNum -> ShoCard list.
            }
            return shoPageList;
        }

        #endregion

        #region showcard associated recording. currently obsolete kept for reference

        //record is passed through to this function to let application know to record in secret.
        //if a TSS question has a 'record' tag, then it will begin recording. It will stop recording if there is no record tag.
        //this record tag is to be found in the lok up table which contains TSS &QN& -> ShowCard page.
        //record tags are yet to be added, and will need to talk to Thomas about this adition.
        static WaveFileWriter waveFile;
        static WaveInEvent waveSource;

        private void RecordInSecret()
        {

            //If the previous question was recording, then this will stop it from recording and dispose the 
            //waveSource. waveSource is re initialised when told to being recording again.
            if (recording == true)
            {
                waveSource.StopRecording();              
                recording = false;
                waveFile.Dispose();
            }

            string userName = System.Environment.UserName;

            if (record == "record")
            {
                waveSource = new WaveInEvent();
                waveSource.WaveFormat = new WaveFormat(8000, 1);
                waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
                string fileName = latestFile.Replace(' ', '_') + "_" + DateTime.Now.ToString("h_mm_ss tt").Replace(' ', '_');
                                    
                string tempFile = (@"C:\RecordedQuestions\" + fileName + ".wav");
                waveFile = new WaveFileWriter(tempFile , waveSource.WaveFormat);
                waveSource.StartRecording();                
                recording = true;
            }

        }

        static void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            waveFile.WriteData(e.Buffer, 0, e.BytesRecorded);

        }

        private string ShouldYouRecord(string inputTxt) //This could be tidied up in the future.
        {
            //Obtain the question number and then find the corresponding showcard from look up.
            if (string.Equals(inputTxt.Substring(0, 8), "question", StringComparison.CurrentCultureIgnoreCase)); //Makesure question&QN& CHILD/ADULT
            {
                char Qsplitter = ' ';//Splitting at the space between e.g. "question14 CHILD/ADULT" (the raw survey args).
                string[] subStrings = inputTxt.Split(Qsplitter);
                string questionNum = subStrings[0].Substring(8); //Page number hardcoded to correspond to question number.
                string surveyInfo = subStrings[1];//Either CHILD or ADULT (determines which survey look up and PDF to use).
                //houseHoldID = subStrings[2];
                return SelectRecord(surveyInfo, questionNum);
            }
            return ("Error in instruction file");
         }


        private string SelectRecord(string surveyInfo, string questionNum)
        {
            string permission = ReadFirstLine(@"C:\AudioRecording\RecordPermission\recordpermission.txt");
            string record = null; //Default record setting. Rif record keyword does not exist then this wil be returned.

            //Ensures that file creation is within two hour limit and respondne has consented to recording.
            if (permission.Contains("true") && CheckTimeStamp(@"C:\AudioRecording\RecordPermission\recordpermission.txt", 2))
            {
                List<string[]> showcardList = getShowcardList(surveyInfo);
                int i = 0;
                while (i < showcardList.Count)
                {
                    if ((showcardList[i])[0] == questionNum)//Page num is first element i.e. 0 index of showcard list entries.
                    {
                        //Must ensure that length is greater than 2 in order to avoid index out of range exception.
                        //Any element in list with more than just QN->SPN will have greater than 2 length.
                        if ((showcardList[i].Length > 4) && showcardList[i][4] == "record")//Checks to see that record exists.

                        { record = (showcardList[i])[4]; }//Because record is found in 3rd column.
                        break;
                    }
                    i++;
                }
            }
            return record;
        }

        private string ReadFirstLine(string textfile)
        {
            string permission = File.ReadLines(textfile).First();
            return permission;
        }

        #endregion

        #region connection monitoring functions
        //This is run in while(true) of polltxtfile(), if a connection has failed, it runs the auto-
        //reconnection attempt. If this then fails it runs the ManualReconnectionAttempt.
        private void CheckConnection() //Force refreshes device info, then checks if display device still connected.
        {
           
            deviceInfo.Refresh();
            //WindowControl chrome = new WindowControl();
            //chrome.AppName = "chrome";
            if (deviceInfo.Connected == false | !checkClientConnection())
            {      
                //chrome.Minimize();

                //this.TopMost = true;
                updateStatus("BT lost. Attempting re-connection." + Environment.NewLine
                    + "May take up to 15 seconds.");
                updateSent("BlueTooth connection lost. Attempting auto re-connection." + Environment.NewLine
                + "This may take up to 15 seconds." + Environment.NewLine
                + "You will be informed if re-connection is successful to continue the survey");
                
                //Message box below enables status and sent to update. God knows why..
                AutoClosingMessageBox.Show("BlueTooth lost, attempting to re-connect..." + Environment.NewLine + Environment.NewLine
                       + "Please wait for up to 15 seconds after this message closes."
                       + Environment.NewLine + Environment.NewLine
                       + "Do not continue with the survey until prompted.", "Auto Re-connection attempt", 5000);

                Thread ReattemptThread = new Thread(new ThreadStart(RestartConnectionThread));
                ReattemptThread.Start();

                //time-out method for RestartConnectionThread protocol. Will wait either 15 seconds
                //or until a connection has been re-established in order to show correct prompts.
                //Covers both laptop and tablet faults.
                Stopwatch s = new Stopwatch();
                 s.Start();
                while ((s.ElapsedMilliseconds < 20000 && !deviceInfo.Connected)
                    || (s.ElapsedMilliseconds < 20000 && !checkClientConnection()))
                 {
                    deviceInfo.Refresh();
                 }
                 s.Reset();

                //this.TopMost = true;

                deviceInfo.Refresh();
                if ((!deviceInfo.Connected && successfulConnection == false) | !checkClientConnection())//Failed auto re-connection attempt. Prompt for manual re-connect.
                    {
                        AddToConnectionCSV("Auto Re-connect", "Failure");
                        ReattemptThread.Abort();
                        ManualReconnectionAttempt();
                    }
                    else if(deviceInfo.Connected && successfulConnection == true && checkClientConnection())
                    {
                        /*MessageBox.Show(new Form { TopMost = true }, "Bluetooth was lost, but has successfully re-connected." + Environment.NewLine + Environment.NewLine
                        + "Click OK to continue with the survey and electronic showcards." + Environment.NewLine + Environment.NewLine
                        + "The show-cards will continue once you proceed to the next question in survey.", "Re-connection successful!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        updateStatus("Connection successful!" + Environment.NewLine
                        + "Continue with survey.");*/
                    AddToConnectionCSV("Auto Re-connect", "Success");
                    this.TopMost = false;
                    Thread.Sleep(500);
                    //chrome.Restore();
                }
                this.TopMost = false;
            }
        }

        //Same connection protocol but prompts for manual connect.
        private void ManualReconnectionAttempt()
        {

            deviceInfo.Refresh();

            WindowControl chrome = new WindowControl();
            chrome.AppName = "chrome";
            //WindowControl askiaFace = new WindowControl();
            //chrome.AppName = "AskiaFace";


            if (MessageBox.Show(new Form { TopMost = true }, "Bluetooth auto re-connection attempt failed, or TabletShowcards has been closed." + Environment.NewLine + Environment.NewLine
             + "Click YES to attempt to re-connect. Re-connecting may take up to 15 seconds." + Environment.NewLine + Environment.NewLine +
             "If you wish to exit electronic show-cards completely, click NO. You will be returned to the survey.", "Bluetooth connection lost", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {

                this.TopMost = true;
                updateStatus("BT lost. Attempting re-connection." + Environment.NewLine
                        + "May take up to 15 seconds.");
                    updateSent("BT lost. Attempting re-connection." + Environment.NewLine
                    + "May take up to 15 seconds." + Environment.NewLine
                + "You will be informed if re-connection is successful to continue the survey");

                chrome.Minimize();
               // askiaFace.Minimize();

                //Alerting surveyor that an automatic re-connection attempt is underway
                this.TopMost = true;
                AutoClosingMessageBox.Show("Attempting to re-connect..." + Environment.NewLine + Environment.NewLine
                        + "Please wait for up to 15 seconds after this message closes."
                        + Environment.NewLine + Environment.NewLine
                        + "Do not continue with the survey until prompted.", "Manual Re-connection attempt", 5000);

                Thread ReattemptThread = new Thread(new ThreadStart(RestartConnectionThread));
                ReattemptThread.Start();

                //time-out method for RestartConnectionThread protocol. Will wait either 15 seconds
                //or until a connection has been re-established in order to show correct prompts.
                //Covers both laptop and tablet faults.
                Stopwatch s = new Stopwatch();
                s.Start();
                while ((s.ElapsedMilliseconds < 15000 && !deviceInfo.Connected)
                    || (s.ElapsedMilliseconds < 15000 && !checkClientConnection()))
                {
                    deviceInfo.Refresh();
                }
                s.Reset();

                this.TopMost = true;

                deviceInfo.Refresh();
                    if ((!deviceInfo.Connected && successfulConnection == false) || !checkClientConnection())//Failed auto re-connection attempt. Prompt for manual re-connect.
                    {
                    AddToConnectionCSV("Manual Re-connect", "Failure");
                    ManualReconnectionAttempt();
                    }
                    else if (deviceInfo.Connected)
                    {
                        MessageBox.Show(new Form { TopMost = true }, "Bluetooth was lost, but has successfully re-connected." + Environment.NewLine + Environment.NewLine
                        + "Click OK to continue with the survey and electronic showcards." + Environment.NewLine + Environment.NewLine
                        + "The show-cards will continue once you proceed to the next question in survey." , "Re-connection successful!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        updateStatus("Connection successful!" + Environment.NewLine
                        + "Continue with survey.");
                    AddToConnectionCSV("Manual Re-connect", "Success");
                    this.TopMost = false;                  
                    Thread.Sleep(500);
                    chrome.Restore();
                    //askiaFace.Restore();
                    }
                }
                else
                {
                //Exits the application, however TabletShowcards will still be running.
                chrome.Restore();
                //askiaFace.Restore();
                Application.Exit();
                }
            this.TopMost = false;
        }

        private bool checkClientConnection()
        {
            //check that proper device is connected. HARDCODED FOR SCTAB AND TEST CASE!
           string check = deviceInfo.DeviceName;
            if (!check.Contains("SCTAB") && !check.Contains("DESKTOP-"))
            {
                return false;
            }
            else
            {
                return true;
            }  
        }

        private void AddToConnectionCSV(string failureType, string status)
        {
            //Create CSV monitoring if does not exist.
            System.IO.Directory.CreateDirectory(@"C:\CBGShared\ConnectionMonitoring");

            string username = ReadFirstLine(@"C:\CBGShared\currentuser\currentuser.txt");
            string machinename = Environment.MachineName.ToString();
            string fileName = @"C:\CBGShared\ConnectionMonitoring\" + username + "_ConnectionCSV.csv";

            string currentDateTime = DateTime.Now.ToString();


            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
                var csv1 = new StringBuilder();
                var first1 = "Date";
                var second1 = "Re-connection Type";
                var third1 = "Status";
                var fourth1 = "Username";
                var fifth1 = "Machine Name";
                var newLine1 = string.Format("{0},{1},{2},{3},{4}", first1, second1, third1, fourth1, fifth1);
                csv1.AppendLine(newLine1);
                File.WriteAllText(fileName, csv1.ToString());
            }

            var csv = new StringBuilder();
            var first = currentDateTime;
            var second = failureType;
            var third = status;
            var fourth = username;
            var fifth = machinename;
            var newLine = string.Format("{0},{1},{2},{3},{4}", first, second, third, fourth, fifth);
            csv.AppendLine(newLine);

            File.AppendAllText(fileName, csv.ToString());
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////   

        //Ensures that the given file was created/modified within the given hoursRange.
        private bool CheckTimeStamp(string fileName, int hoursRange)
        {
            
            DateTime modificationTime = File.GetLastWriteTime(fileName);
            DateTime currentTime = DateTime.Now;
            bool sameDate = false;
            bool withinRange = false;

            int modDate = modificationTime.Day;
            int currentDate = currentTime.Day;
            if (modDate == currentDate) { sameDate = true; }

            int modHour = modificationTime.Hour;
            int currentHour = currentTime.Hour;
            if ((currentHour - modHour) <= hoursRange) { withinRange = true; }

            if ((withinRange == true) && (sameDate == true))
            {
                return true;
            }
            else { return false; }
        }

        #endregion

        #region chrome close
        private void CloseChrome()//Makes sure foxit is closed before launch so it can be launch full-screen mode.
        {
            if (Process.GetProcessesByName("chrome").Length > 0)
            {
                foreach (Process proc in Process.GetProcessesByName("chrome"))
                {
                    proc.Kill();
                }
            }
        }

        #endregion

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }

    #region external control classes

    public class AutoClosingMessageBox
    {
        System.Threading.Timer _timeoutTimer;
        string _caption;
        DialogResult _result;
        DialogResult _timerResult;
        AutoClosingMessageBox(string text, string caption, int timeout, MessageBoxButtons buttons = MessageBoxButtons.OK, DialogResult timerResult = DialogResult.None)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            _timerResult = timerResult;
            using (_timeoutTimer)
                _result = MessageBox.Show(text, caption, buttons);
        }
        public static DialogResult Show(string text, string caption, int timeout, MessageBoxButtons buttons = MessageBoxButtons.OK, DialogResult timerResult = DialogResult.None)
        {
            return new AutoClosingMessageBox(text, caption, timeout, buttons, timerResult)._result;
        }
        void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
            _result = _timerResult;
        }
        const int WM_CLOSE = 0x0010;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// </summary>

    public class WindowControl
    {
        //Set-up to declare which application we want to perform controls on. Vary for chrome and 
        //LaptopShowcards minimisation.
        //"chrome"
        //"BluetoothTestClient"
        private string appName;  // the name field
        public string AppName    // the Name property
        {
            get
            {
                return appName;
            }
            set
            {
                appName = value;
            }
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int ForceMinimize = 11;
        private const int RestoreMaximization = 9;
        public void Minimize()
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist.Where(process => process.ProcessName == appName))
            {
                ShowWindow(Process.GetProcessById(process.Id).MainWindowHandle, ForceMinimize);
            }
        }
        public void Restore()
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist.Where(process => process.ProcessName == appName))
            {
                ShowWindow(Process.GetProcessById(process.Id).MainWindowHandle, RestoreMaximization);
            }
        }

    }

    #endregion

}



    
 