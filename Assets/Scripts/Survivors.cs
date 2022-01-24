using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survivors : MonoBehaviour
{
    public compass survivorCompass;
    public GameObject survivorList;
    public GameObject buildingList;
    public GameObject spaceshipList;
    public Transform landing_zone;
    public Spaceship spaceship;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addMarkedTarget(GameObject target)
    {
        if(target.TryGetComponent<TargetMarker>(out TargetMarker markedTarget)){
            survivorCompass.addTarget(markedTarget);
        }
    }

    public void removeMarkedTarget(GameObject target)
    {
        if (target.TryGetComponent<TargetMarker>(out TargetMarker markedTarget))
        {
            survivorCompass.removeTarget(markedTarget);
        }
    }
}
