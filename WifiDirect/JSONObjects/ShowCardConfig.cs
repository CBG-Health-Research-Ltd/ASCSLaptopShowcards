using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WifiDirectHost.JSONObjects
{

    public class ShowCardConfig
    {
        [JsonProperty("NHC13")]
        public string NHC13;

        [JsonProperty("NHA13")]
        public string NHA13;

        [JsonProperty("NHA14")]
        public string NHA14;

        [JsonProperty("NHC14")]
        public string NHC14;

        [JsonProperty("Y7CVS")]
        public string Y7CVS;

        [JsonProperty("Y7PPM")]
        public string Y7PPM;
    }


}
