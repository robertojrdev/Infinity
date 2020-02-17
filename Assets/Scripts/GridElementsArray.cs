using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GridElementsArray
{
    public IGridElement[,] elements;

    public GridElementsArray(int x, int y)
    {
        elements = new IGridElement[x, y];
    }

    public void AddElement(IGridElement element, int x, int y)
    {
        if(elements[x, y] != null)
        {
            Debug.LogWarning("The position " + x + ", " + y + " is already taken");
            return;
        }

        elements[x, y] = element;
        element.gridPositions = new List<Vector2Int>();
        element.gridPositions.Add(new Vector2Int(x, y));
    }

    public bool AddElementPositionOwnership(IGridElement element, int x, int y)
    {
        if (elements[x, y] != null)
        {
            Debug.LogWarning("the position already has an owner");
            return false;
        }

        elements[x, y] = element;
        element.gridPositions.Add(new Vector2Int(x, y));
        return true;
    }

    public bool RemoveElementPositionOwnership(IGridElement element, int x, int y)
    {
        if (elements[x, y] != element)
        {
            Debug.LogWarning("the element has not the ownership of this position");
            return false;
        }

        elements[x, y] = null;
        var index = element.gridPositions.FindIndex(p => p.x == x && p.y == y);
        if (index != -1)
            element.gridPositions.RemoveAt(index);

        return true;
    }

    public void ClearAllAdditionalElementPositions(IGridElement element)
    {
        if (element.gridPositions == null)
            return;

        var posCount = element.gridPositions.Count;
        for (int i = 1; i < posCount; i++) //start at one to skip the element position
        {
            var (x, y) = (element.gridPositions[i].x, element.gridPositions[i].y);
            elements[x,y] = null;
        }

        element.gridPositions.RemoveRange(1, posCount -1);
    }

    public void RemoveElement(IGridElement element)
    {
        foreach (var pos in element.gridPositions)
        {
            elements[pos.x, pos.y] = null;
        }

        element.gridPositions = null;
    }
}