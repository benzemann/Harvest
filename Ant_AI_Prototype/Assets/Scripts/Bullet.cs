using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    GameObject target;
    public float speed;
    float damage;
    public void Seek(GameObject t, float d)
    {
        target = t;
        damage = d;
    }

	// Update is called once per frame
	void Update () {
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        Vector3 dir = target.transform.position - transform.position;
        float disThisFrame = speed * Time.deltaTime;

        if(dir.magnitude <= disThisFrame)
        {
            HitTarget();
            Destroy(this.gameObject);
            return;
        }

        transform.Translate(dir.normalized * disThisFrame, Space.World);
	}

    void HitTarget()
    {
        if(target.GetComponent<Ant>() != null)
        {
            target.GetComponent<Ant>().Damage(damage);
        }
        else if (target.GetComponent<WarriorAnt>() != null)
        {
            target.GetComponent<WarriorAnt>().Damage(damage);
        }
    }


}
