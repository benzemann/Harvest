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

    public int TakeResource(int amount)
    {
        var amountToBeTaken = Mathf.Min(amount, _currentStorage);
        _currentStorage -= amountToBeTaken;
        return amountToBeTaken;
    }

    public bool StoreResource(int amount, out int remainder)
    {
        remainder = 0;
        int newStorage = _currentStorage + amount;

        _currentStorage = Mathf.Min(newStorage, maxStorage);

        if(newStorage > maxStorage)
        {
            remainder = newStorage - maxStorage;
        }

        if (_currentStorage == maxStorage)
        {
            return false;
        } else
        {
            return true;
        }

    }

}
