using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabScript : MonoBehaviour
{
    [SerializeField]
    PlayerController getController;

    // Start is called before the first frame update


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("Ledge"))
        {
            //if (getController.moveDirection.y > 1 || getController.moveDirection.y < -1)
                //getController.grabLedge(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Equals("Ledge"))
        {
                getController.onLedge = false;
        }
    }
}
