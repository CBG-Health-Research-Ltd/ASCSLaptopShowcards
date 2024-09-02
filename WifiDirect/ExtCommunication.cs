using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace WifiDirectHost
{
    public  class ExtCommunication
    {
        public static string SmAuthorize = "66165b02-d2fc-4917-ae48-baf2e5f4df55";
        public static string BrowseGet(string url)
        {
            var options = new RestClientOptions()
            {
                MaxTimeout = 60000
            };
            var client = new RestClient(options);
            var request = new RestRequest(url,Method.Get);
            request.AddHeader("sm-authorize",  SmAuthorize);
            request.RequestFormat = DataFormat.Json;
            var response = client.Execute(request);
            if (response.Content != null)
            {
                return response.Content;
            }
            else
            {
                return "{}";
            }
        }
    }
}
