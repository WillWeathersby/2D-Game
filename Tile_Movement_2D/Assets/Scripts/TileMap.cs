using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileMap : MonoBehaviour {

    public GameObject selectedUnit;// used to target selected unit

    
    public List<Node> currentPath = null;
    public List<Node> addedPath = null;
    Node listCheckpoint = null;

    int moveWeight = 0;

    public TileType[] tileTypes;//TileType should be an Enum

    int[,] tiles;
    public Node[,] graph;

    bool isAdding = false;

    int mapSizeX = 20;
    int mapSizeY = 15;

    private void Start()
    {
        //// Setup the selectedUnit's variables.
        //selectedUnit.GetComponent<Unit>().tileX = (int)selectedUnit.transform.position.x;
        //selectedUnit.GetComponent<Unit>().tileY = (int)selectedUnit.transform.position.y;
        //selectedUnit.GetComponent<Unit>().map = this;

        GenerateMapData();
        GeneratePathfindingGraph();
        GenerateMapVisuals();
    }

    private void Update()
    {
        if (currentPath != null && currentPath.Count > selectedUnit.GetComponent<Unit>().moveSpeed +1)
        {
            currentPath.RemoveAt(currentPath.Count - 1);
        }
    }

    public void SetupUnit()
    {
        // Setup the selectedUnit's variables.
        selectedUnit.GetComponent<Unit>().tileX = (int)selectedUnit.transform.position.x;
        selectedUnit.GetComponent<Unit>().tileY = (int)selectedUnit.transform.position.y;
        selectedUnit.GetComponent<Unit>().map = this;
    }

    public void RequestMove()
    {
        selectedUnit.GetComponent<Unit>().MoveNextTile();
    }

    private void GenerateMapData()
    {
        // Allocate map tiles
        tiles = new int[mapSizeX, mapSizeY];

        //Initialize our map tiles
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                tiles[x, y] = 0;
            }
        }

        for (int x = 9; x < 16; x++)
        {
            for (int y = 3; y < 7; y++)
            {
                tiles[x, y] = 1;
            }
        }

        //Make a Wall
        tiles[6, 8] = 2; // Change this or use values from a level editor or generate based on patterns
        tiles[7, 8] = 2;
        tiles[8, 8] = 2;
        tiles[9, 8] = 2;
        tiles[10, 8] = 2;
        tiles[11, 8] = 2;
        tiles[11, 9] = 2;
        tiles[11, 11] = 2;
        tiles[11, 12] = 2;

        tiles[6, 12] = 2;
        tiles[7, 12] = 2;
        tiles[8, 12] = 2;
        tiles[9, 12] = 2;
        tiles[10, 12] = 2;
        tiles[11, 12] = 2;

        tiles[6, 8] = 2;
        tiles[6, 9] = 2;
        tiles[6, 10] = 2;
        tiles[6, 11] = 2;
        tiles[6, 12] = 2;

        // Instantiate visual prefabs
    }

    public float CostToEnterTile(int x, int y)
    {
        TileType tt = tileTypes[tiles[x, y]];

        if (graph[x, y].isOccupied)
        {
            return graph[x, y].occupiedCost;
        }
        else
        {
            return tt.movementCost;
        }
    }

    void GeneratePathfindingGraph()
    {
        // Initialize the array.
        graph = new Node[mapSizeX, mapSizeY];

        // Initialize a Node for each spot in the array.
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;
            }
        }

        // Now that all of the nodes exist, calculate their neighbours.
        for (int x = 0; x < mapSizeX; x++)
            {
            for (int y = 0; y < mapSizeY; y++)
                {

                // 4-way movement system

                if (x > 0)
                    graph[x, y].neighbours.Add(graph[x-1, y]);
                if (x < mapSizeX-1)
                    graph[x, y].neighbours.Add(graph[x+1, y]);

                if (y > 0)
                    graph[x, y].neighbours.Add(graph[x, y-1]);
                if (y < mapSizeY - 1)
                    graph[x, y].neighbours.Add(graph[x, y+1]);

            }
        }
    }

    void GenerateMapVisuals()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                GameObject go = (GameObject)Instantiate(tileTypes[tiles[x, y]].tileVisualPrefab, new Vector3(x, y, 0), Quaternion.identity);

                //Check for Grass on a material
                if (tileTypes[tiles[x, y]].name == "Grass")
                {
                                                                                                        //I think we will need a lot of ifs to check each side of each tile to decide which texture is best
                }

                

                ClickableTile ct = go.GetComponent<ClickableTile>();
                ct.tileX = x;
                ct.tileY = y;
                ct.map = this;
            }
        }
    }

    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(x, y, 0);
    }

    public bool UnitCanEnterTile(int x, int y)
    {
        // Test a units movement type (walking/flying/swimming)
        if(tileTypes[tiles[x, y]].isWalkable && !graph[x,y].isOccupied)
        {
            return true;
        }
        else
        {
            return false;
        }
            
    }

    public void AddNodeToPath(int x, int y)
    {
        currentPath = new List<Node>();
        currentPath = selectedUnit.GetComponent<Unit>().currentPath;

        // This checks to see if the unit is exceeding its move speed. If it is
        // it recalculates its path to more efficiently get from point A to point B

        if (!selectedUnit.GetComponent<Unit>().isMoving)
        {
            if (currentPath != null && currentPath.Count + 1 < selectedUnit.GetComponent<Unit>().moveSpeed + 2)
            {

                isAdding = true;

                listCheckpoint = currentPath[currentPath.Count - 1];
                GeneratePathTo(x, y, currentPath[currentPath.Count - 1].x, currentPath[currentPath.Count - 1].y);
            }
            else
            {
                listCheckpoint = null;
                currentPath = new List<Node>();
                GeneratePathTo(x, y, selectedUnit.GetComponent<Unit>().tileX, selectedUnit.GetComponent<Unit>().tileY);
            }

            if (selectedUnit.GetComponent<Unit>().currentPath == null)
                selectedUnit.GetComponent<Unit>().currentPath = currentPath;

        }
    }

    public void CutPath(int start, int end)
    {
        Debug.Log("start: " + start);
        Debug.Log("end: " + end);
        for (int s = start; s < end; s++)
        {
            Debug.Log("removed: " + s);
            currentPath.RemoveAt(start);
        }
    }

    public void ChosenNodeIsOccupied(int x, int y, bool occupied)
    {
        graph[x, y].isOccupied = occupied;
    }


    public void GeneratePathTo(int targetX, int targetY, int sourceX, int sourceY)
    {
        // Clear out units old path.
        //selectedUnit.GetComponent<Unit>().currentPath = null;
        addedPath = new List<Node>();
        int moveSpeed = selectedUnit.GetComponent<Unit>().moveSpeed;

        if (UnitCanEnterTile(targetX,targetY) == false)
        {
            isAdding = false;
            currentPath = null;
            // We clicked on something impassable

            return;
        }

        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        // List of nodes we haven't visited yet.
        List<Node> unvisited = new List<Node>();

        Node source = graph[
                            sourceX,
                            sourceY
                            ];

        Node target = graph[
                            targetX,
                            targetY
                            ];

        dist[source] = 0;
        prev[source] = null;

        // Initialize everything to have INFINITY distance, since
        // we don't know any better right now. Also, it's possible
        // that some nodes can't be reached from the source, which
        // would make INFINITY a reasonable value.
        // Later we can set the distance to a preferred distance.
        foreach (Node v in graph)
        {
            if(v != source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }

            unvisited.Add(v);
        }
        while(unvisited.Count > 0)
        {
            // u is going th be the unvisited node with the smallest distance.
            Node u = null;

            foreach (Node possibleU in unvisited)
            {
                if(u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }

            if(u == target)
            {
                break;
            }

            unvisited.Remove(u);

            foreach (Node v in u.neighbours)
            {
                float alt = dist[u] + CostToEnterTile(v.x,v.y);
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        // If we made it here, then either we found the shortest route
        // to our target, or there is no route at all to our target.

        if(prev[target] == null)
        {
            // No route to our target.
            return;
        }


        Node curr = target;
        currentPath.Reverse();

        //step through the prev chain and add it to our path.
        // If we are adding onto our current path we instead set the new coordinates
        // into a temp List in order to add them together later
        while(curr != null || (curr == currentPath[currentPath.Count-1]) && prev[curr] != listCheckpoint)
        {
            if (isAdding)
            {
                //currentPath.Insert(0, curr);
                addedPath.Add(curr);
            }
            else
            {
                currentPath.Add(curr);
            }

            curr = prev[curr];
        }

        //Right now currentPath describes a route from our target to our
        //source so we need to invert it.

        // Here we reverse both pathes in order to move towards our target destination
        currentPath.Reverse();
        addedPath.Reverse();


        // Now we check to see if we are adding pathes together
        // if we are then added path is attached to the end of 
        // current path in order to update current path 
        if (isAdding)
        {
            addedPath.RemoveRange(0,1);
            currentPath.AddRange(addedPath);
        }

        isAdding = false;


        int cutStart = 0;
        int cutEnd = 0;
        bool stop = false;


        // Here we search for repeat nodes in order to prune our path for more efficient movement speed.
        for (int i = 0; i < currentPath.Count-1; i++)
        {
            for (int j = currentPath.Count-1; j > i; j--)
            {
                if(currentPath[i].x == currentPath[j].x && currentPath[i].y == currentPath[j].y)
                {
                    cutStart = i;
                    cutEnd = j;
                    stop = true;

                    CutPath(cutStart, cutEnd);
                    break;
                }
            }

            if (stop)
                break;
        }

        moveSpeed = selectedUnit.GetComponent<Unit>().moveSpeed;
        int tileCount = 0;


        // This gets the cost of the targeted tile, NOT the current tile
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            moveSpeed -= (int)CostToEnterTile(currentPath[i + 1].x, currentPath[i + 1].y) - 1;

            if (moveSpeed <= tileCount)
                break;

            tileCount++;
        }

        List<Node> tempPath = currentPath;

        moveSpeed = selectedUnit.GetComponent<Unit>().moveSpeed;

        while (currentPath.Count > tileCount + 1)
        {
            currentPath.RemoveAt(currentPath.Count - 1);
        }

        selectedUnit.GetComponent<Unit>().currentPath = currentPath;
    }

    
}
