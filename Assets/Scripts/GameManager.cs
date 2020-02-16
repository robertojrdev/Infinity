using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Grid grid;
    public Node nodePrefab;

    private Dictionary<int, Node> nodes = new Dictionary<int, Node>();
    private int draggingIndex = -1;
    private Vector2 currentDragPosition;

    private void Start()
    {
        for (int x = 0; x < 4; x++)
        {
            var position = grid.GetCellPosition(x, 5);
            var index = grid.PositionToIndex(x, 5);
            var node = Instantiate(nodePrefab, position, Quaternion.identity);
            nodes.Add(index, node);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryStartDrag();

        if(draggingIndex != -1)
        {
            Drag();
        }

        if (Input.GetMouseButtonUp(0) && draggingIndex != -1)
            StopDrag();
    }

    private void Drag()
    {

        var node = nodes[draggingIndex];
        if(node.positions.Count > node.maxPositions)
            return;

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        node.Drag(mousePos);
        
        int x, y = 0;
        if(grid.GetCellAtPosition(mousePos, out x, out y))
        {
            //check if cell is empty
            var cellId = grid.PositionToIndex(x,y);
            if(nodes.ContainsKey(cellId))
                return;
            
            //previous node position
            int nx, ny = 0;
            grid.GetCellAtPosition(node.lastPosition, out nx, out ny);

            //check if the grid position that the mouse is above is in an orthogonal direction
            if(Mathf.Abs(nx - x) + Mathf.Abs(ny - y) <= 1)
            {
                //get the cell center
                var cellPos = grid.GetCellPosition(x, y);
                node.AddPoint(cellPos);
                nodes.Add(cellId, node);
            }
        }
    }

    private void TryStartDrag()
    {
        int x, y;
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (grid.GetCellAtPosition(mousePos, out x, out y))
        {
            var index = grid.PositionToIndex(x, y);
            if (nodes.ContainsKey(index))
            {
                var node = nodes[index];

                node.ResetPositions();
                draggingIndex = index;

                var lastPos = node.lastPosition;
                grid.GetCellAtPosition(lastPos, out x, out y);
                currentDragPosition = new Vector2(x, y);
            }
        }
    }

    private void StopDrag()
    {
        nodes[draggingIndex].CancelDrag();
        draggingIndex = -1;
    }
}