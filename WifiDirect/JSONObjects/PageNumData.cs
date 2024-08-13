using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABI.Windows.UI.Composition;

namespace WifiDirectHost.JSONObjects
{
    public class PageNumData
    {
        public string File { get; set; }

        public string Shortcut{ get; set; }

        public string PageNumber{ get; set; }
        
        public int FirstPageIndex { get; set; }

        public string Record { get; set; }
        
        public string SurveyInfo { get; set; }
        
        public bool IsUserInputting { get; set; }
        
        public string[] FullLine { get; set; }
        
        public bool IsFirstShowCardPresented { get; set; }
        
        public bool HasData { get; set; }
        
    }
}
