using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeromoneManager : MonoBehaviour
{
    [SerializeField, Tooltip("The width of the feromone grid (number of nodes)")]
    private int gridWidth;
    [SerializeField, Tooltip("The height of the feromone grid (number of nodes)")]
    private int gridHeight;
    [SerializeField, Tooltip("The spacing between each feromone node")]
    private float nodeSpacing;
    [SerializeField, Tooltip("The prefab of the feromone trail")]
    private GameObject feromoneTrail;

    private FeromoneNode[,] feromoneGrid;
    private ObjectPool feromoneTrailPool;

    private static FeromoneManager _instance;
    public static FeromoneManager Instance { get { return _instance; } }

    struct int2
    {
        public int2 (int _x, int _y)
        {
            x = _x;
            y = _y;
        }
        public int x;
        public int y;
    }

    struct FeromoneNode
    {
        public FeromoneNode(int _x, int _y, Vector2 c, float fv)
        {
            x = _x;
            y = _y;
            center = c;
            feromoneValue = fv;
            connections = new List<int2>();
            feromoneTrail = null;
        }
        public int x;
        public int y;
        public Vector2 center;
        public float feromoneValue;
        public List<int2> connections;
        public GameObject feromoneTrail;
    }



    // Use this for initialization
    void Awake()
    {
        // Set instance
        _instance = this;
        // Instantiate feromone trail pool
        feromoneTrailPool = new ObjectPool(feromoneTrail, 5, true);
        // Initialize grid
        feromoneGrid = new FeromoneNode[gridWidth, gridHeight];
        var halfWidth = (gridWidth * nodeSpacing) * 0.5f;
        var halfHeight = (gridHeight * nodeSpacing) * 0.5f;
        var gridCenter = this.transform.position;
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                feromoneGrid[i, j].center = new Vector2(
                    gridCenter.x - halfWidth + (i * nodeSpacing + 0.5f), 
                    gridCenter.z - halfHeight + (j * nodeSpacing + 0.5f)
                    );
                feromoneGrid[i, j].x = i;
                feromoneGrid[i, j].y = j;
                feromoneGrid[i, j].connections = new List<int2>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void WorldToGridCoords(Vector3 pos, out int x, out int y)
    {
        x = -1;
        y = -1;
        var halfWidth = (gridWidth * nodeSpacing) * 0.5f;
        var halfHeight = (gridHeight * nodeSpacing) * 0.5f;
        var gridCenter = this.transform.position;

        // Check if position is outside the feromone trail
        if(pos.x < gridCenter.x - halfWidth || pos.x > gridCenter.x + halfWidth ||
            pos.y < gridCenter.z - halfHeight || pos.y > gridCenter.z + halfHeight)
        {
            return;
        }

        var point00 = gridCenter - new Vector3(halfWidth, 0f, halfHeight);

        var tmp = pos - point00;
        x = (int)(tmp.x / nodeSpacing);
        y = (int)(tmp.z / nodeSpacing);
    }

    public void AddFeromoneValue(Vector3 pos, float value, int oldX, int oldY)
    {
        int x, y;
        WorldToGridCoords(pos, out x, out y);
        if (x == -1 || y == -1)
            return;

        feromoneGrid[x, y].feromoneValue += value;
        if (oldX < 0 || oldX >= gridWidth || oldY < 0 || oldY >= gridHeight)
            return;
        var newDir = new int2(oldX, oldY);
        if (!feromoneGrid[x, y].connections.Contains(newDir))
        {
            feromoneGrid[x, y].connections.Add(new int2(oldX, oldY));
            AddFeromoneParticles(x, y, oldX, oldY);
        }
    }

    public void AddFeromoneValue(int x, int y, float value, int oldX, int oldY)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
            return;

        feromoneGrid[x, y].feromoneValue += value;
        if (oldX < 0 || oldX >= gridWidth || oldY < 0 || oldY >= gridHeight)
            return;
        var newDir = new int2(oldX, oldY);
        if (!feromoneGrid[x, y].connections.Contains(newDir))
        {
            feromoneGrid[x, y].connections.Add(new int2(oldX, oldY));
            AddFeromoneParticles(x, y, oldX, oldY);
        }
    }

    public void SetFeromoneValue(Vector3 pos, float value, int oldX, int oldY)
    {
        int x, y;
        WorldToGridCoords(pos, out x, out y);
        if (x == -1 || y == -1)
            return;

        feromoneGrid[x, y].feromoneValue = value;
        if (oldX < 0 || oldX >= gridWidth || oldY < 0 || oldY >= gridHeight || value == 0f)
            return;
        var newDir = new int2(oldX, oldY);
        if (!feromoneGrid[x, y].connections.Contains(newDir))
        {
            feromoneGrid[x, y].connections.Add(new int2(oldX, oldY));
            AddFeromoneParticles(x, y, oldX, oldY);
        }
    }

    public void SetFeromoneValue(int x, int y, float value, int oldX, int oldY)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
            return;

        feromoneGrid[x, y].feromoneValue = value;
        if (oldX < 0 || oldX >= gridWidth || oldY < 0 || oldY >= gridHeight || value == 0f)
            return;
        var newDir = new int2(oldX, oldY);
        if (!feromoneGrid[x, y].connections.Contains(newDir))
        {
            feromoneGrid[x, y].connections.Add(new int2(oldX, oldY));
            AddFeromoneParticles(x, y, oldX, oldY);
        }
        
    }

    private void AddFeromoneParticles(int startX, int startY, int endX, int endY)
    {

        GameObject trail = feromoneTrailPool.GetPooledObject();
        trail.SetActive(true);
        trail.transform.position = new Vector3(feromoneGrid[startX, startY].center.x, 0.1f, feromoneGrid[startX,startY].center.y);
        var v = new Vector3(feromoneGrid[endX, endY].center.x, 0.1f, feromoneGrid[endX, endY].center.y) - trail.transform.position;
        var r = Quaternion.FromToRotation(Vector3.right, v);
        trail.transform.rotation = r;
        feromoneGrid[startX, startY].feromoneTrail = trail;

    }

    void OnDrawGizmosSelected()
    {
        var halfWidth = (gridWidth * nodeSpacing) * 0.5f;
        var halfHeight = (gridHeight * nodeSpacing) * 0.5f;
        var gridCenter = this.transform.position;
        Vector3 point00 = new Vector3(gridCenter.x - halfWidth, gridCenter.y, gridCenter.z - halfHeight);
        Vector3 point10 = new Vector3(gridCenter.x + halfWidth, gridCenter.y, gridCenter.z - halfHeight);
        Vector3 point01 = new Vector3(gridCenter.x - halfWidth, gridCenter.y, gridCenter.z + halfHeight);
        Vector3 point11 = new Vector3(gridCenter.x + halfWidth, gridCenter.y, gridCenter.z + halfHeight);

        Gizmos.color = Color.green;

        Gizmos.DrawLine(point00, point01);
        Gizmos.DrawLine(point00, point10);
        Gizmos.DrawLine(point10, point11);
        Gizmos.DrawLine(point01, point11);

        for(int i = 0; i < gridWidth; i++)
        {
            var p = new Vector3(point00.x + (i * nodeSpacing), point00.y, point00.z);
            var p2 = new Vector3(point01.x + (i * nodeSpacing), point01.y, point01.z);
            Gizmos.DrawLine(p, p2);
        }

        for (int i = 0; i < gridHeight; i++)
        {
            var p = new Vector3(point00.x, point00.y, point00.z + (i * nodeSpacing));
            var p2 = new Vector3(point10.x, point10.y, point10.z + (i * nodeSpacing));
            Gizmos.DrawLine(p, p2);
        }


    }
}
