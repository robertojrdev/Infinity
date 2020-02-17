using UnityEngine;

[System.Serializable]
public struct Level
{
    public Vector2Int[] nodes;
    public int[] nodesMaxPositions ;
    public Vector2Int[] connections ;

    public Level(ElementPrototype[] nodes, ElementPrototype[] connections)
    {
        this.nodes = new Vector2Int[nodes.Length];
        this.nodesMaxPositions = new int[nodes.Length];
        this.connections = new Vector2Int[connections.Length];

        for (int i = 0; i < nodes.Length; i++)
        {
            this.nodes[i] = nodes[i].gridPositions[0];
            this.nodesMaxPositions[i] = nodes[i].MaxPositions;
        }

        for (int i = 0; i < connections.Length; i++)
        {
            this.connections[i] = connections[i].gridPositions[0];
        }
    }
}