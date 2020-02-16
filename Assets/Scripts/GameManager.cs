using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private Connection connectionPrefab;
    [SerializeField] private Background background;

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

        for (int x = 0; x < 4; x++)
        {
            var position = grid.GetCellPosition(x, 2);
            var connection = Instantiate(connectionPrefab, position, Quaternion.identity);
            gridElements.AddElement(connection, x, 2);
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
        {
            StopDrag();

            var won = CheckForWinGame();
            if (won)
                OnWinGame();
        }
    }

    private void Drag()
    {
        //return if has achieved the max num of positions or already is connected
        if (draggingNode.positions.Count > draggingNode.maxPositions || draggingNode.connection)
            return;

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        draggingNode.Drag(mousePos);

        int x, y = 0;
        if (grid.GetCellAtPosition(mousePos, out x, out y))
        {
            //check if cell is not a node
            if (!(gridElements.elements[x, y] is Node))
            {
                //previous node position
                var prev = draggingNode.gridPositions[draggingNode.gridPositions.Count - 1];

                //check if the grid position that the mouse is above is in an orthogonal direction
                if (Mathf.Abs(prev.x - x) + Mathf.Abs(prev.y - y) <= 1)
                {
                    //get the cell center
                    var cellPos = grid.GetCellPosition(x, y);

                    //add point (extend line)
                    draggingNode.AddPoint(cellPos);

                    if (gridElements.elements[x, y] is Connection) //if is a connection connect
                    {
                        var connection = (Connection)gridElements.elements[x, y];
                        connection.Connected = true;
                        draggingNode.connection = connection;
                    }
                    else
                        gridElements.AddElementPositionOwnership(draggingNode, x, y);
                }
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

                //disconnect
                if (node.connection)
                {
                    node.connection.Connected = false;
                    node.connection = null;
                }

                draggingNode = node;
            }
        }
    }

    private void StopDrag()
    {
        draggingNode.CancelDrag();
        draggingNode = null;
    }

    private bool CheckForWinGame()
    {
        foreach (var element in gridElements.elements)
        {
            if (element is Connection)
            {
                var connection = (Connection)element;
                if (!connection.Connected)
                    Debug.Log("not connected", connection);
                    return false;
            }
            else if (element is Node)
            {
                var node = (Node)element;
                if (node.positions.Count <= node.maxPositions)
                    return false;
            }
        }

        return true;
    }

    private void OnWinGame()
    {
        background.UpdateColor();
    }
}