using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchButton : MonoBehaviour, Interactable
{

    public Spaceship ship;

    public void Interact(PlayerController player)
    {
        bool readyForLaunch = true;
        foreach(KeyValuePair<string, int> it_Requirement in ship.requirement)
        {
            Debug.Log($"{it_Requirement.Key}: {ship.resources[it_Requirement.Key]} / {it_Requirement.Value}");
            if (it_Requirement.Value > ship.resources[it_Requirement.Key])
            {
                readyForLaunch = false;
            }
        }
        if (readyForLaunch)
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["Type"] = "Player Action";
            payload["Action"] = "Survivors Win";
            NetworkMain.broadcastAction(payload);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
