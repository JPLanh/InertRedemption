using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Qube : MonoBehaviour, ISelectionResponse, Interactable
{
    private Dictionary<string, int> resource;
    public Pod station;
    private int progress = 0;

    // Start is called before the first frame update
    void Start()
    {
        resource = new Dictionary<string, int>();
        resource["Credit"] = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(0).transform.eulerAngles += new Vector3(0, .75f, 0);
        if (station != null)
        {
            if (!station.type.Equals("Credit"))
            {
                if (resource["Credit"] > 0)
                {
                    progress++;
                    if (progress == 100)
                    {
                        if (!resource.TryGetValue(station.type, out int amount))
                        {
                            resource[station.type] = 0;
                        }
                        resource[station.type] += 2;
                        progress = 0;
                        resource["Credit"] -= 1;
                    }
                }
            } else
            {
                progress++;
                if (progress == 100)
                {
                    resource[station.type] += 1;
                    progress = 0;
                }

            }
        }
    }


    public void OnSelect(Transform selection)
    {
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.GetComponent<Text>().text = printDictionary("Resource", resource);
    }

    public void OnDeselect(Transform selection)
    {
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
    }

    public void Interact(PlayerController player)
    {
            gameObject.SetActive(false);
    }

    private string printDictionary(string dictName, Dictionary<string, int> getDict)
    {
        string getString = "";
        foreach (KeyValuePair<string, int> getpay in getDict)
        {
            getString += getpay.Key + " = " + getpay.Value + "\n";
        }
        return getString;
    }

}
