using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionScript : MonoBehaviour
{

    public VirusController currentVirus;
    public PlayerController currentPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void infect(VirusController in_virus)
    {
        currentVirus = in_virus;
        currentVirus.getHost(currentPlayer);
        NetworkMain.broadcastAction("Infect", currentPlayer.name);
    }
}
