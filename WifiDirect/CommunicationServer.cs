using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using WifiDirect;

namespace WifiDirectHost
{
    public class CommunicationServer
    {
        private Stream ClientStream;
        private TcpClient Client;
        private WifiDirect.WifiDirect frm;
        private ShowCardManager showCardManager;

        public CommunicationServer(TcpClient client,WifiDirect.WifiDirect frm,ShowCardManager cardManager)
        {
            this.Client = client;
            ClientStream = client.GetStream();
            this.frm = frm;
            showCardManager=cardManager;
            frm.Notify("Connected on Socket. Ready to receive message.");
        }

        public void WriteToClient(string message)
        {
            StreamWriter sw = new StreamWriter(ClientStream);
            sw.WriteLine(message);
            sw.Flush();
        }

        public void WriteFileToClient(byte[] data)
        {
             BinaryWriter sw = new BinaryWriter(ClientStream);
            sw.Write(data);
            sw.Flush();
        }


        public void CloseConnection()
        {
           ClientStream.Close();
        }
        
        
         
        
        
        
        public void ReadFromClient()
        {
            StreamReader sr = new StreamReader(ClientStream);
            try
            {
               
                WriteToClient("Connected to LaptopShowcards");
                string data;
                
                while ((data = sr.ReadLine()) != "exit" || !Globals.AppCancellationTokenSource.IsCancellationRequested)
                {
                    frm.NotifyReceiveMessage(data);
                    var command = data.ToLower();
                    if (command.StartsWith("api.download:"))
                    {
                        var fileDataText = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Config", "ShowCard.json"));

                        var fileJsonData = fileDataText.ToLower();

                        Globals.JsonConfig = JObject.Parse(fileJsonData);
                        
                        var urlCommand = command.Replace("api.download:", "");
                        var filePointer = urlCommand.Trim();
                        var config = Globals.JsonConfig[filePointer];
                        var valueData = ((Newtonsoft.Json.Linq.JValue)config).Value;
                        if (valueData != null)
                        {
                            var fileName = Path.Combine(AppContext.BaseDirectory, "Files", valueData.ToString());
                            var fileData = File.ReadAllBytes(fileName);
                            WriteFileToClient(fileData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                sr.Close();
            }

        }

    }
}
