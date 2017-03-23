using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamager : Damager {

    [SerializeField]
    private Transform barrelExit;
    [SerializeField]
    private float bulletSpeed;
    [SerializeField, Tooltip("The bullet shoots towards the target with a random offset of this in degrees")]
    private float accuracy;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (IsTargetClose())
        {
            if (!RotateTowardsTarget())
                return;

            if (IsReady())
            {
                var target = GetComponent<TargetFinder>().Target;
                Shoot();
            }
        }
	}

    void Shoot()
    {
        var bullet = ObjectPoolsManager.Instance.phasmaBulletPool.GetPooledObject();
        bullet.SetActive(true);
        bullet.transform.position = barrelExit.position;
        bullet.GetComponent<Bullet>().Speed = bulletSpeed;
        bullet.GetComponent<Bullet>().Damage = damage;
        bullet.transform.LookAt(GetComponent<TargetFinder>().Target.transform, Vector3.up);
        bullet.transform.Rotate(Random.Range(-accuracy, accuracy), Random.Range(-accuracy,accuracy), 0f, Space.Self);
    }
}
