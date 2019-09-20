using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// This class was heavily inspired by the tutorial from Code Monkey: https://www.youtube.com/watch?v=CmU5-v-v1Qo

public class Graph : MonoBehaviour
{
    [SerializeField] public Sprite circleSprite;

    private GraphData data;
    private RectTransform graphContainer;

    private Color graphColor = Color.green;
    private float lineThickness = 4f;
    private float dotSize = 8f;

    private List<GameObject> createdObjects = new List<GameObject>();

    private float[] lastData;

    // Start is called before the first frame update
    private void Awake()
    {
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        data = GetComponent<GraphData>();
    }

    private void FixedUpdate()
    {
        float[] currentData = data.data.ToArray();
        if (lastData == null || !Enumerable.SequenceEqual(lastData, currentData))
        {
            ClearGraph();
            ShowGraph(data.data);
            lastData = currentData;
        }
    }


    private GameObject CreateCircle(Vector2 anchorPos)
    {
        GameObject circleObj = new GameObject("circle", typeof(Image));
        circleObj.transform.SetParent(graphContainer, false);
        circleObj.GetComponent<Image>().sprite = circleSprite;
        circleObj.GetComponent<Image>().color = graphColor;
        RectTransform rectTransform = circleObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchorPos;
        rectTransform.sizeDelta = new Vector2(dotSize, dotSize);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        createdObjects.Add(circleObj);
        return circleObj;
    }

    private void ShowGraph(List<float> values)
    {
        if (values.Count == 0) return;

        float graphHeight = graphContainer.sizeDelta.y;
        float xSize = graphContainer.sizeDelta.x / (values.Count + 1);
        GameObject lastDot = null;
        for(int i = 0; i < values.Count; i++)
        {
            float value = Mathf.Clamp01(values[i]);
            float xPos = xSize + i * xSize;
            float yPos = value * graphHeight;
            GameObject dot = CreateCircle(new Vector2(xPos, yPos));
            if (lastDot != null)
            {
                CreateDotConnection(lastDot.GetComponent<RectTransform>().anchoredPosition, dot.GetComponent<RectTransform>().anchoredPosition);
            }
            lastDot = dot;
        }
    }

    private void CreateDotConnection(Vector2 pos1, Vector2 pos2)
    {
        GameObject line = new GameObject("line", typeof(Image));
        line.transform.SetParent(graphContainer, false);
        line.GetComponent<Image>().color = new Color(graphColor.r, graphColor.g, graphColor.b, 0.5f);
        RectTransform rectTransform = line.GetComponent<RectTransform>();
        Vector2 dir = (pos2 - pos1).normalized;
        float distance = Vector2.Distance(pos1, pos2);
        float angle = Mathf.Rad2Deg * Mathf.Atan(dir.y / dir.x);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, lineThickness);
        rectTransform.anchoredPosition = pos1 + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, angle);
        createdObjects.Add(line);
    }

    private void ClearGraph()
    {
        for(int i = 0; i < createdObjects.Count; i++)
        {
            Destroy(createdObjects[i]);
        }

        createdObjects.Clear();
    }

}
