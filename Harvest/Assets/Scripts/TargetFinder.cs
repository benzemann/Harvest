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

    public GameObject Target {
        get
        {
            if (_currentTarget == null)
                FindTarget();
            return _currentTarget;
        }
    }

    public enum TargetType
    {
        Player,
        Ants,
        Tags
    }

    /// <summary>
    /// Find the closest target
    /// </summary>
    private void FindTarget()
    {
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
                _currentTarget = potentialTargets[i];
                closestDistance = distance;
            }
        }
    }

}
