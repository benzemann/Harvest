using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableManager : Singleton<SelectableManager> {

    private List<Selectable> selectables;

    private void Awake()
    {
        selectables = new List<Selectable>();
    }

    private void Update()
    {
        // Check if mouse is up
        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)){
                if(hit.transform.gameObject != null)
                {
                    if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        DeSelectAll();
                    } 
                }
            }
        }
    }

    /// <summary>
    /// Adds a selectable to the list of all selectables.
    /// </summary>
    /// <param name="s">The selectable which will be added</param>
	public void AddSelectable(Selectable s)
    {
        selectables.Add(s);
    }

    /// <summary>
    /// Get the selected object. This assumes that there is only one selected object!
    /// </summary>
    /// <returns>The selected object</returns>
    public Selectable GetSelectedObject()
    {
        foreach (var selectable in selectables)
            if (selectable.IsSelected)
                return selectable;
        return null;
    }

    /// <summary>
    /// Get all selected objects
    /// </summary>
    /// <returns>Returns a list of all selected objects</returns>
    public List<Selectable> GetSelectedObjects()
    {
        List<Selectable> selection = new List<Selectable>();
        foreach (var selectable in selectables)
            if (selectable.IsSelected)
                selection.Add(selectable);
        return selection;
    }

    /// <summary>
    /// Deselects all selectables
    /// </summary>
    public void DeSelectAll()
    {
        for(int i = 0; i < selectables.Count; i++)
        {
            selectables[i].IsSelected = false;
        }
    }

    /// <summary>
    /// Selects an object and deselect others if needed.
    /// </summary>
    /// <param name="s">The object that should be selected.</param>
    /// <param name="deSelectOthers">Defines whether or not other objects should be deselected.</param>
    public void SelectObject(Selectable s, bool deSelectOthers = true)
    {
        // Deselect all other objects
        if (deSelectOthers)
        {
            foreach (var selectable in selectables)
            {
                selectable.IsSelected = false;
            }
        }
        // Select object
        s.IsSelected = true;
    }
}
