using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Line))]
public class Node : MonoBehaviour
{
    public bool isDragging { get; private set; }
    public Vector2 lastPosition
    {
        get => positions.Count == 0 ? (Vector2)transform.position : positions[positions.Count - 1];
    }

    private Line line;
    private List<Vector2> positions = new List<Vector2>();

    private void Awake()
    {
        line = GetComponent<Line>();
        ResetPositions();
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var pos = new List<Vector2>(positions);
            pos.Add(mousePos);
            line.SetPositions(pos.ToArray());
        }
    }

    public void BeginDrag()
    {
        isDragging = true;
    }

    public void ApplyDrag(Vector2 position)
    {
        isDragging = false;
        AddPoint(position);
    }

    public void CancelDrag()
    {
        isDragging = false;
        line.SetPositions(positions.ToArray());
    }

    public void AddPoint(Vector2 position)
    {
        positions.Add(position);
        line.SetPositions(positions.ToArray());
    }

    [ContextMenu("Reset")]
    public void ResetPositions()
    {
        positions.Clear();
        positions.Add(transform.position);
        line.SetPositions(positions.ToArray());
    }
}