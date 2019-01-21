using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour {

    public int tileX;
    public int tileY;
    public TileMap map;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            map.selectedUnit.GetComponent<Unit>().ResetPath();
    }

    private void OnMouseDown()
    {
        map.AddNodeToPath(tileX, tileY);
        Debug.Log(map.currentPath.Count);
    }
}
