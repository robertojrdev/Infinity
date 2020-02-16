using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour, IGridElement
{
    [SerializeField] private Sprite defaltSprite;
    [SerializeField] private Sprite connectedSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool connected;

    public bool Connected
    {
        get => connected;
        set
        {
            connected = value;
            spriteRenderer.sprite = connected ? connectedSprite : defaltSprite;
        }
    }

    public List<Vector2Int> gridPositions { get; set; }

    private void Awake() {
        Connected = false;
    }
}