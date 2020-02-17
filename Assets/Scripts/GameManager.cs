using System;
using System.Collections;
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
        CreateNewLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //TODO: REMOVE THIS
            OnWinGame();

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
        if (draggingNode.positions.Count > draggingNode.MaxPositions || draggingNode.connection)
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


                    if (gridElements.elements[x, y] is Connection) //if is a connection connect
                    {
                        //connect
                        var connection = (Connection)gridElements.elements[x, y];

                        if(connection.Connected) //if already has a connection don't connect
                            return;

                        connection.Connected = true;
                        draggingNode.connection = connection;

                        //add point (extend line)
                        draggingNode.AddPoint(cellPos);
                    }
                    else
                    {
                        //add point (extend line)
                        draggingNode.AddPoint(cellPos);
                        gridElements.AddElementPositionOwnership(draggingNode, x, y);
                    }
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
                {
                    return false;
                }
            }
            else if (element is Node)
            {
                var node = (Node)element;
                if (node.positions.Count <= node.MaxPositions)
                {
                    return false;
                }
            }
        }

        return true;
    }

    [ContextMenu("Win game")]
    private void OnWinGame()
    {
        foreach (var el in gridElements.elements)
        {
            if (el != null)
                Destroy(((Component)el).gameObject);
        }

        CreateNewLevel();
    }

    private void CreateNewLevel()
    {
        background.UpdateColor();
        LevelGenerator.CreateNewLevel(
            8, 10,
            nodePrefab, connectionPrefab,
            grid, ref gridElements
            );
    }

}