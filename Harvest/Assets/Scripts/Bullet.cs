using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour {
    
    public float Speed { get; set; }
    public float Damage { get; set; }

	// Update is called once per frame
	void Update () {
        this.transform.Translate(this.transform.forward * Speed * Time.deltaTime, Space.World);
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<WarriorAntAI>() != null)
        {
            other.gameObject.GetComponent<Health>().Damage(10.0f);
            var bulletHit = ObjectPoolsManager.Instance.phasmaHitPool.GetPooledObject();
            bulletHit.SetActive(true);
            bulletHit.transform.position = this.transform.position;
            this.gameObject.SetActive(false);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            var bulletHit = ObjectPoolsManager.Instance.phasmaHitPool.GetPooledObject();
            bulletHit.SetActive(true);
            bulletHit.transform.position = this.transform.position;
            this.gameObject.SetActive(false);
        }
    }
}
