using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WifiDirect;
using NAudio.Wave;
using System.Media;
using Windows.Security.EnterpriseData;
using NAudio;
using NAudio.Wave.Compression;
using Newtonsoft.Json;
using WifiDirectHost.JSONObjects;

namespace WifiDirectHost
{
    public class ShowCardManager
    {

        public static List<string> DeviceItems;
        public Guid MUUID;
        private Stream _stream;

        public string CurrentDeviceName;
        
        private List<string[]> _adultY13ShowcardList;
        private List<string[]> _childY13ShowcardList;
        private List<string[]> _nzcvsy7ShowcardList;
        private List<string[]> _ppmy7ShowcardList;
        private List<string[]> _adultY14ShowcardList;
        private List<string[]> _childY14ShowcardList;
        private WaveFileWriter _waveFile;
        private WaveInEvent _waveSource;
        private string _desiredShowcard;
        private bool _recording;
        private string _latestFile;
        private string _record="";//Used as a global to determine if the voice recording is still hapening in secret.
        private string _savedDevice;
        private bool _newDevice;
        private string[] _subStrings;
        private string _mostRecentDevice = null;
        public bool UserInputting = false; //Determines if the current question demands user input.
        private bool _successfulConnection;
        private WifiDirect.WifiDirect _mainForm;


        //Global variables to be updated on separate selection form.
        public string SelectedDevice; //The device selected from opened select device form.
        public int SelectIndex; //The index of this selected device so it may be relayed to the device list constructed for this form.
        public bool DeviceIsSelected = false;//Helps for instances of no device-selection from device form.
        FileSystemWatcher _fileWatcher = new FileSystemWatcher();
        string _username = Environment.UserName;
        bool _newFile = false;
        bool _changedFile = false;
        string _pageNum;
        bool _firstShowcardPresented = false;
        private bool recording = false;

        public void Initialize(WifiDirect.WifiDirect frm)
        {

           





            if (Directory.Exists(Globals.QuestionLog))
            {
            //    Array.ForEach(Directory.GetFiles(Globals.QuestionLog), File.Delete);
            }

            if (!Directory.Exists(Globals.QuestionLog))
            {
                Directory.CreateDirectory(Globals.QuestionLog);
            }
            ReceiveShowcardLists();
            _waveSource = new WaveInEvent();//Needs to be initialised for event firing inr ecording function.
            _recording = false; //needs to be initialised orignially to false for correct functionality.
            MUUID = new Guid("8a63d9e7-ab03-4fd1-b835-9fa143b02c10");//Used on both laptop and tablet for specified conenction.
            _mainForm = frm;

            

            //New file watcher initialised to monitor the QuesitonLog folder and raise created/updated events.
            //This process is used along side page turner ex to retrieve the current survey question name as a .txt file.
      
            _fileWatcher.Path = Globals.QuestionLog;
            _fileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                                                | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            _fileWatcher.Filter = "*.txt";



            //Event handling for given filewatcher filters. Allows detection of file update/creation and
            //modification detection of POST info sent by bluetooth link.
            _fileWatcher.EnableRaisingEvents = true;
            _fileWatcher.Created += delegate { _newFile = true; };
            _fileWatcher.Changed += delegate { _changedFile = true; };

            recording = false;


        }


        private void ReceiveShowcardLists()
        {
            //This method could be cleaned up in the future
            _childY13ShowcardList = GetShowcardPageList("CHILDY13");
            _adultY13ShowcardList = GetShowcardPageList("ADULTY13");
            _nzcvsy7ShowcardList = GetShowcardPageList("NZCVSY7");
            _ppmy7ShowcardList = GetShowcardPageList("PPMY7");
            _childY14ShowcardList = GetShowcardPageList("CHILDY14");
            _adultY14ShowcardList = GetShowcardPageList("ADULTY14");
        }

        private List<string[]> GetShowcardPageList(string survey)//takes the type of survey CHILD or ADULT to dtetermine list to load.
        {
            string User = Environment.UserName;
            string[] ShowcardPageArray = new string[0];
            try
            {
                switch (survey)
                {
                    case ("CHILDY13"):
                        ShowcardPageArray = File.ReadAllLines(@"C:\CBGShared\surveyinstructions\NZHSY13ChildInstructions.txt", Encoding.Default);
                        break;
                    case ("ADULTY13"):
                        ShowcardPageArray = File.ReadAllLines(@"C:\CBGShared\surveyinstructions\NZHSY13AdultInstructions.txt", Encoding.Default);
                        break;
                    case ("NZCVSY7"):
                        ShowcardPageArray = File.ReadAllLines(@"C:\CBGShared\surveyinstructions\NZCVSY7Instructions.txt", Encoding.Default);
                        break;
                    case ("PPMY7"):
                        ShowcardPageArray = File.ReadAllLines(@"C:\CBGShared\surveyinstructions\PPMY7Instructions.txt", Encoding.Default);
                        break;
                    case ("CHILDY14"):
                        ShowcardPageArray = File.ReadAllLines(@"C:\CBGShared\surveyinstructions\NZHSY14ChildInstructions.txt", Encoding.Default);
                        break;
                    case ("ADULTY14"):
                        ShowcardPageArray = File.ReadAllLines(@"C:\CBGShared\surveyinstructions\NZHSY14AdultInstructions.txt", Encoding.Default);
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
                _subStrings = ShowcardPageArray[i].Split(splitter);//Forming array into sub strings so it may be added to list
                shoPageList.Add(_subStrings);//Generating the pageNum -> ShoCard list.
            }
            return shoPageList;
        }

        public void ReceiveMessage(string message)
        {
            
        }

        public void SendMessage(string message)
        {
            _mainForm.SendMessage(message);
        }

        private string GetLatest(string directory)//Gets the name of the latest file created/updated in QuestionLog directory.
        {
            string Username = Environment.UserName;
            DirectoryInfo questionDirectory = new DirectoryInfo(directory);
            string latestFile = FindLatestFile(questionDirectory).Name;
            return latestFile;

        }
        
        

        private static FileInfo FindLatestFile(DirectoryInfo directoryInfo)//Gets file info of latest file updated/created in directory.
        {
            if (directoryInfo == null || !directoryInfo.Exists)
                return null;

            var myFile = directoryInfo.GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .First();
            return myFile;
        }

        private string ObtainShowcard(string inputTxt,PageNumData pageNumData)
        {

            //Obtain the question number and then find the corresponding showcard from look up.
            if (string.Equals(inputTxt.Substring(0, 8), "question", StringComparison.CurrentCultureIgnoreCase)) ; //Makesure question&QN& CHILD/ADULT
            {
                char Qsplitter = ' ';//Splitting at the space between e.g. "question14 CHILD/ADULT" (the raw survey args).
                string[] subStrings = inputTxt.Split(Qsplitter);
                string questionNum = subStrings[0].Substring(8); //Page number hardcoded to correspond to question number.
                string surveyInfo = subStrings[1];//Either CHILD or ADULT (determines which survey look up and PDF to use).
                pageNumData.SurveyInfo = surveyInfo;
                pageNumData.Shortcut = questionNum;
                return ReturnDesiredShowcard(surveyInfo, questionNum,pageNumData);
            }

            return "non question specific detail from TSS. Check pageturner.txt";

        }



        //Two functions below must be changed to accomodate new surveys, also the global variable list instantiation
        //must be updated so that showcard reference lists are generated upon application launch.
        private List<string[]> getShowcardList(string survey)
        {
            survey = survey.ToLower();
            List<string[]> showcardList = new List<string[]>();
            switch (survey)
            {
                case ("nhc13"):
                    showcardList = _childY13ShowcardList;//UPDATE FOR ASKIA SURVEY
                    break;
                case ("nha13"):
                    showcardList = _adultY13ShowcardList;//UPDATE FOR ASKIA SURVEY
                    break;
                case ("y7cvs"):
                    showcardList = _nzcvsy7ShowcardList;
                    break;
                case ("y7ppm"):
                    showcardList = _ppmy7ShowcardList;
                    break;
                case ("nhc14"):
                    showcardList = _childY14ShowcardList;//UPDATE FOR ASKIA SURVEY
                    break;
                case ("nha14"):
                    showcardList = _adultY14ShowcardList;//UPDATE FOR ASKIA SURVEY
                    break;

            }
            return showcardList;
        }

        private int FirstPageIndex(string survey)
        {
            survey = survey.ToLower();
            int pageIndex = 0;
            switch (survey)
            {
                case ("nha13"):
                    pageIndex = 3;//UPDATE FOR ASKIA SURVEY first seen QID that has a showcard
                    break;
                case ("nhc13"):
                    pageIndex = 3;//UPDATE FOR ASKIA SURVEY
                    break;
                case ("y7cvs"):
                    pageIndex = 2;//UPDATE!!!!
                    break;
                case ("y7ppm"):
                    pageIndex = 2;//UPDATE!!!!
                    break;
                case ("nha14"):
                    pageIndex = 3;//UPDATE FOR ASKIA SURVEY first seen QID that has a showcard
                    break;
                case ("nhc14"):
                    pageIndex = 3;//UPDATE FOR ASKIA SURVEY
                    break;
            }
            return pageIndex;
        }
        private string ReturnDesiredShowcard(string survey, string questionNum, PageNumData pageNumData)  //Quite an ugly function but no other option really.
        {
            int i = 0;
            pageNumData.HasData = false;
            string surveyType = survey.ToLower();
            List<string[]> showcardList = getShowcardList(surveyType);
            int firstPageIndex = FirstPageIndex(surveyType);
            pageNumData.FirstPageIndex = firstPageIndex;
            while (i < showcardList.Count)
            {
                if ((showcardList[i])[0] == questionNum)//Page num is first element i.e. 0 index of showcard list entries.
                {
                    pageNumData.Shortcut = showcardList[i][0];
                    pageNumData.PageNumber = showcardList[i][1];
                    pageNumData.Record=showcardList[i][2];
                    pageNumData.HasData = true;
                    pageNumData.FullLine = showcardList[i];
                    //IMPORTANT: IF showcard-list[i] contains 'user-input', then desiredShowcard = user-input(ID). Tablet will have to
                    //open in full-screen chrome environment to gain user input. "user-input questionNum".
                    if (showcardList[i].Contains("user-input"))
                    {
                        _desiredShowcard = "user-input" + questionNum;
                        UserInputting = true;
                        break;
                    }

                    _desiredShowcard = (showcardList[i])[1];//Because desired showcard is found in 2nd index of list.
                    _firstShowcardPresented = true;
                    break;
                }
                //Automatically returns to title page for the case that showcards haven't started being displayed.
                //EXTRMEMELY IMPORTANT: 20 is hardcoded for NZCHSY7 survey!!!
                else if (_firstShowcardPresented == false)
                {
                    _desiredShowcard = "0"; //Because on tablet side, a +1 is added to 'desiredShowcard'. So actual display at "1"
                }
                else
                {
                    _desiredShowcard = "1";//Because on tablet side, a +1 is added to 'desiredShowcard'. "2" shown if non-existant showcard.
                }
                i++;
            }

            pageNumData.IsUserInputting = UserInputting;


            pageNumData.IsFirstShowCardPresented = _firstShowcardPresented;

            return (_desiredShowcard + " " + survey);
            //return "Non specific survey detail, check pageturner.txt";
        }


        public void PollTxtFile() //Checks the most recent .txt file update in QuestionLog folder. Handles null exceptions.
        {
            if (_newFile == true || _changedFile == true) //Un-comment else for user-interactivity
            {
                //Below only occurrs on the event of a .txt file update or creation withing QuestionLog folder.
                //Open a dictionary which contains the relevant bookmark for each question.

                PageNumData pageNumData = new PageNumData();
                
                var fileName= GetLatest(Globals.QuestionLog);

                _latestFile= Path.GetFileNameWithoutExtension(fileName);

                pageNumData.File = fileName;
                
                _pageNum = ObtainShowcard(_latestFile,pageNumData);

                if (!string.IsNullOrEmpty(pageNumData.PageNumber))
                {
                    if (int.TryParse(pageNumData.PageNumber, out var pageNumber)) {
                        pageNumData.PageNumber = (pageNumber + 1).ToString();
                    }
                }


                string jsonData = JsonConvert.SerializeObject(pageNumData);

                _mainForm.Notify(_latestFile + " corresponds to: " + _pageNum);

                
                TransmitText(jsonData); 
                
                
                _changedFile = false;
                _newFile = false;
            }
            else if (UserInputting == true)
            {
              //  readStream(stream);
            } //reads incoming stream for user-input


        }

        private void TransmitText(string text)
        {
            _mainForm.SendMessage(text);
        }

        public void ReadStream(string data)
        {
            try
            {
                if (!Directory.Exists(@"C:\CBGShared\postRetrieve\"))
                {
                    Directory.CreateDirectory(@"C:\CBGShared\postRetrieve\");
                }
                File.WriteAllText(@"C:\CBGShared\postRetrieve\post.txt", data);
            }
            catch (Exception e) { }
        }


        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            _waveFile.WriteData(e.Buffer, 0, e.BytesRecorded);

        }

        private void RecordInSecret()
        {

            //If the previous question was recording, then this will stop it from recording and dispose the 
            //waveSource. waveSource is re initialised when told to being recording again.
            if (recording == true)
            {
                _waveSource.StopRecording();
                recording = false;
                _waveFile.Dispose();
            }

            string userName = System.Environment.UserName;

            if (_record == "record")
            {
                _waveSource = new WaveInEvent();
                _waveSource.WaveFormat = new WaveFormat(8000, 1);
                _waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
                string fileName = _latestFile.Replace(' ', '_') + "_" + DateTime.Now.ToString("h_mm_ss tt").Replace(' ', '_');

                string tempFile = (@"C:\RecordedQuestions\" + fileName + ".wav");
                _waveFile = new WaveFileWriter(tempFile, _waveSource.WaveFormat);
                _waveSource.StartRecording();
                recording = true;
            }

        }

        private string ShouldYouRecord(string inputTxt) //This could be tidied up in the future.
        {
            //Obtain the question number and then find the corresponding showcard from look up.
            if (string.Equals(inputTxt.Substring(0, 8), "question", StringComparison.CurrentCultureIgnoreCase)) ; //Makesure question&QN& CHILD/ADULT
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


        private string ReadFirstLine(string textfile)
        {
            string permission = File.ReadLines(textfile).First();
            return permission;
        }


        public void AddToConnectionCSV(string failureType, string status)
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

    }
}
