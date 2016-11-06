using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

    public float maxHealth;
    public float maxAlpha;
    public float minAlpha;
    public float secToRepair;
    float health;
    bool disabled = true;
    float lastHit;
    bool ready = false;


    void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

	void Update()
    {
        if (!disabled)
        {
            Color c = GetComponent<Renderer>().material.color;
            c.a = (health / maxHealth) * (maxAlpha - minAlpha) + minAlpha;
            GetComponent<Renderer>().material.color = c;
        }
        if(health < maxHealth && ready)
        {
            if(Time.time - lastHit > secToRepair)
            {
                health = maxHealth;
                disabled = false;
                GetComponent<Renderer>().enabled = true;
            }
        }

    }
    
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Ants")
        {
            if (col.GetComponent<WarriorAnt>() != null)
                col.GetComponent<WarriorAnt>().EnterShield(this.gameObject);
            else
                col.GetComponent<Ant>().GoHome();
        }
    }

    public void Damage(float d)
    {
        health -= d;
        lastHit = Time.time;
        if(health <= 0)
        {
            disabled = true;
            GetComponent<Renderer>().enabled = false;
        }
    }

    public bool IsDisabled()
    {
        return disabled;
    }
    
    public void Enable()
    {
        lastHit = Time.time;
        ready = true;
    }

    public void AddHealth(float h)
    {
        health += h;
    }
}
