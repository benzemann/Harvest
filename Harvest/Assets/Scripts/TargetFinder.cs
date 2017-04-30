using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour {

    [SerializeField, Tooltip("The TargetFinder will find the closest gameobject of this type (tag is finds all gameobject with specific tag)")]
    private TargetType targetType;
    [SerializeField, Tooltip("The tag of the target. Only used if target type is 'tag'")]
    private string targetTag;
    [SerializeField, Tooltip("The radius it will look for enemies")]
    private float searchRadius;
    
    private GameObject _currentTarget;

    private bool hasSearchedInThisFrame;

    public GameObject Target {
        get
        {
            if (_currentTarget == null && !hasSearchedInThisFrame)
                FindTarget();
            return _currentTarget;
        }
    }

    public void Update()
    {
        if(_currentTarget != null && GetComponent<Vision>() != null)
        {
            if (!GetComponent<Vision>().CanISeeIt(_currentTarget))
            {
                _currentTarget = null;
            }
        }
    }

    public void LateUpdate()
    {
        hasSearchedInThisFrame = false;
    }

    public enum TargetType
    {
        Player,
        Ants,
        Tags
    }

    public void ClearTarget()
    {
        _currentTarget = null;
        hasSearchedInThisFrame = true;
    }

    /// <summary>
    /// Find the closest target
    /// </summary>
    private void FindTarget()
    {
        hasSearchedInThisFrame = true;
        // Get all potential targets
        GameObject[] potentialTargets = new GameObject[0];
        switch (targetType)
        {
            case TargetType.Player:
                potentialTargets = ObjectManager.Instance.AllPlayerObjects;
                break;
            case TargetType.Ants:
                potentialTargets = ObjectManager.Instance.AllAnts;
                break;
            case TargetType.Tags:
                potentialTargets = GameObject.FindGameObjectsWithTag(targetTag);
                break;
            default:
                break;
        }
        
        // Find the closest target
        float closestDistance = float.MaxValue;
        for(int i = 0; i < potentialTargets.Length; i++)
        {
            float distance = Vector3.Distance(this.transform.position, potentialTargets[i].transform.position);
            if(distance < closestDistance && distance <= searchRadius)
            {
                // Handle vision
                if (GetComponent<Vision>() != null && !GetComponent<Vision>().CanISeeIt(potentialTargets[i]))
                    continue;

                _currentTarget = potentialTargets[i];
                closestDistance = distance;
            }
        }
    }

}
