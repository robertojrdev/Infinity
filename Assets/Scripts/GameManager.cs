using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Grid grid;
    public Node nodePrefab;

    private Dictionary<int, Node> nodes = new Dictionary<int, Node>();
    private int draggingIndex = -1;
    private Vector2 currentDragPosition;

    private void Start()
    {
        var position = grid.GetCellPosition(2, 3);
        var index = grid.PositionToIndex(2, 3);
        var node = Instantiate(nodePrefab, position, Quaternion.identity);
        nodes.Add(index, node);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int x, y;
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (grid.GetCellAtPosition(mousePos, out x, out y))
            {
                var index = grid.PositionToIndex(x, y);
                if (nodes.ContainsKey(index))
                {
                    var node = nodes[index];

                    node.ResetPositions();
                    node.BeginDrag();
                    draggingIndex = index;

                    var lastPos = node.lastPosition;
                    grid.GetCellAtPosition(lastPos, out x, out y);
                    currentDragPosition = new Vector2(x , y);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && draggingIndex != -1)
        {
            int x, y;
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if (grid.GetCellAtPosition(mousePos, out x, out y))
            {
                var cellPos = grid.GetCellPosition(x,y);
                nodes[draggingIndex].ApplyDrag(cellPos);
            }
            else
                nodes[draggingIndex].CancelDrag();

            draggingIndex = -1;
        }
    }
}