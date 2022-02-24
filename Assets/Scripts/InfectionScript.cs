using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionScript : MonoBehaviour
{

    public VirusController currentVirus;
    public PlayerController currentPlayer;
    public Resource currentResource;

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
        if (in_virus.name.Equals(NetworkMain.Username)) currentVirus.getHost(currentPlayer);
        currentVirus.attachToHost(currentPlayer);
    }

    public void infectResource(VirusController in_virus)
    {
        currentVirus = in_virus;
        currentVirus.attachToHost(currentResource);
    }
}
