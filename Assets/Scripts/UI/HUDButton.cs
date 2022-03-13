using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDButton : MonoBehaviour
{
    public string action = "";
    public string hint;
    public GameObject subMenu;
    private subDisplayMenu display_cache; 
    public Text buttonText;
    public Text hintText;

    private void Start()
    {
    }

    public void onClick()
    { 
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["Type"] = "Player Update";
        payload["Action"] = "Player HUD Menu";
        payload["Team"] = NetworkMain.Team;
        payload["Menu"] = action;
        NetworkMain.broadcastAction(payload);
    }



    public void buttonHooverEnter()
    {
        if (hintText != null)
        hintText.text = hint;
        if (subMenu != null) Destroy(subMenu);
        subMenu = Instantiate(Resources.Load<GameObject>("UI/Display Info"), transform.parent);
        subMenu.TryGetComponent<subDisplayMenu>(out subDisplayMenu out_subDisplay);
        out_subDisplay.textField.text = hint;
    }

    public void buttonHooverExit()
    {
        if (hintText != null)
            hintText.text = "";
        if (subMenu != null) Destroy(subMenu);

    }

}
