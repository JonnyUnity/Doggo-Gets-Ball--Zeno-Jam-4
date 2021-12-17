using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int rows = 5;
    [SerializeField]
    private int cols = 8;
    [SerializeField]
    private float tileSize = 1;
    [SerializeField]
    private float wallThickness = 1;

    void Start()
    {
       // GenerateGrid();
        
    }

    private void GenerateGrid()
    {
        // Create grid
        GameObject refTile = (GameObject)Instantiate(Resources.Load("GrassTile"));
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject tile = (GameObject)Instantiate(refTile, transform);
                float posX = col * tileSize;
                float posY = row * -tileSize;

                tile.transform.position = new Vector2(posX, posY);
            }
        }

        Destroy(refTile);

        float gridW = cols * tileSize;
        float gridH = rows * tileSize;

        // Create walls
        GameObject refWall = (GameObject)Instantiate(Resources.Load("Wall"));
        GameObject wall = Instantiate(refWall, transform);

        Debug.Log("GridW:" + gridW);
        Debug.Log("GridH:" + gridH);

        //// up
        wall.transform.localScale = new Vector2(cols + (wallThickness * 2), wallThickness);
        wall.transform.position = new Vector2((gridW / 2) - (tileSize / 2), (tileSize / 2) + (wallThickness / 2));


        //down
        wall = Instantiate(refWall, transform);
        wall.transform.localScale = new Vector2(cols + (wallThickness * 2), wallThickness);
        wall.transform.position = new Vector2((gridW / 2) - (tileSize / 2), -gridH + (tileSize / 2) - (wallThickness / 2));

        //left
        wall = Instantiate(refWall, transform);
        wall.transform.localScale = new Vector2(wallThickness, rows + (wallThickness*2));
        wall.transform.position = new Vector2(-(tileSize/2) - (wallThickness/2), (-gridH/2) + (tileSize / 2));


        //right
        wall = Instantiate(refWall, transform);
        wall.transform.localScale = new Vector2(wallThickness, rows + (wallThickness*2));
        wall.transform.position = new Vector2(gridW + (wallThickness/2) - (tileSize/2), (-gridH / 2) + (tileSize / 2));


        //// up
        //wall.transform.localScale = new Vector2((cols + 1) * tileSize + wallThickness, wallThickness);
        //wall.transform.position = new Vector2((gridW/2) - (wallThickness/2), (gridH/2) + (wallThickness/2));

        //// down
        //wall = (GameObject)Instantiate(refWall, transform);

        //wall.transform.localScale = new Vector2((cols + 1) * tileSize + wallThickness, wallThickness);
        //wall.transform.position = new Vector2((gridW / 2) - (wallThickness / 2), (-gridH / 2) - (wallThickness / 2));


        //// left
        //wall = (GameObject)Instantiate(refWall, transform);

        //wall.transform.localScale = new Vector2(wallThickness, (rows + 1) * tileSize + wallThickness);
        //wall.transform.position = new Vector2(-wallThickness, (gridH / 2) - (wallThickness / 2));

        Destroy(refWall);

        // Center the grid
        transform.position = new Vector2((-gridW / 2) + (tileSize / 2), (gridH / 2) - (tileSize / 2));

    }

    void Update()
    {
        
    }
}
