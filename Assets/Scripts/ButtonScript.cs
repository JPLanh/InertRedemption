using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour, Interactable
{
    public string action;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttonClick()
    {
        NetworkMain.broadcastAction(action);
    }

    public void Interact(PlayerController player)
    {
        if (NetworkMain.Username.Equals(player.name))
            buttonClick();
    }
}
