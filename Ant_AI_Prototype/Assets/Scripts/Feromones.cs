using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class Feromones : MonoBehaviour {

    public enum DeteriationType { Linear, Percentage, TimeDependent };
    public enum ChoosePathType { ValuePercentage, SortedPercentage };

    public float maxValue;
    public float maxDefendValue;
    public DeteriationType deteriationType;
    public float deteriationPrSec;
    public float percentagePrSec;
    public float minValueForPercentage;
    public float timeUntilDeteriation;
    public float firstTimeDeteriationCoolDown;
    ChoosePathType choosePathType = ChoosePathType.ValuePercentage;
    int[] sortedPercentages = new int[3];
    public bool createFeromoneText;
    public Font textFont;
    public int fontSize;
    GameObject ground;

    Texture2D feromoneTex;
    Texture2D defendTex;
    Node[,] feromoneGrid;
    GameObject[,] feromoneText;
    GameObject canvas;

    float deteriationTimer;

    struct Node
    {
        public int x;
        public int y;
        public float feromoneValue;
        public float defendValue;
        public List<Vector2> connectedNodes;
        public List<Vector2> backConnectedNodes;
        public float timeStampOnLastAdd;
        public float firstAddTimeStamp;
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
                    if (Time.time - feromoneGrid[i, j].firstAddTimeStamp > firstTimeDeteriationCoolDown)
                    {
                        if (deteriationType == DeteriationType.Linear)
                        {
                            feromoneGrid[i, j].feromoneValue -= deteriationPrSec;
                            feromoneGrid[i, j].defendValue -= deteriationPrSec;
                        }
                        else if (deteriationType == DeteriationType.Percentage)
                        {
                            feromoneGrid[i, j].feromoneValue *= (100.0f - percentagePrSec) / 100.0f;
                            feromoneGrid[i, j].defendValue *= (100.0f - percentagePrSec) / 100.0f;
                            if (feromoneGrid[i, j].feromoneValue <= minValueForPercentage)
                            {
                                feromoneGrid[i, j].feromoneValue = 0.0f;
                                feromoneGrid[i, j].defendValue = 0.0f;
                                feromoneGrid[i, j].connectedNodes.Clear();
                                feromoneGrid[i, j].backConnectedNodes.Clear();
                            }
                        }
                        else if (deteriationType == DeteriationType.TimeDependent)
                        {
                            if (Time.time - feromoneGrid[i, j].timeStampOnLastAdd > timeUntilDeteriation)
                            {
                                feromoneGrid[i, j].feromoneValue -= deteriationPrSec;
                            }
                        }
                    }
                    if(feromoneGrid[i, j].feromoneValue < 0)
                    {
                        feromoneGrid[i, j].feromoneValue = 0.0f;
                        feromoneGrid[i, j].connectedNodes.Clear();
                        feromoneGrid[i, j].backConnectedNodes.Clear();
                    }
                    if(feromoneGrid[i,j].defendValue < 0)
                    {
                        feromoneGrid[i, j].defendValue = 0.0f;
                    }
                    deteriationTimer = 0.0f;
                    feromoneTex.SetPixel(i, j, new Color(0.0f, (float)feromoneGrid[i, j].feromoneValue / maxValue, 0.0f));
                    defendTex.SetPixel(i, j, new Color(0.0f, 0.0f, (float)feromoneGrid[i, j].defendValue / maxDefendValue));
                    if (createFeromoneText)
                    {
                        if (feromoneGrid[i, j].feromoneValue > 0.0f)
                        {
                            feromoneText[i, j].GetComponent<Text>().enabled = true;
                            int tmpValue = (int)feromoneGrid[i, j].feromoneValue;
                            feromoneText[i, j].GetComponent<Text>().text = tmpValue.ToString();
                        }
                        else
                        {
                            feromoneText[i, j].GetComponent<Text>().enabled = false;
                        }
                    }       
                }
            }
            feromoneTex.Apply();
            defendTex.Apply();
        }
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                foreach (Vector2 cNCoords in feromoneGrid[i, j].backConnectedNodes)
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
        if (Input.GetKeyDown("1"))
        {
            ground.GetComponent<Renderer>().material.mainTexture = feromoneTex;
        }
        if (Input.GetKeyDown("2"))
        {
            ground.GetComponent<Renderer>().material.mainTexture = defendTex;
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
        defendTex = new Texture2D(100, 100, TextureFormat.ARGB32, false);

        for(int i = 0; i < 100; i++)
        {
            for(int j = 0; j < 100; j++)
            {
                feromoneTex.SetPixel(i, j, Color.black);
                defendTex.SetPixel(i, j, Color.black);
                feromoneGrid[i, j].x = i;
                feromoneGrid[i, j].y = j;
                feromoneGrid[i, j].feromoneValue = 0;
                feromoneGrid[i, j].connectedNodes = new List<Vector2>();
                feromoneGrid[i, j].backConnectedNodes = new List<Vector2>();
                if (createFeromoneText)
                {
                    GameObject newTextGO = new GameObject();
                    newTextGO.transform.position = Vector3.zero;
                    newTextGO.transform.parent = canvas.transform;
                    Text text = newTextGO.AddComponent<Text>();
                    text.text = "0.0";
                    text.font = textFont;
                    text.GetComponent<RectTransform>().rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                    text.GetComponent<RectTransform>().position = new Vector3((100.0f - i) - 50.0f, 5.0f, (100.0f - j) - 50.0f);
                    text.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    text.color = Color.yellow;
                    text.fontSize = fontSize;
                    text.alignment = TextAnchor.MiddleCenter;
                    text.enabled = false;
                    newTextGO.isStatic = true;
                    feromoneText[i, j] = newTextGO;
                }
            }
        }
        feromoneTex.Apply();
        ground.GetComponent<Renderer>().material.mainTexture = feromoneTex;
    }

    public Vector3 GetProtectionOfFeromoneTrail(int x, int y, bool goingOut)
    {
        List<Vector2> nodes = new List<Vector2>();
        if (goingOut)
            nodes = feromoneGrid[x, y].connectedNodes;
        else
            nodes = feromoneGrid[x, y].backConnectedNodes;
        if (nodes.Count > 0 || !goingOut)
        {
            Dictionary<Vector3, float> valueMapping = new Dictionary<Vector3, float>();
            foreach (Vector2 nCoords in nodes)
            {
                if (feromoneGrid[(int)nCoords.x, (int)nCoords.y].feromoneValue > 0)
                {

                    valueMapping.Add(GetPosFromCoords((int)nCoords.x, (int)nCoords.y), (float)feromoneGrid[(int)nCoords.x, (int)nCoords.y].feromoneValue - feromoneGrid[(int)nCoords.x, (int)nCoords.y].defendValue);

                }
            }

            Vector3 largestPos = new Vector3(-1000.0f,-1000.0f,-1000.0f);
            float largestValue = -1000.0f;
            foreach(Vector3 pos in valueMapping.Keys)
            {
                Debug.Log(valueMapping[pos]);
                if(valueMapping[pos] > largestValue)
                {
                    largestPos = pos;
                    largestValue = valueMapping[pos];
                }
            }
            return largestPos;
        }
        else
        {

            Dictionary<Vector3, float> valueMapping = new Dictionary<Vector3, float>();
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

                    if (feromoneGrid[nX, nY].feromoneValue > 0)
                    {
                        valueMapping.Add(GetPosFromCoords(nX, nY), (float)feromoneGrid[nX, nY].feromoneValue - feromoneGrid[nX, nY].defendValue);
                    }
                }
            }
            Vector3 largestPos = new Vector3(-1000.0f, -1000.0f, -1000.0f);
            float largestValue = 0.0f;
            foreach (Vector3 pos in valueMapping.Keys)
            {
                if (valueMapping[pos] > largestValue)
                {
                    largestPos = pos;
                    largestValue = valueMapping[pos];
                }
            }
            return largestPos;
        }
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

        if(feromoneGrid[x, y].feromoneValue == 0.0f)
        {
            feromoneGrid[x, y].firstAddTimeStamp = Time.time;
        }

        feromoneGrid[x, y].feromoneValue += value;
        feromoneGrid[x, y].timeStampOnLastAdd = Time.time;
        
        if (feromoneGrid[x, y].feromoneValue > maxValue)
            feromoneGrid[x, y].feromoneValue = maxValue;
        
        feromoneTex.SetPixel(x, y, new Color(0.0f, (float)feromoneGrid[x, y].feromoneValue / maxValue, 0.0f));
        feromoneTex.Apply();
        
        if (x == lastX && y == lastY)
            return;
        if(!feromoneGrid[x, y].connectedNodes.Contains(new Vector2(lastX, lastY)))
            feromoneGrid[x, y].connectedNodes.Add(new Vector2(lastX,lastY));
        if (!feromoneGrid[lastX, lastY].backConnectedNodes.Contains(new Vector2(x, y)))
            feromoneGrid[lastX, lastY].backConnectedNodes.Add(new Vector2(x, y));
    }

    public void AddDefendTrail(int x, int y, float value)
    {

        feromoneGrid[x, y].defendValue += value;

        if (feromoneGrid[x, y].defendValue > maxDefendValue)
            feromoneGrid[x, y].defendValue = maxDefendValue;

        defendTex.SetPixel(x, y, new Color(0.0f,0.0f, (float)feromoneGrid[x, y].defendValue / maxDefendValue));
        defendTex.Apply();

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

    public void SetDefendFeromone(int x, int y, float value)
    {
        feromoneGrid[x, y].defendValue = value;
    }
}
