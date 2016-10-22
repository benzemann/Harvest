using UnityEngine;
using System.Collections;

public class Revealer : MonoBehaviour {

    public int radius;

    private void Start()
    {
        if (FogOfWar.Instance == null)
            return;
        FogOfWar.Instance.RegisterRevealer(this);
    }
}
