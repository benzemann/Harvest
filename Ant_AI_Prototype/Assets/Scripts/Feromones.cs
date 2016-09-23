using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class Feromones : MonoBehaviour {

    public enum DeteriationType { Linear, Percentage, TimeDependent };
    public enum ChoosePathType { ValuePercentage, SortedPercentage };

    public float maxValue;
    public DeteriationType deteriationType;
    public float deteriationPrSec;
    public float percentagePrSec;
    public float minValueForPercentage;
    public float timeUntilDeteriation;
    ChoosePathType choosePathType = ChoosePathType.ValuePercentage;
    int[] sortedPercentages = new int[3];
    public Font textFont;
    public int fontSize;
    GameObject ground;

    Texture2D feromoneTex;
    Node[,] feromoneGrid;
    GameObject[,] feromoneText;
    GameObject canvas;

    float deteriationTimer;

    struct Node
    {
        public int x;
        public int y;
        public float feromoneValue;
        public List<Vector2> connectedNodes;
        public float timeStampOnLastAdd;
    }

    void Update()
    {
        deteriationTimer += Time.deltaTime;
        if(deteriationTimer >= 1.0f)
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    if(deteriationType == DeteriationType.Linear)
                    {
                        feromoneGrid[i, j].feromoneValue -= deteriationPrSec;
                    } else if (deteriationType == DeteriationType.Percentage)
                    {
                        feromoneGrid[i, j].feromoneValue *= (100.0f - percentagePrSec) / 100.0f;
                        if(feromoneGrid[i, j].feromoneValue <= minValueForPercentage)
                        {
                            feromoneGrid[i, j].feromoneValue = 0.0f;
                        }
                    } else if(deteriationType == DeteriationType.TimeDependent)
                    {
                        if(Time.time - feromoneGrid[i, j].timeStampOnLastAdd > timeUntilDeteriation)
                        {
                            feromoneGrid[i, j].feromoneValue -= deteriationPrSec;
                        }
                    }
                    
                    if(feromoneGrid[i, j].feromoneValue < 0)
                    {
                        feromoneGrid[i, j].feromoneValue = 0.0f;
                        feromoneGrid[i, j].connectedNodes.Clear();
                    }
                    deteriationTimer = 0.0f;
                    feromoneTex.SetPixel(i, j, new Color(0.0f, (float)feromoneGrid[i, j].feromoneValue / maxValue, 0.0f));
                    if(feromoneGrid[i, j].feromoneValue > 0.0f)
                    {
                        feromoneText[i, j].GetComponent<Text>().enabled = true;
                        feromoneText[i, j].GetComponent<Text>().text = feromoneGrid[i, j].feromoneValue.ToString();
                    } else
                    {
                        feromoneText[i, j].GetComponent<Text>().enabled = false;
                    }
                            
                }
            }
            feromoneTex.Apply();
        }
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                foreach (Vector2 cNCoords in feromoneGrid[i, j].connectedNodes)
                {
                    int tmpX = (100 - feromoneGrid[i, j].x) - 50;
                    int tmpY = (100 - feromoneGrid[i, j].y) - 50;
                    int tmpLastX = (100 - (int)feromoneGrid[(int)cNCoords.x, (int)cNCoords.y].x) - 50;
                    int tmpLastY = (100 - (int)feromoneGrid[(int)cNCoords.x, (int)cNCoords.y].y) - 50;
                    Debug.DrawLine(new Vector3(tmpX, 0.0f, tmpY), new Vector3(tmpLastX, 0.0f, tmpLastY));
                    Debug.DrawLine(new Vector3(tmpLastX, 0.0f, tmpLastY), new Vector3(tmpLastX, 0.0f, tmpLastY) + new Vector3(0.1f, 0.0f, 0.0f));
                    Debug.DrawLine(new Vector3(tmpLastX, 0.0f, tmpLastY), new Vector3(tmpLastX, 0.0f, tmpLastY) + new Vector3(-0.1f, 0.0f, 0.0f));
                    Debug.DrawLine(new Vector3(tmpLastX, 0.0f, tmpLastY), new Vector3(tmpLastX, 0.0f, tmpLastY) + new Vector3(0.0f, 0.0f, 0.1f));
                    Debug.DrawLine(new Vector3(tmpLastX, 0.0f, tmpLastY), new Vector3(tmpLastX, 0.0f, tmpLastY) + new Vector3(0.0f, 0.0f, -0.1f));
                    //Debug.Log(new Vector3(feromoneGrid[i, j].x + 50.0f, 0.0f, feromoneGrid[i, j].y - 50.0f) + " " + new Vector3(feromoneGrid[i, j].connectedNodes[0].x - 50.0f, 0.0f, feromoneGrid[i, j].connectedNodes[0].y + 50.0f));
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        ground = GameObject.Find("Ground");
        feromoneGrid = new Node[100, 100];
        feromoneText = new GameObject[100, 100];
        feromoneTex = new Texture2D(100, 100, TextureFormat.ARGB32, false);
        for(int i = 0; i < 100; i++)
        {
            for(int j = 0; j < 100; j++)
            {
                feromoneTex.SetPixel(i, j, Color.black);
                feromoneGrid[i, j].x = i;
                feromoneGrid[i, j].y = j;
                feromoneGrid[i, j].feromoneValue = 0;
                feromoneGrid[i, j].connectedNodes = new List<Vector2>();

                GameObject newTextGO = new GameObject();
                newTextGO.transform.position = Vector3.zero;
                newTextGO.transform.parent = canvas.transform;
                Text text = newTextGO.AddComponent<Text>();
                text.text = "0.0";
                text.font = textFont;
                text.GetComponent<RectTransform>().rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                text.GetComponent<RectTransform>().position = new Vector3((100.0f - i) - 50.0f, 5.0f, (100.0f-j) - 50.0f);
                text.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 0.01f);
                text.color = Color.yellow;
                text.fontSize = fontSize;
                text.alignment = TextAnchor.MiddleCenter;
                text.enabled = false;
                feromoneText[i, j] = newTextGO;

                
                
            }
        }
        feromoneTex.Apply();
    }
    
    public Vector3 GetCloseFeromoneTrail(int x, int y)
    {
        //Debug.Log( x + " " + y + " " + feromoneGrid[x, y].connectedNodes.Count);
        if (feromoneGrid[x,y].connectedNodes.Count > 0)
        {
            Dictionary<Vector3, float> valueMapping = new Dictionary<Vector3, float>();
            float sumValue = 0.0f;
            foreach (Vector2 nCoords in feromoneGrid[x, y].connectedNodes)
            {
                sumValue += feromoneGrid[(int)nCoords.x, (int)nCoords.y].feromoneValue;
            }
            foreach (Vector2 nCoords in feromoneGrid[x, y].connectedNodes)
            {
                if(feromoneGrid[(int)nCoords.x, (int)nCoords.y].feromoneValue > 0)
                {
                    valueMapping.Add(GetPosFromCoords((int)nCoords.x, (int)nCoords.y), (float)feromoneGrid[(int)nCoords.x, (int)nCoords.y].feromoneValue/sumValue);
                }
            }

            if(choosePathType == ChoosePathType.ValuePercentage)
            {
                float ran = Random.Range(0.0f, 1.0f);
                /*var myList = valueMapping.ToList();

                myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
                //Debug.Log("Sorted:");
                foreach (KeyValuePair<Vector3, float> pair in myList)
                {
                    Debug.Log(pair.Value);
                }*/
                foreach (Vector3 nPos in valueMapping.Keys)
                {

                    ran -= valueMapping[nPos];

                    if (ran <= 0.0f)
                    {
                        return nPos;
                    }
                }
            } else
            {
                var myList = valueMapping.ToList();

                myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
                Debug.Log("Sorted:");
                foreach(KeyValuePair<Vector3,float> pair in myList)
                {
                    Debug.Log(pair.Value);
                }
            }

        } else
        {

            Dictionary<Vector3, float> valueMapping = new Dictionary<Vector3, float>();
            float sumValue = 0.0f;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int nX = x + i;
                    int nY = y + j;
                    if (nX < 0 || nX >= 100 || nY < 0 || nY >= 100)
                    {
                        continue;
                    }
                    if (i == 0 && j == 0)
                        continue;
                    sumValue += feromoneGrid[nX, nY].feromoneValue;
                }
            }
            for (int i = -1; i < 2; i++)
            {
                for(int j = -1; j < 2; j++)
                {
                    int nX = x + i;
                    int nY = y + j;
                    if (nX < 0 || nX >= 100 || nY < 0 || nY >= 100)
                    {
                        continue;
                    }
                    if (i == 0 && j == 0)
                        continue;
                    
                    if (feromoneGrid[nX,nY].feromoneValue > 0)
                    {
                        valueMapping.Add(GetPosFromCoords(nX, nY), (float)feromoneGrid[nX, nY].feromoneValue / sumValue);
                    }
                }
            }
            float ran = Random.Range(0.0f, 1.0f);
            foreach (Vector3 nPos in valueMapping.Keys)
            {

                ran -= valueMapping[nPos];

                if (ran <= 0.0f)
                {
                    return nPos;
                }
            }
        }

        return new Vector3(-1000.0f,-1000.0f,-1000.0f);
    }

    public void AddFeromone(int x, int y, int lastX, int lastY, int value)
    {
        //Debug.Log(x + " " + y + " " + lastX + " " + lastY);

        feromoneGrid[x, y].feromoneValue += value;
        feromoneGrid[x, y].timeStampOnLastAdd = Time.time;
        if (feromoneGrid[x, y].feromoneValue > maxValue)
            feromoneGrid[x, y].feromoneValue = maxValue;
        
        feromoneTex.SetPixel(x, y, new Color(0.0f, (float)feromoneGrid[x, y].feromoneValue / maxValue, 0.0f));
        feromoneTex.Apply();
        ground.GetComponent<Renderer>().material.mainTexture = feromoneTex;
        if (x == lastX && y == lastY)
            return;
        if(!feromoneGrid[x, y].connectedNodes.Contains(new Vector2(lastX, lastY)))
            feromoneGrid[x, y].connectedNodes.Add(new Vector2(lastX,lastY));
    }

    public void GetCurrentGridCoords(Vector3 pos, out int x, out int y)
    {
        x = (int)Mathf.Round(pos.x) + 50;
        x = 100 - x;
        y = (int)Mathf.Round(pos.z) + 50;
        y = 100 - y;
    }

    Vector3 GetPosFromCoords(int x, int y)
    {
        int tmpX = 100 - x;
        int tmpY = 100 - y;
        tmpX -= 50;
        tmpY -= 50;
        return new Vector3(tmpX, 0.0f, tmpY);
    }
}
