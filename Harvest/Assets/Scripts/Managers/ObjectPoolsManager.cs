using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolsManager : Singleton<ObjectPoolsManager> {
    [SerializeField]
    private GameObject groundClickPrefab;
    [SerializeField]
    private GameObject phasmaBulletPrefab;
    [SerializeField]
    private GameObject phasmaGroundHitPrefab;
    [SerializeField]
    private GameObject phasmaAntHitPrefab;

    public ObjectPool groundClickPool;
    public ObjectPool phasmaBulletPool;
    public ObjectPool phasmaGroundHitPool;
    public ObjectPool phasmaAntHitPool;

    private void Awake()
    {
        groundClickPool = new ObjectPool(groundClickPrefab, 0, true);
        phasmaBulletPool = new ObjectPool(phasmaBulletPrefab, 0, true);
        phasmaGroundHitPool = new ObjectPool(phasmaGroundHitPrefab, 0, true);
        phasmaAntHitPool = new ObjectPool(phasmaAntHitPrefab, 0, true);
    }
}
