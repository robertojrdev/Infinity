using System;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Min(0)] public Vector2Int gridSize = new Vector2Int(5, 10);
    public float cellsSize = 5;
    public float cellsOffset = 0.2f;
    public bool debugLines = true;

    public void LoopThroughGrid(Action<int, int, Vector2> action)
    {
        var (mapSize, bottomLeftPos) = GetGridDimensions();
        var cellFullSize = cellsOffset + cellsSize;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                var position = new Vector2(cellFullSize * x, cellFullSize * y);
                position += bottomLeftPos;  //start from bottom
                position += Vector2.one * cellFullSize / 2; //center the cell

                action.Invoke(x, y, position);
            }
        }
    }

    public bool GetCellAtPosition(Vector2 position, out int x, out int y)
    {
        var (mapSize, bottomLeftPos) = GetGridDimensions();
        //is position inside the grid?
        if (position.x < bottomLeftPos.x ||
            position.x > (bottomLeftPos + mapSize).x ||
            position.y < bottomLeftPos.y ||
            position.y > (bottomLeftPos + mapSize).y)
        {
            x = -1;
            y = -1;
            return false;
        }

        var topRightPos = bottomLeftPos + mapSize;

        var tx = Mathf.InverseLerp(bottomLeftPos.x, topRightPos.x, position.x);
        x = (int)Mathf.Lerp(0, gridSize.x, tx);
        var ty = Mathf.InverseLerp(bottomLeftPos.y, topRightPos.y, position.y);
        y = (int)Mathf.Lerp(0, gridSize.y, ty);

        return true;
    }

    public Vector2 GetCellPosition(int x, int y)
    {
        var (mapSize, bottomLeftPos) = GetGridDimensions();
        var cellFullSize = cellsOffset + cellsSize;
        var position = new Vector2(cellFullSize * x, cellFullSize * y);
        position += bottomLeftPos;  //start from bottom
        position += Vector2.one * cellFullSize / 2; //center the cell

        return position;
    }

    public int PositionToIndex(int x, int y)
    {
        return y * gridSize.x + x;
    }

    public (int, int) IndexToPosition(int i)
    {
        var y = (i / gridSize.x);
        var x =  i - (y * gridSize.x);

        return (x, y);
    }

    /// <summary>
    /// return - mapSize, bottomLeftPosition
    /// </summary>
    public (Vector2, Vector2) GetGridDimensions()
    {
        var offset = (Vector2)gridSize * cellsOffset;
        var mapSize = (Vector2)gridSize * cellsSize + offset;
        var bottomLeftPos = -mapSize / 2;

        return (mapSize, bottomLeftPos);
    }

    private void OnDrawGizmos()
    {
        if (!debugLines)
            return;

        var (mapSize, bottomLeftPos) = GetGridDimensions();
        var cellPos = cellsOffset + cellsSize;

        for (int x = 0; x <= gridSize.x; x++)
        {
            var fromPos = bottomLeftPos + x * cellPos * Vector2.right;
            var toPos = fromPos + Vector2.up * mapSize.y;
            Gizmos.DrawLine(fromPos, toPos);
        }

        for (int y = 0; y <= gridSize.y; y++)
        {
            var fromPos = bottomLeftPos + y * cellPos * Vector2.up;
            var toPos = fromPos + Vector2.right * mapSize.x;
            Gizmos.DrawLine(fromPos, toPos);
        }
    }

}
