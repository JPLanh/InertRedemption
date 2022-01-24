using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public float reachDistance = 5;
    [SerializeField] private string selectableTag = "Selectable";

    private ISelectionResponse _selectionResponse;

    private Transform _selection;

    private void Awake()
    {
        _selectionResponse = GetComponent<ISelectionResponse>();
    }

    // Update is called once per frame
    void Update()
    {

            if (_selection != null)
        {
            _selectionResponse.OnDeselect(_selection);
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        _selection = null;
        if (Physics.Raycast(ray, out var hit, reachDistance))
        {
            var selection = hit.transform;
            if (selection.CompareTag(selectableTag))
            {
                _selection = selection;
                _selectionResponse = _selection.GetComponent<ISelectionResponse>();
            }
        }
        if (_selection != null)
        {
            _selectionResponse.OnSelect(_selection);
        }
    }

}