using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Line))]
public class Node : MonoBehaviour, IGridElement
{
    [Min(0), SerializeField] private int maxPositions = 3;
    [SerializeField] private TextMesh text;

    public Vector2 lastPosition
    {
        get => positions.Count == 0 ? (Vector2)transform.position : positions[positions.Count - 1];
    }

    private Line line;
    public List<Vector2> positions { get; private set; } = new List<Vector2>();
    public List<Vector2Int> gridPositions { get; set; }
    public Connection connection { get; set; }
    public int MaxPositions { get => maxPositions; set { maxPositions = value; UpdateText(); } }

    private void Awake()
    {
        line = GetComponent<Line>();
        ResetPositions();
    }

    private void UpdateText()
    {
        text.text = (MaxPositions - positions.Count + 1).ToString();
    }

    public void Drag(Vector2 position)
    {
        var pos = new List<Vector2>(positions);
        pos.Add(position);
        line.SetPositions(pos.ToArray());
    }

    public void CancelDrag()
    {
        line.SetPositions(positions.ToArray());
    }

    public void AddPoint(Vector2 position)
    {
        positions.Add(position);
        line.SetPositions(positions.ToArray());
        UpdateText();
    }

    [ContextMenu("Reset")]
    public void ResetPositions()
    {
        positions.Clear();
        positions.Add(transform.position);
        line.SetPositions(positions.ToArray());
        UpdateText();
    }

    // public void SetColor(Color)
    // {

    // }
}