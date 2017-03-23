using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolsManager : Singleton<ObjectPoolsManager> {
    [SerializeField]
    private GameObject groundClickPrefab;
    [SerializeField]
    private GameObject phasmaBulletPrefab;
    [SerializeField]
    private GameObject phasmaHitPrefab;

    public ObjectPool groundClickPool;
    public ObjectPool phasmaBulletPool;
    public ObjectPool phasmaHitPool;

    private void Awake()
    {
        groundClickPool = new ObjectPool(groundClickPrefab, 0, true);
        phasmaBulletPool = new ObjectPool(phasmaBulletPrefab, 0, true);
        phasmaHitPool = new ObjectPool(phasmaHitPrefab, 0, true);
    }
}
