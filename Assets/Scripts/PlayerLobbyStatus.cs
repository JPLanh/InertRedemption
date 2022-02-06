using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class PlayerLobbyStatus : MonoBehaviour
{

    public Text readyText;
    public Image readyCheckImage;
    public Text playerName;
    public bool ready;
    public string team;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init(string in_username, string in_team)
    {
        playerName.text = in_username;
        readyText.text = "Not ready";
        team = in_team;
        readyCheckImage.color = new Color(1f, 0f, 0f, 1f);
    }

    public void readyCheckToggle()
    {
        ready = ready ? false : true;
        if (ready)
        {
            readyText.text = "Ready";
            readyCheckImage.color = new Color(0f, 1f, 0f, 1f);
        } else
        {
            readyText.text = "Not ready";
            readyCheckImage.color = new Color(1f, 0f, 0f, 1f);
        }
    }
}
