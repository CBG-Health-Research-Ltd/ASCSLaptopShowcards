using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WifiDirect;

namespace WifiDirectHost
{
    public class CommunicationServer
    {
        private Stream ClientStream;
        private TcpClient Client;
        private WifiDirect.WifiDirect frm;
        private ShowCardManager showCardManager;
        private string sessionId = "66165b02-d2fc-4917-ae48-baf2e5f4df55";

        public CommunicationServer(TcpClient Client,WifiDirect.WifiDirect frm,ShowCardManager cardManager)
        {
            this.Client = Client;
            ClientStream = Client.GetStream();
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
                    if (command.StartsWith("browse.get:"))
                    {
                        var urlCommand = command.Split(":");
                        if (urlCommand.Length >= 2)
                        {
                            var url = urlCommand[1].Trim();
                            
                            
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
