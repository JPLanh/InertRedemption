using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferCenter : MonoBehaviour
{
    public Dictionary<string, int> resources = new Dictionary<string, int>();
    public string team;

    // Start is called before the first frame update
    void Start()
    {
        resources = new Dictionary<string, int>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
