using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineSelectionResponse : MonoBehaviour, ISelectionResponse
{
    public void OnSelect(Transform selection)
    {
        foreach(Transform child in selection.transform)
        {
            var outline = child.gameObject.GetComponent<Outline>();
            outline.OutlineWidth = 10;
        }
    }

    public void OnDeselect(Transform selection)
    {

        foreach(Transform child in selection.transform)
        {
            var outline = child.transform.GetComponent<Outline>();
            outline.OutlineWidth = 0;
        }
    }
}
