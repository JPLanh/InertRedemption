﻿using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSender : MonoBehaviour
{
    //private static int localPort;

    //// prefs
    //private string IP;  // define in init
    //public int port;  // define in init

    //// "connection" things
    //IPEndPoint remoteEndPoint;
    //UdpClient client;

    //// gui
    //string strMessage = "";


    //// call it from shell (as program)
    //private static void Main()
    //{
    //    UDPSend sendObj = new UDPSend();
    //    sendObj.init();

    //    // testing via console
    //    // sendObj.inputFromConsole();

    //    // as server sending endless
    //    sendObj.sendEndless(" endless infos \n");

    //}
    // start from unity3d
    public void Start()
    {
        //init();
    }

    //// OnGUI
    //void OnGUI()
    //{
    //    Rect rectObj = new Rect(40, 380, 200, 400);
    //    GUIStyle style = new GUIStyle();
    //    style.alignment = TextAnchor.UpperLeft;
    //    GUI.Box(rectObj, "# UDPSend-Data\n" + IP + ":" + port + " #\n"
    //                + "shell> nc -lu" + IP + ":" + port + " \n"
    //            , style);

    //    // ------------------------
    //    // send it
    //    // ------------------------
    //    strMessage = GUI.TextField(new Rect(40, 420, 140, 20), strMessage);
    //    if (GUI.Button(new Rect(190, 420, 40, 20), "send"))
    //    {
    //        sendString(strMessage + "\n");
    //    }
    //}

    // init
    //public void init()
    //{
    //    // Endpunkt definieren, von dem die Nachrichten gesendet werden.
    //    print("UDPSend.init()");

    //    // define
    //            IP = "192.168.0.141";
    //    //        IP = "192.168.0.179";
    //    //IP = "45.49.197.129";
    //    //        IP = "127.0.0.1";
    //    //        port = 19875;

    //    // ----------------------------
    //    // Senden
    //    // ----------------------------
    //    //        IP = Dns.GetHostAddresses(IP)[0];
    //    //            remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
    //    //          remoteEndPoint = new IPEndPoint(Dns.GetHostAddresses(IP)[0], port);
    //    remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
    //    client = new UdpClient(port);
    //    print(client);

    //    // status
    //    print("Sending to " + IP + " : " + port);
    //    print("Testing: nc -lu " + IP + " : " + port);

    //}

    //// inputFromConsole
    //private void inputFromConsole()
    //{
    //    try
    //    {
    //        string text;
    //        do
    //        {
    //            text = Console.ReadLine();

    //            // Den Text zum Remote-Client senden.
    //            if (text != "")
    //            {

    //                // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
    //                byte[] data = Encoding.UTF8.GetBytes(text);

    //                // Den Text zum Remote-Client senden.
    //                client.Send(data, data.Length);
    //            }
    //        } while (text != "");
    //    }
    //    catch (Exception err)
    //    {
    //        print(err.ToString());
    //    }

    //}

    //// sendData
    //private void sendString(string message)
    //{
    //    try
    //    {
    //        //if (message != "")
    //        //client.Connect(IP, port);

    //        // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
    //        byte[] data = Encoding.UTF8.GetBytes(message);
    //        print(message);

    //        // Den message zum Remote-Client senden.
    //        client.Send(data, data.Length, remoteEndPoint);
    //        //}
    //    }
    //    catch (Exception err)
    //    {
    //        print(err.ToString());
    //    }
    //}


    //// endless test
    //private void sendEndless(string testStr)
    //{
    //    do
    //    {
    //        sendString(testStr);


    //    }
    //    while (true);

    //}

}