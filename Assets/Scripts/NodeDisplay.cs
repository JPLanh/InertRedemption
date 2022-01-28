using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeDisplay : MonoBehaviour, Displayable
{
    [SerializeField]
    private Node node;
    [SerializeField]
    private NodeCollision nodeDetector;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public string display()
    {
       
        string disp = node.gameObject.name + " \n";
        disp += StringUtils.printDictionary(node.transferCenter.resources);

        //if (node.conversion != 0 && node.conversion != 100 && nodeDetector.group != null)
        //{
        //    disp += "Converting: \n" +
        //            " -Team: " + nodeDetector.group + "\n" +
        //            " -Process: " + StringUtils.convertFloatToString(node.conversion) + "\n";
        //}

        return disp;
    }
}
