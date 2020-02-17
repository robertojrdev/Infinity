using System.Collections.Generic;
using UnityEngine;

public class ElementPrototype : IGridElement
{
    public List<Vector2Int> gridPositions { get; set; }
    public int MaxPositions;
}