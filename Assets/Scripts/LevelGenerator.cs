using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator
{
    public static void CreateNewLevel(int nodesCount, int nodesSize, Node nodePrefab,
     Connection connectionPrefab, Grid grid, ref GridElementsArray gridElements)
    {
        gridElements = grid.GetGridElementsArray();

        //define available positions
        var gridSize = grid.gridSize.x * grid.gridSize.y;
        List<int> availablePositions = new List<int>();
        for (int i = 0; i < gridSize; i++)
            availablePositions.Add(i);

        //store nodes to clear their paths
        var nodes = new List<Node>();

        for (int i = 0; i < nodesCount; i++)
        {
            //get an available position
            var nodeIndex = UnityEngine.Random.Range(0, availablePositions.Count);
            var nodePositionIndex = availablePositions[nodeIndex];
            availablePositions.RemoveAt(nodeIndex);

            //create new node
            var (x, y) = grid.IndexToPosition(nodePositionIndex);
            var position = grid.GetCellPosition(x, y);
            var node = GameObject.Instantiate(nodePrefab, position, Quaternion.identity);
            nodes.Add(node);
            node.MaxPositions = nodesSize;
            gridElements.AddElement(node, x, y);

            //create a path to a connection
            var (prevX, prevY) = (x, y);
            for (int j = 0; j < nodesSize; j++)
            {
                //start with a random direction
                var dirIndex = UnityEngine.Random.Range(0, 4);
                var foundValidPosition = false;
                for (int ind = 0; ind < 4; ind++)
                {
                    //shift position direction index each try
                    dirIndex = ++dirIndex % 4;
                    var dir = GetDirection(dirIndex);

                    //check if direction is available
                    var (nx, ny) = (prevX + dir.x, prevY + dir.y);
                    foundValidPosition = CheckPosition(nx, ny, grid, ref gridElements);

                    //if position is not available try the next
                    if (!foundValidPosition)
                        continue;

                    //FOUND AN EMPTY CELL

                    //mark position as taken, update previous position and continue looking for the next position
                    gridElements.AddElementPositionOwnership(node, nx, ny);
                    var index = grid.PositionToIndex(nx, ny);
                    availablePositions.Remove(index);
                    (prevX, prevY) = (nx, ny);
                    break;
                }

                //if couldn't find any available position
                if (!foundValidPosition)
                {
                    if (j == 0) //if is confined destroy node
                    {
                        gridElements.RemoveElement(node);
                        nodes.Remove(node);
                        GameObject.Destroy(node.gameObject);
                    }
                    else //if still could manage to create some path use it...
                    {
                        node.MaxPositions = j;
                        gridElements.RemoveElementPositionOwnership(node, prevX, prevY);

                        var connectionPosition = grid.GetCellPosition(prevX, prevY);
                        var connection = GameObject.Instantiate(connectionPrefab, connectionPosition, Quaternion.identity);
                        gridElements.AddElement(connection, prevX, prevY);

                        var index = grid.PositionToIndex(prevX, prevY);
                        availablePositions.Remove(index);
                    }
                    break;
                }

                //if is the last position add a connection
                if (j == nodesSize - 1)
                {
                    gridElements.RemoveElementPositionOwnership(node, prevX, prevY);

                    var connectionPosition = grid.GetCellPosition(prevX, prevY);
                    var connection = GameObject.Instantiate(connectionPrefab, connectionPosition, Quaternion.identity);
                    gridElements.AddElement(connection, prevX, prevY);

                    var index = grid.PositionToIndex(prevX, prevY);
                    availablePositions.Remove(index);
                }
            }
        }

        //clear paths
        foreach (var n in nodes)
        {
            gridElements.ClearAllAdditionalElementPositions(n);
        }
    }

    private static bool CheckPosition(int nx, int ny, Grid grid, ref GridElementsArray gridElements)
    {
        //if is outside grid bounds continue
        if (nx >= grid.gridSize.x || nx < 0 ||
            ny >= grid.gridSize.y || ny < 0)
        {
            return false;
        }

        //if is in a occupied cell
        if (gridElements.elements[nx, ny] != null)
            return false;

        return true;
    }

    private static Vector2Int GetDirection(int index)
    {
        var dir = Vector2Int.right;
        switch (index)
        {
            case 0:
                break;
            case 1:
                dir = Vector2Int.up;
                break;
            case 2:
                dir = Vector2Int.left;
                break;
            case 3:
                dir = Vector2Int.down;
                break;
        }

        return dir;
    }

}