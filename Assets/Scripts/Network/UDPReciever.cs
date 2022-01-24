using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReciever : MonoBehaviour
{

    void Start()
    {
        NetworkMain.currentPlayer = true;
    }

//    // receiving Thread
//    Thread receiveThread;

//    // udpclient object
//    UdpClient client;

//    // public
//    // public string IP = "127.0.0.1"; default local
//    public int port; // define > init

//    // infos
//    public string lastReceivedUDPPacket = "";
//    public string allReceivedUDPPackets = ""; // clean up this from time to time!

//    // start from unity3d
//    public void Start()
//    {

//        init();
//    }

//    // OnGUI
//    void OnGUI()
//    {
//        Rect rectObj = new Rect(40, 10, 200, 400);
//        GUIStyle style = new GUIStyle();
//        style.alignment = TextAnchor.UpperLeft;
//        GUI.Box(rectObj, "# UDPReceive\n " + ":" + port + " #\n"
//                    + "shell> nc -u " + ":" + port + " \n"
//                    + "\nLast Packet: \n" + lastReceivedUDPPacket
//                    + "\n\nAll Messages: \n" + allReceivedUDPPackets
//                , style);
//    }

//    // init
//    private void init()
//    {
//        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
//        print("UDPSend.init()");

//        //// define port
//        //port = 19875;

//        // status
//        print("Sending to 127.0.0.1 : " + port);
//        print("Test-Sending to this Port: nc -u 127.0.0.1  " + port + "");


//        // ----------------------------
//        // Abhören
//        // ----------------------------
//        // Lokalen Endpunkt definieren (wo Nachrichten empfangen werden).
//        // Einen neuen Thread für den Empfang eingehender Nachrichten erstellen.
//        receiveThread = new Thread(
//            new ThreadStart(ReceiveData));
//        receiveThread.IsBackground = true;
//        receiveThread.Start();

//    }

//    // receive thread
//    private void ReceiveData()
//    {

//                        //IPEndPoint anyIP = new IPEndPoint(Dns.GetHostAddresses("45.49.197.129")[0], 19875);
//        client = new UdpClient(port);
//        while (true)
//        {

//            try
//            {
//                // Bytes empfangen.
//                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
//                print("Part 1");
////                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("192.168.0.141"), 19875);
//                byte[] data = client.Receive(ref anyIP);
//                print("Part 2");

//                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
//                string text = Encoding.UTF8.GetString(data);
//                print("Part 3");

//                // Den abgerufenen Text anzeigen.
//                print(">> " + text);

//                // latest UDPpacket
//                lastReceivedUDPPacket = text;
//                print("Part 4");

//                // ....
//                allReceivedUDPPackets = allReceivedUDPPackets + text;

//            }
//            catch (Exception err)
//            {
//                print(err.ToString());
//            }
//        }
//    }

//    // getLatestUDPPacket
//    // cleans up the rest
//    public string getLatestUDPPacket()
//    {
//        allReceivedUDPPackets = "";
//        return lastReceivedUDPPacket;
//    }


}