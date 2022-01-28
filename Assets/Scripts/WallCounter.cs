using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCounter : MonoBehaviour
{
    [SerializeField] private Material on;
    [SerializeField] private Material off;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void turnOn()
    {
        gameObject.GetComponent<MeshRenderer>().material = on;
    }

    public void turnOff()
    {
        gameObject.GetComponent<MeshRenderer>().material = off;
    }


}
