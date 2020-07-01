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

namespace BluetoothTestClient
{
    public partial class DeviceSelection : Form
    {
        List<string> devicesInfo; //grabs all device items found on previous form operations.
        bool localDeviceSelected = false;

        public DeviceSelection()
        {
            InitializeComponent();
            devicesInfo = Laptop_BT_link.deviceItems;
            updateExteriorDeviceList();
            this.TopMost = true;
            this.WindowState = FormWindowState.Maximized;
            this.FormClosing += DeviceSelection_FormClosing;


            
        }

       /* private void DeviceSelection_FormClosing(object sender, FormClosingEventArgs e)
        {
            throw new NotImplementedException();
        }*/

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Laptop_BT_link.selectedDevice = devicesList.SelectedItem.ToString();
                Laptop_BT_link.selectIndex = devicesList.SelectedIndex;
                devicesList.DataSource = null; //must be set to null in order to clear items list.
                devicesList.Items.Clear();
                Laptop_BT_link.deviceIsSelected = true;
                localDeviceSelected = true;
                this.Close();
            }
            catch(Exception f) { this.Close(); }
           
        }

        private void DeviceSelection_Closed(object sender, EventArgs e)
        {
            devicesInfo = null; //reset devices info so it may be refilled again.
        }

        private void updateExteriorDeviceList()
        {
            Thread.Sleep(500); //ensures that window has opened before filling out device list. (could use shown event).
            devicesList.DataSource = devicesInfo;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Exit Show-cards button
            Application.Exit();
        }

        private void DeviceSelection_FormClosing(Object sender, FormClosingEventArgs e)
        {
            //In case windows is trying to shut down, don't hold the process up
            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            if (this.DialogResult == DialogResult.Cancel && localDeviceSelected == false)
            {
                // X has been selected, so go back to the beginning phase of form 1.
                Laptop_BT_link.deviceIsSelected = false;
                
            }
        }
    }
}
