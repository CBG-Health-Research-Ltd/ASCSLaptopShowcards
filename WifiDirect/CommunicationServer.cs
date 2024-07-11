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
