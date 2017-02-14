﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAntIdle : State {

    private GameObject ant;

    public WarriorAntIdle(GameObject a)
    {
        ant = a;
        id = StateID.IdleID;
    }

    public override void Enter()
    {
        //Debug.Log("Enter idle");
    }

    public override void Execute()
    {
    }

    public override StateID Reason()
    {
        if (ant.GetComponent<TargetFinder>().Target != null)
        {
            return StateID.PursuitID;
        }
        return StateID.ScoutID;
    }

    public override void Exit()
    {
        //Debug.Log("Exit idle");
    }

}

public class WarriorAntScout : State
{

    private GameObject ant;
    private bool goToIdle = false;
    private Vector3 obstacleNormal;

    public WarriorAntScout(GameObject a)
    {
        ant = a;
        id = StateID.ScoutID;
    }

    public override void Enter()
    {
        //Debug.Log("Enter scout");
        GoInDirection(ant.transform.forward, 45f);
        
    }

    public override void Execute()
    {
        if (!ant.GetComponent<AgentController>().IsWalking && obstacleNormal != Vector3.zero
            && !ant.GetComponent<AgentController>().IsSearchingForPath())
        {
            GoInDirection(obstacleNormal, 45);
        }
    }

    public override StateID Reason()
    {

        if (ant.GetComponent<TargetFinder>().Target != null)
        {
            return StateID.PursuitID;
        }

        if (goToIdle)
        {
            goToIdle = false;
            return StateID.IdleID;
        }
        
        return base.Reason();
    }

    public override void Exit()
    {
        //Debug.Log("Exit scout");
    }

    /// <summary>
    /// Go to in the specified direction until hidding an obstacle.
    /// </summary>
    /// <param name="dir"> The direction</param>
    /// <param name="randomDeviation">A random deviation in degrees ( -randomDeviation to randomDeviation ) </param>
    private void GoInDirection(Vector3 dir, float randomDeviation)
    {
        var d = new Vector3(dir.x, 0f, dir.z);
        Vector3 ranDir = Quaternion.Euler(0, Random.Range(-randomDeviation, randomDeviation), 0) * d.normalized;
        RaycastHit hit;
        Ray ray = new Ray(ant.transform.position + Vector3.up * 0.5f, ranDir);
        LayerMask layerMask = 1 << LayerMask.NameToLayer("Obstacle");
        if (Physics.Raycast(ray, out hit, 1000f, layerMask))
        {
            obstacleNormal = hit.normal;
            ant.GetComponent<AgentController>().GoToPos(hit.point);
        }
        else
        {
            goToIdle = true;
        }
    }

}

public class WarriorAntPursuit : State
{

    private GameObject ant;
    private GameObject target;
    private Vector3 targetOldPos;

    public WarriorAntPursuit(GameObject a)
    {
        ant = a;
        id = StateID.PursuitID;
    }

    public override void Enter()
    {
        target = ant.GetComponent<TargetFinder>().Target;
        if(target != null)
        {
            ant.GetComponent<AgentController>().GoToPos(target.transform.position);
            targetOldPos = target.transform.position;
        }
    }

    public override void Execute()
    {
        if (target == null)
            return;
        if(Vector3.Distance(target.transform.position, targetOldPos) > ant.GetComponent<MeleeDamager>().GetAttackRange())
        {
            ant.GetComponent<AgentController>().GoToPos(target.transform.position);
            targetOldPos = target.transform.position;
        }
    }

    public override StateID Reason()
    {
        if (target == null)
            return StateID.IdleID;

        if (Vector3.Distance(ant.transform.position, target.transform.position) <=
            ant.GetComponent<MeleeDamager>().GetAttackRange())
            return StateID.AttackID;

        return base.Reason();
    }

    public override void Exit()
    {

    }
}

public class WarriorAntAttack : State
{

    private GameObject ant;
    private GameObject target;
    private MeleeDamager damager;
    private float timeAtLastAttack;

    public WarriorAntAttack(GameObject a)
    {
        ant = a;
        id = StateID.AttackID;
    }

    public override void Enter()
    {
        target = ant.GetComponent<TargetFinder>().Target;
        ant.GetComponent<AgentController>().Stop();
        ant.GetComponent<AgentController>().AvoidPushing();
        damager = ant.GetComponent<MeleeDamager>();
        timeAtLastAttack = Time.time;
        if (damager == null)
            Debug.LogError("The ant needs a damager component!");
    }

    public override void Execute()
    {
        if (target == null)
            return;
        float step = 5f * Time.deltaTime;
        Vector3 targetDir = target.transform.position - ant.transform.position;
        targetDir = new Vector3(targetDir.x, ant.transform.position.y, targetDir.z);
        Vector3 newDir = Vector3.RotateTowards(ant.transform.forward, targetDir, step, 0.0f);
        ant.transform.rotation = Quaternion.LookRotation(newDir);
        if(Time.time - timeAtLastAttack > damager.GetAttackSpeed())
        {
            timeAtLastAttack = Time.time;
            damager.Damage(target.GetComponent<Health>());
        }
    }

    public override StateID Reason()
    {
        if (target == null)
            return StateID.IdleID;

        if (Vector3.Distance(ant.transform.position, target.transform.position) >
            ant.GetComponent<MeleeDamager>().GetAttackRange())
            return StateID.PursuitID;

        return base.Reason();
    }

    public override void Exit()
    {
        ant.GetComponent<AgentController>().ReleaseGroundNodes();
    }
}

