using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDButton : MonoBehaviour
{
    public string action = "";
    public Text buttonText;

    public void onClick()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["Type"] = "Player Update";
        payload["Action"] = "Player HUD Menu";
        payload["Menu"] = action;
        NetworkMain.broadcastAction(payload);
    }
}
