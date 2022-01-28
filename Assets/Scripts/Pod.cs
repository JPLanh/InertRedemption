using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pod : MonoBehaviour, ISelectionResponse, Interactable
{
    public string type;

    public void OnSelect(Transform selection)
    {
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.GetComponent<Text>().text = type;
    }

    public void OnDeselect(Transform selection)
    {
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
    }

    public void Interact(PlayerController player)
    {
        //if (GameObject.Find("Player").GetComponent<PlayerController>().carrying != null)
        //{
        //    Qube carrying = GameObject.Find("Player").GetComponent<PlayerController>().carrying;
        //    carrying.gameObject.SetActive(true);
        //    carrying.transform.SetParent(this.transform);
        //    carrying.station = this;
        //    carrying.transform.position = this.transform.position + new Vector3(0f, 3f, 0f);
        //    GameObject.Find("Player").GetComponent<PlayerController>().carrying = null;
        //}

    }
}
