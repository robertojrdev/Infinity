using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Grid grid;
    public Node nodePrefab;
    public Connection connectionPrefab;

    private Node draggingNode;
    private GridElementsArray gridElements;

    private void Start()
    {
        gridElements = grid.GetGridElementsArray();

        for (int x = 0; x < 4; x++)
        {
            var position = grid.GetCellPosition(x, 5);
            var node = Instantiate(nodePrefab, position, Quaternion.identity);
            gridElements.AddElement(node, x, 5);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryStartDrag();

        if (draggingNode != null)
        {
            Drag();
        }

        if (Input.GetMouseButtonUp(0) && draggingNode != null)
            StopDrag();
    }

    private void Drag()
    {
        if (draggingNode.positions.Count > draggingNode.maxPositions)
            return;

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        draggingNode.Drag(mousePos);

        int x, y = 0;
        if (grid.GetCellAtPosition(mousePos, out x, out y))
        {
            //check if cell is empty
            if (gridElements.elements[x, y] != null)
                return;

            //previous node position
            var prev = draggingNode.gridPositions[draggingNode.gridPositions.Count - 1];

            //check if the grid position that the mouse is above is in an orthogonal direction
            if (Mathf.Abs(prev.x - x) + Mathf.Abs(prev.y - y) <= 1)
            {
                //get the cell center
                var cellPos = grid.GetCellPosition(x, y);
                draggingNode.AddPoint(cellPos);
                gridElements.AddElementPositionOwnership(draggingNode, x, y);
            }
        }
    }

    private void TryStartDrag()
    {
        int x, y;
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (grid.GetCellAtPosition(mousePos, out x, out y))
        {
            if (gridElements.elements[x, y] is Node)
            {
                var node = (Node)gridElements.elements[x, y];

                //clear previous movements
                gridElements.ClearAllAdditionalElementPositions(node);
                node.ResetPositions();

                draggingNode = node;
            }
        }
    }

    private void StopDrag()
    {
        draggingNode.CancelDrag();
        draggingNode = null;
    }
}