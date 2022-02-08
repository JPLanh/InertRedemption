using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCollision : MonoBehaviour
{

    private Dictionary<string, int> withinRadius = new Dictionary<string, int>();
    public string group = "";

    [SerializeField]
    Node parentNode;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (withinRadius.Count > 0)
        {
            int _high = 0;
            string _group = "";
            foreach (KeyValuePair<string, int> getDict in withinRadius)
            {
                if (_high < getDict.Value)
                {
                    _high = getDict.Value;
                    _group = getDict.Key;
                }
            }

            if (group == "")
            {
                group = _group;
            }
            else if (group != _group && _group != "")
            {
                parentNode.convert(-_high, _group);
            }
            else if (group == _group)
            {
                parentNode.convert(_high, _group);
            }
        }
    }

    //void OnTriggerEnter(Collider col)
    //{
    //    if (col.GetComponent<LivingBeing>())
    //    {
    //        col.GetComponent<LivingBeing>().currentNode = parentNode;
    //        if (!withinRadius.TryGetValue(col.transform.tag, out int ComponentsAmount))
    //        {
    //            withinRadius[col.transform.tag] = 1;
    //        }
    //        else
    //        {
    //            withinRadius[col.transform.tag] += 1;
    //        }
    //    }
    //}

    //void OnTriggerExit(Collider col)
    //{
    //    if (col.GetComponent<LivingBeing>())
    //    {
    //        col.GetComponent<LivingBeing>().currentNode = null;
    //        withinRadius[col.transform.tag] -= 1;

    //    }
    //}
}
