using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell {
    public int x;
    public int y;
    public float x_position;
    public float y_position;
    public GameObject[] cell_walls = new GameObject[4];
    public bool[] walls = { true, true, true, true };
    public bool visited = false;
    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;

    }
    
}
