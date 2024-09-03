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
                    if (command.StartsWith("api.get:"))
                    {
                        var urlCommand = command.Replace("api.get:","");
                            var url = urlCommand.Trim();
                            var returnData=ExtCommunication.BrowseGet(url);
                            WriteToClient(returnData);

                    }
                    if (command.StartsWith("api.download:"))
                    {
                        var urlCommand = command.Replace("api.download:", "");
                            var url = urlCommand.Trim();
                        var returnData = ExtCommunication.DownloadGet(url);
                        WriteFileToClient(returnData);
                        

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
