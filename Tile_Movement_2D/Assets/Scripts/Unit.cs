using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public int tileX;
    public int tileY;
    public TileMap map;

    public float moveDuration = .5f;
    float moveTimer = 0;

    public List<Node> currentPath = null;

    public bool canMove = true;
    public bool canAttack = true;

    public bool isMoving = false;

    // Calculate moveSpeed based on equipment weight, weight capacity and character speed.
    // Movespeed should be pulled from a character sheet.
    public int moveSpeed = 5;


    //Stats
    public int speed = 20;


    int remainingMoveSpeed;

    private void Start()
    {
        CombatTracker combatTracker = new CombatTracker();
        combatTracker = GameObject.FindObjectOfType<CombatTracker>();

        //combatTracker.AddUnit(gameObject);
    }

    private void Update()
    {
        moveTimer += Time.deltaTime;

        //if (isMoving && moveTimer > 0.2f)
        //{
        //    MoveNextTile();

        //    moveTimer = 0f;
        //}

        if (currentPath != null)
        {

            int currNode = 0;

            while (currNode < currentPath.Count - 1)
            {
                LimitPathLength();

                Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) + new Vector3(0, 0, -1f);
                Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y) + new Vector3(0, 0, -1f);

                Debug.DrawLine(start, end, Color.black);

                currNode++;
            }
        }
        else
        {
            isMoving = false;
        }
    }

   
    public void ResetPath()
    {
        currentPath = null;
    }

    void LimitPathLength()
    {
        if(currentPath != null && currentPath.Count > moveSpeed+1)
        {
            currentPath.RemoveAt(currentPath.Count-1);
        }
    }

    public void MoveNextTile()
    {
        isMoving = true;

        //This updateds when this units tile is occupied
        map.graph[currentPath[0].x, currentPath[0].y].isOccupied = false;
        map.ChosenNodeIsOccupied(currentPath[currentPath.Count - 1].x, currentPath[currentPath.Count - 1].y, true);

        int remainingMovement = moveSpeed;

        while (isMoving)
        {
            if (currentPath.Count > 1)
            {
                if (moveTimer > .1f|| true)
                {
                    if (currentPath == null)
                        break;

                    // Get cost to enter next tile.
                    //remainingMovement -= (int)map.CostToEnterTile(currentPath[1].x, currentPath[1].y);

                    // Now grab the new first node and move us to that position.
                    // Later switch to some form of Lerp to slide instead of teleport.
                    tileX = currentPath[1].x;
                    tileY = currentPath[1].y;
                    transform.position = map.TileCoordToWorldCoord(tileX, tileY);

                    // Remove the old current/first node from the path.
                    currentPath.RemoveAt(0);

                    if (currentPath.Count == 1)
                    {
                        // We only have one tile left in the path, and that tile must be our ultimate
                        // destination, which we are standing on, so let's just clear our pathfinding info.

                        currentPath = null;
                        isMoving = false;
                    }

                    moveTimer = 0f;
                }
            }
            else
            {
                isMoving = false;
                break;
            }
        }
    }
}
