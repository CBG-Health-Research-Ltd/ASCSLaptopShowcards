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
        private string _record;//Used as a global to determine if the voice recording is still hapening in secret.
        private string _savedDevice;
        private bool _newDevice;
        private string[] _subStrings;
        private string _mostRecentDevice = null;
        private bool _userInputting = false; //Determines if the current question demands user input.
        private bool _successfulConnection;
        private WifiDirect.WifiDirect _mainForm;

        //Global variables to be updated on separate selection form.
        public string SelectedDevice; //The device selected from opened select device form.
        public int SelectIndex; //The index of this selected device so it may be relayed to the device list constructed for this form.
        public bool DeviceIsSelected = false;//Helps for instances of no device-selection from device form.


        public void Initialize(WifiDirect.WifiDirect frm)
        {
            if (Directory.Exists(Globals.QuestionLog))
            {
                Array.ForEach(Directory.GetFiles(Globals.QuestionLog), File.Delete);
            }
            ReceiveShowcardLists();
            _waveSource = new WaveInEvent();//Needs to be initialised for event firing inr ecording function.
            _recording = false; //needs to be initialised orignially to false for correct functionality.
            MUUID = new Guid("8a63d9e7-ab03-4fd1-b835-9fa143b02c10");//Used on both laptop and tablet for specified conenction.
            _mainForm = frm;


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
        
        
    }
}
