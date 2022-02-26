using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survivors : MonoBehaviour
{
    public Compass survivorCompass;
    public GameObject survivorList;
    public GameObject buildingList;
    public GameObject spaceshipList;
    public Transform landing_zone;
    public Spaceship spaceship;


    public List<WallCounter> disinfectCounter;
    private int index = 2;
    public Transform ship;
    public Collider shipCollider;

    public void Interact(PlayerController player)
    {
    }
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

    public void disinfectShip()
    {
        if (index >= 0)
        {
            disinfectCounter[index].turnOff();
            index--;
            Vector3 shipSize = new Vector3(shipCollider.bounds.size.x, shipCollider.bounds.size.y, shipCollider.bounds.size.z);
            Collider[] lv_collided = Physics.OverlapBox(shipCollider.transform.position, shipSize);
            Debug.Log($"Counter: {lv_collided.Length}, Size: {shipSize}");
            foreach (Collider it_collider in lv_collided)
            {
                if (it_collider.tag.Equals("Viruses") && !it_collider.name.Equals("Collision Blocker"))
                    Debug.Log("Virus Detected, Game is over");
            }
        }
    }
}
