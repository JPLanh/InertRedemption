using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour, IFeet
{
    [SerializeField]
    private string partName;

    public string getName()
    {
        return partName;
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
