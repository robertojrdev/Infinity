using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator
{
    public static Level CreateNewLevel(int nodesCount, int nodesSize, Vector2Int gridSize)
    {
        var gridElements = new GridElementsArray(gridSize.x, gridSize.y);

        //define available positions
        var cellsCount = gridSize.x * gridSize.y;
        List<int> availablePositions = new List<int>();
        for (int i = 0; i < cellsCount; i++)
            availablePositions.Add(i);

        //store nodes to clear their paths
        var nodes = new List<ElementPrototype>();
        var connections = new List<ElementPrototype>();

        for (int i = 0; i < nodesCount; i++)
        {
            //get an available position
            var nodeIndex = UnityEngine.Random.Range(0, availablePositions.Count);
            var nodePositionIndex = availablePositions[nodeIndex];
            availablePositions.RemoveAt(nodeIndex);

            //create new node
            var (x, y) = Grid.IndexToPosition(nodePositionIndex, gridSize);
            var node = new ElementPrototype();
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
                    foundValidPosition = CheckPosition(nx, ny, gridSize, ref gridElements);

                    //if position is not available try the next
                    if (!foundValidPosition)
                        continue;

                    //FOUND AN EMPTY CELL

                    //mark position as taken, update previous position and continue looking for the next position
                    gridElements.AddElementPositionOwnership(node, nx, ny);
                    var index = Grid.PositionToIndex(nx, ny, gridSize);
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
                    }
                    else //if still could manage to create some path use it...
                    {
                        node.MaxPositions = j;
                        gridElements.RemoveElementPositionOwnership(node, prevX, prevY);

                        var connection = new ElementPrototype();
                        connections.Add(connection);
                        gridElements.AddElement(connection, prevX, prevY);

                        var index = Grid.PositionToIndex(prevX, prevY, gridSize);
                        availablePositions.Remove(index);
                    }
                    break;
                }

                //if is the last position add a connection
                if (j == nodesSize - 1)
                {
                    gridElements.RemoveElementPositionOwnership(node, prevX, prevY);

                    var connection = new ElementPrototype();
                    connections.Add(connection);
                    gridElements.AddElement(connection, prevX, prevY);

                    var index = Grid.PositionToIndex(prevX, prevY, gridSize);
                    availablePositions.Remove(index);
                }
            }
        }

        return new Level(nodes.ToArray(), connections.ToArray());
    }

    private static bool CheckPosition(int nx, int ny, Vector2Int gridSize, ref GridElementsArray gridElements)
    {
        //if is outside grid bounds continue
        if (nx >= gridSize.x || nx < 0 ||
            ny >= gridSize.y || ny < 0)
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