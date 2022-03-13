using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Socket.Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public class NetworkMain : MonoBehaviour
{
    private static bool isConnected { get; set; }
    public static bool isPlaying;
    public static bool isBroadcastable;
    public static QSocket socket;
    public static String Username { get; set; }
    public static String Password { get; set; }
    public static String LobbyID { get; set; }
    public static String UserID { get; set; }
    public static String Team { get; set; }
    public static bool local { get; set; }
    public static Queue<Payload> serverResponse = new Queue<Payload>();
    public static Queue<Dictionary<string, string>> mainMenuResponse = new Queue<Dictionary<string, string>>();

    public static Dictionary<string, PlayerNetworkListener> payloadStack = new Dictionary<string, PlayerNetworkListener>();
    public static bool currentPlayer = false;

    void OnApplicationQuit()
    {
        if (isConnected)
        {
            NetworkMain.broadcastAction("Exit");
            socket.Disconnect();
        }
    }
    public static void disconnect()
    {
        if (socket != null) socket.Disconnect();
    }

    void Start()
    {
    }


    #region QSocket implementation
    public static void joinGame(String username, String password, string in_action)
    {

            socket = IO.Socket("http://35.212.249.77:26842");
        socket.On(QSocket.EVENT_CONNECT, () =>
        {
            isConnected = true;
            Debug.Log("Connecting");
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
        });

        socket.On("disconnect", () =>
        {
            Debug.Log("Disconnectiong");
        });

        socket.On("Broadcast", (getData) =>
        {
            Payload lv_payload = JsonConvert.DeserializeObject<Payload>(getData.ToString()
                .Replace('`', '\"')
                .Replace("\\", string.Empty)
                .Replace("\"{", "{")
                .Replace("}\"", "}")
               );

//            if (!lv_payload.data["Type"].Equals("Player Update"))
//                Debug.Log($"Broadcast 1: {getData.ToString().Replace('`', '\"').Replace("\\", string.Empty).Replace("\"{", "{").Replace("}\"", "}")}");

            switch (lv_payload.data["Type"])
            {
                case "Action":
                    serverResponse.Enqueue(lv_payload);
                    break;
                case "Player Update":
                    if (payloadStack.ContainsKey(lv_payload.source))
                        payloadStack[lv_payload.source].positionQueue.Push(lv_payload);
                    break;
                case "Player Action":
                    if (payloadStack.ContainsKey(lv_payload.source))
                    {
                        payloadStack[lv_payload.source].actionQueue.Push(lv_payload);
                    }
                    break;

            }

            //switch (lv_payload.data["Action"])
            //{
            //    case "Exit":
            //    case "Message":
            //    case "Swap Team":
            //    case "Ready":
            //    case "Pickup Item":
            //    case "Damage Resource":
            //    case "Join Acknowledge":
            //    case "Join Game":
            //        serverResponse.Enqueue(lv_payload);
            //        break;
            //    case "Get Lobby Users":
            //        if (!lv_payload.source.Equals(UserID))
            //        {
            //            Dictionary<string, string> payload = new Dictionary<string, string>();
            //            payload.Add("Username", NetworkMain.Username);
            //            payload.Add("UserID", NetworkMain.UserID);
            //            payload.Add("Team", NetworkMain.Team);
            //            payload.Add("Action", "In Lobby");
            //            NetworkMain.replyAction(payload, lv_payload.data["UserID"]);
            //            serverResponse.Enqueue(lv_payload);
            //        }
            //        break;
            //    case "WhoisHere":
            //        whoisHere(lv_payload);
            //        break;
            //    case "Update":
            //        if (isPlaying) payloadStack[lv_payload.source].positionQueue.Push(lv_payload);
            //        break;
            //    default:
            //        if (isPlaying) payloadStack[lv_payload.source].actionQueue.Push(lv_payload);
            //        break;
            //}
        });

        socket.On("Loading", (getData) =>
        {
//            Debug.Log(getData);
            Payload lv_payload = JsonConvert.DeserializeObject<Payload>(getData.ToString()
                .Replace('`', '\"')
                .Replace("\\", string.Empty)
                .Replace("\"{", "{")
                .Replace("}\"", "}")
            );

            serverResponse.Enqueue(lv_payload);
        });

        socket.On("Reply", (getData) =>
        {
            Payload lv_payload = JsonConvert.DeserializeObject<Payload>(getData.ToString()
                .Replace('`', '\"')
                .Replace("\\", string.Empty)
                .Replace("\"{", "{")
                .Replace("}\"", "}")
            );

            serverResponse.Enqueue(lv_payload);
        });

        socket.On("Action", (getData) =>
        {
            mainMenuResponse.Enqueue(JsonConvert.DeserializeObject<Dictionary<string, string>>(getData.ToString()));
        });
    }

    public static void whoisHere(Payload in_payload)
    {
        if (in_payload.target == null && !in_payload.source.Equals(UserID))
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload.Add("Username", NetworkMain.Username);
            payload.Add("UserID", NetworkMain.UserID);
            payload.Add("Team", "Survivor");
            payload.Add("Health", EntityManager.players[NetworkMain.Username].getHealth().ToString());
            payload.Add("Action", "Join Acknowledge");
            NetworkMain.replyAction(payload, in_payload.source);
            Debug.Log("Being asked");
        }
    }

    public static void reply(Dictionary<string, string> getPayload, string getTarget)
    {
            Dictionary<string, string> networkPayload = new Dictionary<string, string>();
            networkPayload["source"] = UserID;
            networkPayload["lobbyID"] = NetworkMain.LobbyID;
            networkPayload["data"] = StringUtils.convertPayloadToJson(getPayload);
            networkPayload["target"] = getTarget;
            socket.Emit("Reply", StringUtils.convertPayloadToJson(networkPayload));
    }
    public static void broadcast(Dictionary<string, string> getPayload, string getTarget)
    {
            Dictionary<string, string> networkPayload = new Dictionary<string, string>();
            networkPayload["source"] = UserID;
            networkPayload["lobbyID"] = NetworkMain.LobbyID;
            networkPayload["data"] = StringUtils.convertPayloadToJson(getPayload);
            networkPayload["target"] = getTarget;
            socket.Emit("Broadcast", StringUtils.convertPayloadToJson(networkPayload));
    }
    public static void broadcastOthers(Dictionary<string, string> getPayload, string getTarget)
    {
            Dictionary<string, string> networkPayload = new Dictionary<string, string>();
            networkPayload["source"] = UserID;
            networkPayload["lobbyID"] = NetworkMain.LobbyID;
            networkPayload["data"] = StringUtils.convertPayloadToJson(getPayload);
            networkPayload["target"] = getTarget;
            socket.Emit("Other", StringUtils.convertPayloadToJson(networkPayload));
    }
    public static void serverRequest(Dictionary<string, string> getPayload, string getTarget)
    {
            Dictionary<string, string> networkPayload = new Dictionary<string, string>();
            networkPayload["source"] = UserID;
            networkPayload["lobbyID"] = NetworkMain.LobbyID;
            networkPayload["data"] = StringUtils.convertPayloadToJson(getPayload);
            networkPayload["target"] = getTarget;
            socket.Emit("Server", StringUtils.convertPayloadToJson(networkPayload));
    }

    public static void Login(string in_username, string in_server, string in_action)
    {

        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["Username"] = in_username;
        payload["lobbyID"] = in_server;
        payload["Action"] = in_action;
        socket.Emit("Login", StringUtils.convertPayloadToJson(payload));
    }

    public static void replyAction(Dictionary<string, string> in_payload)
    {
        if (isBroadcastable)
        {
            reply(in_payload, null);
        }
    }
    public static void replyAction(Dictionary<string, string> in_payload, string getTarget)
    {
        if (isBroadcastable)
        {
            reply(in_payload, getTarget);
        }
    }
    public static void broadcastToOther(Dictionary<string, string> in_payload)
    {
        if (isBroadcastable)
        {
            broadcastOthers(in_payload, null);
        }
    }

    public static void broadcastAction(string getString)
    {
        if (isBroadcastable)
        {
            Dictionary<string, string> payload = StringUtils.getPayload();
            payload.Add("Action", getString);
            payload.Add("Type", "Action");
            broadcast(payload, null);
        }
    }

    public static void broadcastAction(string getString, string getTarget)
    {
        if (isBroadcastable)
        {
            Dictionary<string, string> payload = StringUtils.getPayload();
            payload.Add("Action", getString);
            payload.Add("Type", "Action");
            broadcast(payload, getTarget);
        }
    }

    public static void broadcastAction(Dictionary<string, string> in_payload)
    {
        if (isBroadcastable)
        {
            broadcast(in_payload, null);
        }
    }
    public static void broadcastAction(Dictionary<string, string> in_payload, string getTarget)
    {
        if (isBroadcastable)
        {
            broadcast(in_payload, getTarget);
        }
    }

    public static void serverAction(Dictionary<string, string> in_payload)
    {
            serverRequest(in_payload, null);
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

public class PlayerNetworkListener
{
    public string player;
    public IPlayerController controller;
    public Stack<Payload> positionQueue;
    public Stack<Payload> actionQueue;

    public PlayerNetworkListener(string in_player)
    {
        player = in_player;
        positionQueue = new Stack<Payload>();
        actionQueue = new Stack<Payload>();
    }

    public void networkActionListen()
    {
        if (actionQueue.Count > 0)
        {
            controller.serverControl(actionQueue.Pop());
        }
    }

    public void networkPositionListen()
    {
        if (positionQueue.Count > 0)
        {
            controller.serverControl(positionQueue.Pop());
            positionQueue = new Stack<Payload>();
        }
    }
}