﻿//*********************************************************
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
using System.Reflection;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Popups;


namespace WifiDirect
{
    public static class Globals
    {
        // WARNING! This custom OUI is for demonstration purposes only.
        // OUI values are assigned by the IEEE Registration Authority.
        // Replace this custom OUI with the value assigned to your organization.
        public static readonly byte[] CustomOui = { 0xAA, 0xBB, 0xCC };
        public static readonly byte CustomOuiType = 0xDD;

        // OUI assigned to the Wi-Fi Alliance.
        public static readonly byte[] WfaOui = { 0x50, 0x6F, 0x9A };

        // OUI assigned to Microsoft Corporation.
        public static readonly byte[] MsftOui = { 0x00, 0x50, 0xF2 };

        public static readonly string strServerPort = "8080";
        public static readonly int iAdvertisementStartTimeout = 5000; // in ms

        public static readonly string QuestionLog = @"C:\nzhs\questioninformation\QuestionLog\";

        public static CancellationTokenSource AppCancellationTokenSource = new CancellationTokenSource();

        public static string QrCodeData = "WIFI:T:WPA;S:{0};P:{1};H:;;";

        //QR Code text for WIfi example : WIFI:T:WPA;S:IPSOS_JLCRIMSON;P:f4c88a546186;H:;;


        public static string GetAssemblyVersion()
        {

            return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
        }

    }


}
