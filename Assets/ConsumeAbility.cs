using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeAbility : MonoBehaviour
{
    [SerializeField]
    private BigBoss bb;

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
        if (other.tag.Equals("Resource"))
        {
            bb.isEating = true;
            bb.targetResource = other.transform;
            bb.eatingState(true);
            //op.targetResource = null;
            //Destroy(other);
        }
        if (other.name.Equals("Data"))
        {
            print("Nomnom");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Resource"))
        {
            //bb.isEating = false;
            //bb.targetResource = null;
            //op.targetResource = null;
            //Destroy(other);
            //print("Nomnom no mo");
        }
    }
}
