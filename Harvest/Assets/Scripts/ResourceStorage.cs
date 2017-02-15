using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage : MonoBehaviour {

    [SerializeField, Tooltip("The maximum amount of resources this object can have")]
    private int maxStorage;

    private int _currentStorage;

    public int CurrentStorage { get { return _currentStorage; } }
    public int MaxStorage { get { return maxStorage; } }
    public int RemainingStorageSpace { get { return maxStorage - _currentStorage; } }
    public float Percentage { get { return ((float)_currentStorage / (float)maxStorage); } }

    public bool StoreResource(int amount)
    {
        int newStorage = _currentStorage + amount;

        _currentStorage = Mathf.Min(newStorage, maxStorage);

        if (_currentStorage == maxStorage)
        {
            return false;
        } else
        {
            return true;
        }

    }

}
