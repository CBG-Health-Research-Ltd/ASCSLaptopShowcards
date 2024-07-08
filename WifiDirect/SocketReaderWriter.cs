//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Devices.Enumeration;
using System.ComponentModel;
using Windows.Devices.WiFiDirect;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.Foundation;
using System.Windows.Forms;

namespace WifiDirect
{
    public class SocketReaderWriter : IDisposable
    {
        DataReader _dataReader;
        DataWriter _dataWriter;
        StreamSocket _streamSocket;
        private WifiDirect _rootPage;

        public SocketReaderWriter(StreamSocket socket, WifiDirect mainPage)
        {
            _dataReader = new DataReader(socket.InputStream);
            _dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
            _dataReader.ByteOrder = ByteOrder.LittleEndian;

            _dataWriter = new DataWriter(socket.OutputStream);
            _dataWriter.UnicodeEncoding = UnicodeEncoding.Utf8;
            _dataWriter.ByteOrder = ByteOrder.LittleEndian;

            _streamSocket = socket;
            _rootPage = mainPage;
        }

        public void Dispose()
        {
            _dataReader.Dispose();
            _dataWriter.Dispose();
            _streamSocket.Dispose();
        }

        public async Task WriteMessageAsync(string message)
        {
            try
            {
                _dataWriter.WriteUInt32(_dataWriter.MeasureString(message));
                _dataWriter.WriteString(message);
                await _dataWriter.StoreAsync();
                _rootPage.Notify("Sent message: " + message);
            }
            catch (Exception ex)
            {
                _rootPage.Notify("WriteMessage threw exception: " + ex.Message);
            }
        }

        public async Task<string> ReadMessageAsync()
        {
            try
            {
                UInt32 bytesRead = await _dataReader.LoadAsync(sizeof(UInt32));
                if (bytesRead > 0)
                {
                    // Determine how long the string is.
                    UInt32 messageLength = _dataReader.ReadUInt32();
                    bytesRead = await _dataReader.LoadAsync(messageLength);
                    if (bytesRead > 0)
                    {
                        // Decode the string.
                        string message = _dataReader.ReadString(messageLength);
                        _rootPage.Notify("Got message: " + message);
                        return message;
                    }
                }
            }
            catch (Exception)
            {
                _rootPage.Notify("Socket was closed!");
            }
            return null;
        }
    }

    public class DiscoveredDevice : INotifyPropertyChanged
    {
        public DeviceInformation DeviceInfo { get; private set; }

        public DiscoveredDevice(DeviceInformation deviceInfo)
        {
            DeviceInfo = deviceInfo;
        }

        public string DisplayName => DeviceInfo.Name + " - " + (DeviceInfo.Pairing.IsPaired ? "Paired" : "Unpaired");
        public override string ToString() => DisplayName;

        public void UpdateDeviceInfo(DeviceInformationUpdate update)
        {
            DeviceInfo.Update(update);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisplayName"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ConnectedDevice : IDisposable
    {

        public WiFiDirectDevice WfdDevice { get; }
        public string DisplayName { get; }

        public ConnectedDevice(string displayName, WiFiDirectDevice wfdDevice)
        {
            DisplayName = displayName;
            WfdDevice = wfdDevice;

        }

        public override string ToString() => DisplayName;

        public void Dispose()
        {
            

            // Close WiFiDirectDevice object
            WfdDevice.Dispose();
        }
    }
}
