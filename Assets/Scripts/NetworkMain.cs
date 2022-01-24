using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Socket.Quobject.SocketIoClientDotNet.Client;
using Socket.Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public class NetworkMain : MonoBehaviour
{
    private static bool isConnected;
    public static Dictionary<string, ClientsInfo> listOfClients = new Dictionary<string, ClientsInfo>();
    public static string clientType;
    #region QSocket Variables
    public static QSocket socket;
    private static string url;
    public static String Username { get; set; }
    public static String Password { get; set; }
    public static String Room { get; set; }
    public static String LobbyID { get; set; }
    public static String UserID { get; set; }
    public static String Team { get; set; }
    public static bool local { get; set; }
    //    public static Queue<string> serverResponse = new Queue<string>();
    public static Queue<Dictionary<string, string>> serverResponse = new Queue<Dictionary<string, string>>();
    public static Queue<string> lobbyResponse = new Queue<string>();
    public static Queue<string> loginResponse = new Queue<string>();
    public static Stack<string> updateResponse = new Stack<string>();
    public static Stack<string> updateResponseItnterpolation = new Stack<string>();
    public static bool isHost;

    #endregion

    public static bool currentPlayer = false;

    #region reciever variables
    //    // receiving Thread
    static Thread receiveThread;

    //    // udpclient object
    static UdpClient recClient;

    //    // public
    //    // public string IP = "127.0.0.1"; default local
    public static int recieverPort = 27000;

    public static Queue<Dictionary<string, string>> serverRecResponse = new Queue<Dictionary<string, string>>();
    public static Stack<Dictionary<string, string>> updateRecResponse = new Stack<Dictionary<string, string>>();
    //public static Stack<string> recieveUpdateResponse = new Stack<string>();
    //    // infos
    public static string lastReceivedUDPPacket = "";
    public static string allReceivedUDPPackets = ""; // clean up this from time to time!

    public static Dictionary<string, Stack<Payload>> payloadStack = new Dictionary<string, Stack<Payload>>();
    #endregion

    #region sender variables

    // prefs
    public static string serverIP;  // define in init

    public static Queue<Dictionary<string, string>> clientSenderResponse = new Queue<Dictionary<string, string>>();
    // "connection" things
    static IPEndPoint remoteEndPoint;
    static UdpClient sendClient;

    #endregion
    //    public static Dictionary<string, LinkedList<string>> updateEachResponse = new Dictionary<string, LinkedList<string>>(); 

    //void OnDestroy()
    //{
    //    broadcastAction("Eliminate");
    //    socket.Disconnect();
    //}

    void OnDestroy()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
                if (clientType == "Client")
        {

            payload["name"] = Username;
            payload["Action"] = "Exit";
            blindSend(payload);
        }

        if (isConnected)
        {
            socket.Disconnect();
        }
    }
    public static void disconnect()
    {
        if (!local && socket != null) socket.Disconnect();
    }

    void Start()
    {
    }


    #region QSocket implementation
    public static void joinGame(String username, String password, string in_action)
    {
        try
        {
            //            GameObject.Find("Listener").GetComponent<LoginServerListener>().alert.GetComponent<Alert>().Show("Connecting", 5f);
            socket = IO.Socket("http://35.212.249.77:26843");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }


        socket.On(QSocket.EVENT_CONNECT, () =>
        {
            isConnected = true;
            try
            {
                Dictionary<string, string> payload = new Dictionary<string, string>();
                payload["Username"] = username;
                payload["Password"] = password;
                payload["Action"] = in_action;
                socket.Emit("Login", StringUtils.convertPayloadToJson(payload));
            } catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            //            socket.Emit("Joined");
            //socket.Emit("WhoAmI");
            //            GameObject.Find("Listener").GetComponent<LoginServerListener>().alert.GetComponent<Alert>().Show("Validating", 5f);
            //if (getAction.Equals("Login")) socket.Emit("Login", "Check", username, password);
            //if (getAction.Equals("Register")) socket.Emit("Register", "Check", username, password);
        });

        socket.On("disconnect", () =>
        {
            Debug.Log("Disconnectiong");
        });

        socket.On("CreateServer", (getData) =>
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["Username"] = username;
            payload["Action"] = "Create Server";
            //serverResponse.Enqueue(getData.ToString());
//            serverResponse.Enqueue(JsonConvert.DeserializeObject<Dictionary<string, string>>(getData.ToString()));
        });

        socket.On("Broadcast", (getData) =>
        {
//            Debug.Log($"Broadcast 1: {getData.ToString().Replace('`', '\"').Replace("\\", string.Empty).Replace("\"{", "{").Replace("}\"", "}")}");
            Payload lv_payload = JsonConvert.DeserializeObject<Payload>(getData.ToString()
                .Replace('`', '\"')
                .Replace("\\", string.Empty)
                .Replace("\"{", "{")
                .Replace("}\"", "}")
               );

            if(lv_payload.data["Action"].Equals("Join Game"))
            {
                serverResponse.Enqueue(lv_payload.data);
                payloadStack.Add(lv_payload.source, new Stack<Payload>());
            } else
            {
                payloadStack[lv_payload.source].Push(lv_payload);
            }
        });

        socket.On("Login", (getData) =>
        {
            serverResponse.Enqueue(JsonConvert.DeserializeObject<Dictionary<string, string>>(getData.ToString()));
        });

        socket.On("Register", (getData) =>
        {
            //serverResponse.Enqueue(getData.ToString());
            serverResponse.Enqueue(JsonConvert.DeserializeObject<Dictionary<string, string>>(getData.ToString()));
        });

        socket.On("Join", (getData) =>
        {
            //serverResponse.Enqueue(getData.ToString());
            serverResponse.Enqueue(JsonConvert.DeserializeObject<Dictionary<string, string>>(getData.ToString()));
        });

        socket.On("Minion", (getData) =>
        {
            //            NetworkListener.playerUpdates[getMinion].Push(getData.ToString());
            updateResponse.Push(getData.ToString());
        });

        socket.On("Lobby Update", (getData) =>
        {
//            updateResponse.Push(getData.ToString());
            serverResponse.Enqueue(JsonConvert.DeserializeObject<Dictionary<string, string>>(getData.ToString()));
        });

        socket.On("Mass Update", (getData) =>
        {
            updateResponseItnterpolation.Push(getData.ToString());
        });

        socket.On("Update", (getData) =>
        {
            serverResponse.Enqueue(JsonConvert.DeserializeObject<Dictionary<string, string>>(getData.ToString()));
        });

        socket.On("Resource Update", (getData) =>
        {
            serverResponse.Enqueue(JsonConvert.DeserializeObject<Dictionary<string, string>>(getData.ToString()));
        });

        socket.On("Action", (getData) =>
        {
            serverResponse.Enqueue(JsonConvert.DeserializeObject<Dictionary<string, string>>(getData.ToString()));
        });

        socket.On("Exit", (getData) =>
        {
            //serverResponse.Enqueue(getData.ToString());
            serverResponse.Enqueue(JsonConvert.DeserializeObject<Dictionary<string, string>>(getData.ToString()));
        });

        socket.On("Lobby", (getData) =>
        {
            lobbyResponse.Enqueue(getData.ToString());
        });

        socket.On("Test", (getData) =>
        {
            print(getData.ToString());
        });
    }

    public static void test(string getData)
    {
        print(getData.ToString());

        JsonConvert.DeserializeObject<Dictionary<string, string>>(getData.ToString());
        //print("Testing"); NetworkListener.playerUpdates[getPlayer["userID"]].Push(getData.ToString());
        print(getData.ToString());
    }

    public static void PositionalUpdate(string getTeam, Vector3 getPosition, Vector3 getRotation)
    {
        if (!local)
        {
            Dictionary<string, string> payload = StringUtils.getPositionAndRotation(getPosition, getRotation);
            socket.Emit("Update", StringUtils.convertPayloadToJson(payload));
        }
    }

    //public static void getUpdate(string getUserID)
    //{
    //    socket.Emit("Get Update", getUserID);
    //}

    public static void minionPositionalUpdate(string minionName, string getTeam, int getNum, float getCount, Vector3 getPosition, Vector3 getRotation)
    {
        if (!local)
        {
            Dictionary<string, string> payload = StringUtils.getPositionAndRotation(getPosition, getRotation);
            payload["Team"] = getTeam;
            payload["Minion"] = minionName;
            payload["Count"] = StringUtils.convertFloatToString(getCount);
            payload["MinionNum"] = StringUtils.convertIntToString(getNum);
            socket.Emit("Minion", StringUtils.convertPayloadToJson(payload));
        }
    }
    /**
     *  Relay to everyone
     */
    //public static void broadcast(Dictionary<string, string> getPayload)
    //{
    //    if (!local)
    //    {
    //        getPayload["Source"] = Username;
    //        getPayload["UserID"] = UserID;
    //        socket.Emit("Everyone", StringUtils.convertPayloadToJson(getPayload));
    //    }
    //}
    public static void broadcast(Dictionary<string, string> getPayload, string getTarget)
    {
        if (!local)
        {
            Dictionary<string, string> networkPayload = new Dictionary<string, string>();
            networkPayload["source"] = Username;
            networkPayload["data"] = StringUtils.convertPayloadToJson(getPayload);
            networkPayload["target"] = getTarget;
            socket.Emit("Broadcast", StringUtils.convertPayloadToJson(networkPayload));
        }
    }

    /**
     *  Update on server
     */
    public static void messageServer(Dictionary<string, string> getPayload)
    {
        if (!local && isHost)
        {
            socket.Emit("Server", StringUtils.convertPayloadToJson(getPayload));
        }
    }
    /**
     * Get updates from server
     */
    public static void getUpdates(Dictionary<string, string> getPayload)
    {
        if (!local)
        {
            socket.Emit("Self", StringUtils.convertPayloadToJson(getPayload));
        }
    }

    public static void updateEntityPosition(GameObject obj)
    {

        Dictionary<string, string> payload = StringUtils.getPositionAndRotation(obj.transform.position, obj.transform.rotation);
        payload["Action"] = "Update";
        payload["State"] = "Force";

        payload["WeaponState"] = StringUtils.convertIntToString(obj.GetComponent<PlayerController>().weaponState);
        payload["name"] = obj.name;
        NetworkMain.socket.Emit("Update", StringUtils.convertPayloadToJson(payload));
    }

    public static void Login(string in_username, string in_server, string in_action)
    {

        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["Username"] = in_username;
        payload["lobbyID"] = in_server;
        payload["Action"] = in_action;
        socket.Emit("Login", StringUtils.convertPayloadToJson(payload));
    }

    public static void updateAction(string getString)
    {
        if (!local)
        {
            Dictionary<string, string> payload = StringUtils.getPayload();
            payload.Add("Action", getString);
            payload.Add("lobbyID", NetworkMain.LobbyID);
            getUpdates(payload);
        }
    }

    public static void broadcastToOthers(Dictionary<string, string> getPayload)
    {
        if (!local)
        {
            socket.Emit("Others", StringUtils.convertPayloadToJson(getPayload));
        }
    }

    public static void broadcastAction(string getString)
    {
        if (!local)
        {
            Dictionary<string, string> payload = StringUtils.getPayload();
            payload.Add("Action", getString);
            broadcast(payload, null);
        }
    }

    public static void broadcastAction(string getString, string getTarget)
    {
        if (!local)
        {
            Dictionary<string, string> payload = StringUtils.getPayload();
            payload.Add("Action", getString);
            broadcast(payload, getTarget);
        }
    }

    public static void broadcastAction(Dictionary<string, string> in_payload)
    {
        if (!local)
        {
            broadcast(in_payload, null);
        }
    }

    public static void createServer()
    {

    }
    #endregion

    #region UDPReciever
    public static void recieverInit()
    {
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }




    public static void broadcastClientsAction(string getAction, string getUser, string getUserID)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["Action"] = getAction;
        payload["name"] = getUser;
        payload["UserID"] = getUserID;
        foreach (KeyValuePair<string, ClientsInfo> perClient in listOfClients)
        {
            perClient.Value.sendPayload(payload);
        }
    }

    // receive thread
    private static void ReceiveData()
    {
        recClient = new UdpClient(recieverPort);
        recClient.Client.SendTimeout = 5000;
        while (true)
        {

            remoteEndPoint = new IPEndPoint(IPAddress.Any, recieverPort);
            var data = recClient.Receive(ref remoteEndPoint);
            string text = Encoding.UTF8.GetString(data);
            Dictionary<string, string> payload = StringUtils.parsePayload(text);
            if (payload["Action"] == "Join")
            {
            payload["ipAddress"] = remoteEndPoint.Address.ToString();
            payload["Port"] = StringUtils.convertIntToString(remoteEndPoint.Port);
            }
            switch (payload["Mode"])
            {
                case "Forward":
                    switch (payload["Action"])
                    {
                        case "Update":
                            updateRecResponse.Push(payload);
                            payload["Mode"] = "Update";
                            break;
                        default:
                            serverRecResponse.Enqueue(payload);
                            payload["Mode"] = "Action";
                            break;
                    }
                    broadcastClients(payload);
                    break;
                case "Action":
                    serverRecResponse.Enqueue(payload);
                    break;
                case "Update":
                    updateRecResponse.Push(payload);
                    break;

            }
        }
    }

        public static void broadcastClients(Dictionary<string, string> payload)
    {

        foreach (KeyValuePair<string, ClientsInfo> perClient in listOfClients)
        {
            perClient.Value.sendPayload(payload);
        }
    }

    public static void server_forwardToClients(string message)
    {

    }

    public static void sendReply(string message, string ipAddress, string port)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Parse(ipAddress), StringUtils.convertToInt(port));
            recClient.Send(data, data.Length, clientEP);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    public static void sendReply(Dictionary<string,string> message, string ipAddress, string port)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(StringUtils.convertPayloadToJson(message));
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Parse(ipAddress), StringUtils.convertToInt(port));
            recClient.Send(data, data.Length, clientEP);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    // getLatestUDPPacket
    // cleans up the rest
    public static string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }
    #endregion

    #region UDPSender

    public static void senderInit()
    {
        sendClient = new UdpClient();
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), recieverPort);
        sendClient.Connect(remoteEndPoint);
        sendClient.Client.SendTimeout = 5000;
        sendClient.Client.ReceiveTimeout = 5000;
    }

    // sendData
    public static void sendString(Dictionary<string,string> payload)
    {
        try
        {
            string message = StringUtils.convertPayloadToJson(payload);
            byte[] data = Encoding.UTF8.GetBytes(message);

            sendClient.Send(data, data.Length);

            var dataRec = sendClient.Receive(ref remoteEndPoint);
            string text = Encoding.UTF8.GetString(dataRec);
            serverRecResponse.Enqueue(StringUtils.parsePayload(text));
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }


    public static void broadcastActions(string getString)
    {
        if (!local)
        {
            Dictionary<string, string> payload = StringUtils.getPayload();
            payload["Action"] = getString;
            payload["Source"] = Username;
            payload["UserID"] = UserID;
            string message = StringUtils.convertPayloadToJson(payload);
            byte[] data = Encoding.UTF8.GetBytes(message);
            sendClient.Send(data, data.Length);
        }
    }

    public static void client_sendActionToClients(string getAction)
    {

        Dictionary<string, string> payload = StringUtils.getPayload();
        payload["Action"] = getAction;
        payload["name"] = Username;
        payload["UserID"] = UserID;
        payload["Mode"] = "Forward";
        string message = StringUtils.convertPayloadToJson(payload);
        byte[] data = Encoding.UTF8.GetBytes(message);
        sendClient.Send(data, data.Length);
    }

    public static void blindSend(Dictionary<string, string> payload)
    {
        try
        {
            string message = StringUtils.convertPayloadToJson(payload);
            byte[] data = Encoding.UTF8.GetBytes(message);

            sendClient.Send(data, data.Length);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }

    }

    // sendData
    public static void sendString(string message, IPEndPoint endPoint)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            sendClient.Send(data, data.Length, endPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
    #endregion
}

[Serializable]
public struct Payload
{
    public string source;
    public Dictionary<string, string> data;
    public string target;
}

public class ClientsInfo
{

    public ClientsInfo(string getIpAddress, string getPort)
    {
        ipAddress = getIpAddress;
        port = getPort;

        client = new UdpClient();
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(getIpAddress), 27000);
        client.Connect(remoteEndPoint);
        client.Client.SendTimeout = 5000;
    }

    public void sendPayload(Dictionary<string, string> payload)
    {
        try
        {
        string message = StringUtils.convertPayloadToJson(payload);
        byte[] data = Encoding.UTF8.GetBytes(message);
            //Debug.Log(message);
        client.Send(data, data.Length);
        }
        catch (Exception err)
        {
            Debug.Log(err);
        }
    }

    public UdpClient client { get; set; }
    public string ipAddress { get; set; }
    public string port { get; set; }
}

public class PlayerNetworkListener
{
    public string player;
    public IPlayerController controller;

    public PlayerNetworkListener(string in_player)
    {
        player = in_player;
    }

    public Payload networkListen()
    {
        if (NetworkMain.payloadStack[player].Count > 0)
        {
            controller.serverControl(NetworkMain.payloadStack[player].Pop());
        }
        return new Payload();
    }


}