using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    private List<Cell> cells = new List<Cell>();
    private List<Cell> visited_cells = new List<Cell>();
    private List<Cell> Closed_cells = new List<Cell>();
    private List<Cell> Stack = new List<Cell>();
    private Cell current_cell;
    private int rows, columns;
    private int grid_size_x, grid_size_y;
    private GameObject g_manager;

    public GameObject current_particle;
    public GameObject stack_particle;
    public GameObject visited_particle;
    [SerializeField]
    private bool toggle_visualization = true;

    public GameObject wall_prefab;

    
    public int cell_size = 10;

    private void Awake()
    {
        rows = Mathf.FloorToInt(GameObject.FindGameObjectWithTag("Ground").transform.localScale.x / cell_size);
        columns = Mathf.FloorToInt(GameObject.FindGameObjectWithTag("Ground").transform.localScale.z / cell_size);

        grid_size_x = Mathf.FloorToInt(GameObject.FindGameObjectWithTag("Ground").transform.localScale.x);
        grid_size_y = Mathf.FloorToInt(GameObject.FindGameObjectWithTag("Ground").transform.localScale.z);
        g_manager = GameObject.FindGameObjectWithTag("Manager")as GameObject;

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                Cell new_cell = new Cell(x, y);
                cells.Add(new_cell);
            }
        }
    }
    private void Start()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            float x_position = cells[i].x * cell_size - ((grid_size_x / 2) - (cell_size / 2));
            float y_position = cells[i].y * cell_size - ((grid_size_y / 2) - (cell_size / 2));
            cells[i].x_position = x_position;
            cells[i].y_position = y_position;

            GameObject parent_instance = new GameObject();
            parent_instance.name = "cell " + i;

            /*walls[0]=top, walls[1]=bottom, walls[2]=right, walls[3]=left*/
            
            if (cells[i].walls[0])
            {
                GameObject top_wall = Instantiate(wall_prefab, new Vector3(x_position, (cell_size/2)-2, y_position + cell_size / 2),
                                                     Quaternion.identity) as GameObject;
                top_wall.transform.localScale = new Vector3(cell_size , cell_size, cell_size / (cell_size/2) - 1);
                top_wall.transform.name = "top_wall";
                top_wall.transform.parent = parent_instance.transform;
                cells[i].cell_walls[0] = (top_wall);
            }
            if (cells[i].walls[1])
            {
                GameObject bottom_wall = Instantiate(wall_prefab, new Vector3(x_position, (cell_size/2)-2, y_position - cell_size / 2),
                                                     Quaternion.identity) as GameObject;
                bottom_wall.transform.localScale = new Vector3(cell_size , cell_size-2, cell_size / (cell_size / 2) - 1);
                bottom_wall.transform.name = "bottom_wall";
                bottom_wall.transform.parent = parent_instance.transform;
                cells[i].cell_walls[1] = (bottom_wall);
            }
            if (cells[i].walls[2])
            {
                GameObject right_wall = Instantiate(wall_prefab, new Vector3(x_position + cell_size / 2, (cell_size / 2) - 2, y_position ),
                                                     Quaternion.identity) as GameObject;
                right_wall.transform.localScale = new Vector3(cell_size / (cell_size / 2) - 1, cell_size-2, cell_size);
                right_wall.transform.name = "right_wall";
                right_wall.transform.parent = parent_instance.transform;
                cells[i].cell_walls[2] = (right_wall);
            }
            if (cells[i].walls[2])
            {
                GameObject left_wall = Instantiate(wall_prefab, new Vector3(x_position - cell_size / 2, (cell_size / 2) - 2, y_position),
                                                     Quaternion.identity) as GameObject;
                left_wall.transform.transform.localScale = new Vector3(cell_size / (cell_size / 2) - 1, cell_size-2, cell_size );
                left_wall.transform.name = "left_wall";
                left_wall.transform.parent = parent_instance.transform;
                cells[i].cell_walls[3] = (left_wall);

            }
            parent_instance.transform.parent = g_manager.transform;
        }
        current_cell = cells[0];
        current_cell.visited = true;
        
    }


    private void Update()
    {

        visited_cells.Add(current_cell);
        Cell next_cell = GetRandomNeighbour(current_cell.x, current_cell.y);
        if (next_cell != null)
        {
            next_cell.visited = true;
            Stack.Add(current_cell);
            RemoveWalls(current_cell, next_cell);
            current_cell = next_cell;

        }
        else if (Stack.Count > 0)
        {
            current_cell = Stack[Stack.Count - 1];

            Stack.Remove(Stack[Stack.Count - 1]);

        }
        if (Input.GetKey(KeyCode.Q))
        {
            toggle_visualization = !toggle_visualization;
        }
        //Visualisation
        if (toggle_visualization == true)
        {
            if (visited_cells.Count > 0)
            {
                for (int i = 0; i < visited_cells.Count; i++)
                {
                    GameObject my_instance = (GameObject)Instantiate(visited_particle, new Vector3(visited_cells[i].x_position, 2, visited_cells[i].y_position), Quaternion.identity);
                    Destroy(my_instance, 0.2f);
                }
            }
            if (Stack.Count > 0)
            {
                for (int i = 0; i < Stack.Count; i++)
                {
                    GameObject my_instance = (GameObject)Instantiate(stack_particle, new Vector3(Stack[i].x_position, 3, Stack[i].y_position), Quaternion.identity);
                    Destroy(my_instance, 0.2f);
                }
            }
            if (Stack.Count > 0)
            {
                GameObject my_instance = (GameObject)Instantiate(current_particle, new Vector3(current_cell.x_position, 4, current_cell.y_position), Quaternion.identity);
                Destroy(my_instance, 0.2f);
            }
        }
    }

    public void Toggle_v()
    {
        toggle_visualization = !Input.GetKeyDown(KeyCode.Q);
    }
    Cell GetRandomNeighbour(int x, int y)
    {
        List<Cell> neighbours = new List<Cell>();

        Cell top = null;
        Cell bottom = null;
        Cell right = null;
        Cell left = null;


        if (Get_Index(x, y + 1) == 0 && y + 1 < 0 || x < 0 || x > rows - 1 || y + 1 > columns - 1)
        {
            top = null;
        }
        else {
            top = cells[Get_Index(x, y + 1)];
        }
        if (Get_Index(x, y - 1) == 0 && y - 1 < 0 || x < 0 || x > rows - 1 || y - 1 > columns - 1)
        {
            bottom = null;
        }
        else
        {
            bottom = cells[Get_Index(x, y - 1)];
        }
        if (Get_Index(x + 1, y) == 0 && y  < 0 || x + 1 < 0 || x + 1 > rows - 1 || y > columns - 1)
        {
            right = null;
        }
        else
        {
            right = cells[Get_Index(x + 1, y)];
        }
        if (Get_Index(x - 1, y) == 0 && y < 0 || x - 1 < 0 || x - 1 > rows - 1 || y > columns - 1)
        {
            left = null;
        }
        else
        {
            left = cells[Get_Index(x - 1, y)];
        }


        if (top != null && !top.visited && !Closed_cells.Contains(top))
        {
            neighbours.Add(top);
        }
        else
        {
            top = null;
        }
        if (bottom != null && !bottom.visited && !Closed_cells.Contains(bottom))
        {
            neighbours.Add(bottom);
        }
        else
        {
            bottom = null;
        }
        if (right != null && !right.visited && !Closed_cells.Contains(right))
        {
            neighbours.Add(right);
        }
        else
        {
            right = null;
        }
        if (left != null && !left.visited && !Closed_cells.Contains(left))
        {
            neighbours.Add(left);


        }
        else
        {
            left = null;
        }

        if (neighbours.Count > 0 && neighbours.Count < 5)
        {
            /*for (int i = 0; i < neighbours.Count; i++)
            {
                Debug.Log(neighbours[i].x + " , " + neighbours[i].y + "\n");
            }*/

            int random_value = Random.Range(0, neighbours.Count);
            
            /*Debug.Log(neighbours[random_value].x + " , " + neighbours[random_value].y + " random " + "\n");*/
            
            return neighbours[random_value];

        }
        else
        {
            return null;
        }
    }
    int Get_Index(int x_val, int y_val)
    {
        if (y_val < 0 || x_val < 0 || x_val > rows - 1 || y_val > columns - 1)
        {
            return 0;
        }
        else
        {
            return y_val + x_val * columns;

        }
    }

    void RemoveWalls(Cell current, Cell next)
    {
        int x = current.x - next.x;
        int y = current.y - next.y;


        if (x == 1)
        {
            current.walls[3] = false;
            next.walls[2] = false;
            Destroy(current.cell_walls[3]);
            Destroy(next.cell_walls[2]);

        }
        else if (x == -1)
        {
            current.walls[2] = false;
            next.walls[3] = false;
            Destroy(current.cell_walls[2]);
            Destroy(next.cell_walls[3]);
        }
        if (y == 1)
        {
            current.walls[1] = false;
            next.walls[0] = false;
            Destroy(current.cell_walls[1]);
            Destroy(next.cell_walls[0]);
        }
        else if (y == -1)
        {
            current.walls[0] = false;
            next.walls[1] = false;
            Destroy(current.cell_walls[0]);
            Destroy(next.cell_walls[1]);
        }
        
    }
    private void OnDrawGizmos()
    {
        if (visited_cells.Count > 0)
        {
            for (int i = 0; i < visited_cells.Count; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(new Vector3(visited_cells[i].x_position, 5, visited_cells[i].y_position), new Vector3(cell_size-8, cell_size - 7, cell_size - 8));
            }
        }
        if(Stack.Count > 0)
        {
            for (int i = 0; i < Stack.Count; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(new Vector3(Stack[i].x_position, 5, Stack[i].y_position), new Vector3(cell_size - 8, cell_size - 7, cell_size - 8));
            }
        }
        if (Stack.Count > 0) {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(current_cell.x_position, 4, current_cell.y_position), new Vector3(cell_size - 4, cell_size - 7, cell_size - 4));
        }


    }
}
