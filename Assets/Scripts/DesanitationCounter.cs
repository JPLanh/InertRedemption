using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesanitationCounter : MonoBehaviour, Interactable
{

    public List<GameObject> counters;

    public void Interact(PlayerController player)
    {
        Debug.Log("Player clicked teh button");
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
