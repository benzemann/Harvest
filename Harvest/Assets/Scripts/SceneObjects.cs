using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjects : Singleton<SceneObjects> {
    [SerializeField]
    private Canvas mainCanvas;

    public Canvas MainCanvas { get { return mainCanvas; } }
}
