using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;
    [SerializeField]
    private float size;
    private int[,] currentGrid;
    public GameObject getGridType;

    void Start() { 
        currentGrid = new int[width, height];

        print(height + " , " + width);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.DrawLine(getGridAtLocation(x, y), getGridAtLocation(x, y + 1), Color.red, 100f);
                Debug.DrawLine(getGridAtLocation(x, y), getGridAtLocation(x + 1, y), Color.red, 100f);
                currentGrid[x, y] = x + (y * height);
                //GameObject newGrid = Instantiate(getGridType, getGridAtLocation(x, y), Quaternion.Euler(90f, 0f, 0f));
                //newGrid.transform.SetParent(this.transform);
                //newGrid.transform.localPosition = getGridAtLocation(x, y);
            }
        }
    }
    
    public Vector3 getGridAtLocation(int getX, int getY)
    {
        return new Vector3(getX, 5, getY) * size + new Vector3(-395f, 0, -395f);
    }

    public int getValue(Vector3 worldPosition)
    {
        int x, y;
        getXY(worldPosition, out x, out y);
        return currentGrid[x, y];
    }

    public void getXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x+395f / size);
        y = Mathf.FloorToInt(worldPosition.z+395f / size);
    }
}
