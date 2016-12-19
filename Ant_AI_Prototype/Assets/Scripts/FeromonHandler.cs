using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeromonHandler : MonoBehaviour {

    [Header("Feromone Grid settings")]
    [SerializeField]
    private int gridwidth;
    [SerializeField]
    private int gridHeight;
    [SerializeField]
    [Tooltip("The space between two nodes.")]
    private float nodeSpacing;
    [SerializeField]
    private Vector2 gridCenter;
    [Header("Feromone deteriation settings")]
    [SerializeField]
    [Tooltip("")]
    private float deteriation;

    private FeromonNode[,] feromoneGrid;

    struct FeromonNode
    {
        public int x;
        public int y;
        public Vector2 worldPos;
        public float feromoneValue;
        public List<FeromonNode> connectedNodes;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void InitializeGrid()
    {
        feromoneGrid = new FeromonNode[gridwidth, gridHeight];
        var halfWidth = (gridwidth * nodeSpacing) * 0.5f;
        var halfHeight = (gridHeight * nodeSpacing) * 0.5f;
        for(int i = 0; i < gridwidth; i++)
        {
            for(int j = 0; j < gridHeight; j++)
            {
                feromoneGrid[i, j].worldPos = new Vector2(gridCenter.x - halfWidth + (i * nodeSpacing), gridCenter.y - halfHeight + (j * nodeSpacing));
                feromoneGrid[i, j].x = i;
                feromoneGrid[i, j].y = j;
                feromoneGrid[i, j].connectedNodes = new List<FeromonNode>();
            }
        }
    }


}
