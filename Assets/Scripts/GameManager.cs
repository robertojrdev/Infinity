using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] private Grid grid;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private Connection connectionPrefab;
    [SerializeField] private Background background;
    [SerializeField] private LevelScriptableFile levelsFile;
    [SerializeField] private MenuManager menuManager;
    public float cameraGridOffset = 0.5f;
    public float cameraGridOffsetDown = 2f;
    public UnityEvent onWinGame;


    private Node draggingNode;
    private GridElementsArray gridElements;
    private bool gameOn = false;
    private Transform objectsHolder;
    private float lastInterstitialTime = 0;
    private GameState gameState;


    public static Vector2Int CurrentLevel
    {
        get => instance.gameState.currentLevel;
        private set => instance.gameState.currentLevel = value;
    }
    public static LevelScriptableFile LevelsFile { get => instance.levelsFile; set => instance.levelsFile = value; }
    public static GameState GameState { get => instance.gameState; }

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
        {
            Debug.LogWarning("Multiple GameManagers not allowed", gameObject);
            Destroy(this);
            return;
        }

        objectsHolder = new GameObject("Grid Holder").transform;
        objectsHolder.position = Vector3.zero;

        background.UpdateColor();

        SetCamera();
        gameState = GameState.LoadOrDefault();
        PlayCurrentLevel();
    }

    private void Update()
    {
        if (!gameOn)
            return;

        if (Input.GetKeyDown(KeyCode.Space)) //TODO: REMOVE THIS
            OnWinGame();

        if (Input.GetMouseButtonDown(0))
            TryStartDrag();

        if (draggingNode != null)
        {
            Drag();
        }

        if (Input.GetMouseButtonUp(0) && draggingNode != null)
        {
            StopDrag();

            var won = CheckForWinGame();
            if (won)
                OnWinGame();
        }
    }

    private void SetCamera()
    {
        var camera = Camera.main;
        var minPos = grid.GetCellPosition(0, 0);
        var maxPos = grid.GetCellPosition(grid.gridSize.x, grid.gridSize.y);

        minPos -= Vector2.one * grid.cellsSize * cameraGridOffset;
        minPos -= Vector2.up * grid.cellsSize * cameraGridOffsetDown;
        maxPos += Vector2.one * grid.cellsSize * cameraGridOffset;

        var size = (maxPos - minPos);
        var center = Vector2.down * grid.cellsSize * (cameraGridOffsetDown / 2);

        var bounds = new Bounds(center, size);

        CameraBounds.Bound(bounds, camera);
    }

    private void Drag()
    {
        //return if has achieved the max num of positions or already is connected
        if (draggingNode.positions.Count > draggingNode.MaxPositions || draggingNode.connection)
            return;

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        draggingNode.Drag(mousePos);

        int x, y = 0;
        if (grid.GetCellAtPosition(mousePos, out x, out y))
        {
            //check if cell is not a node
            if (!(gridElements.elements[x, y] is Node))
            {
                //previous node position
                var prev = draggingNode.gridPositions[draggingNode.gridPositions.Count - 1];

                //check if the grid position that the mouse is above is in an orthogonal direction
                if (Mathf.Abs(prev.x - x) + Mathf.Abs(prev.y - y) <= 1)
                {
                    //get the cell center
                    var cellPos = grid.GetCellPosition(x, y);


                    if (gridElements.elements[x, y] is Connection) //if is a connection connect
                    {
                        //connect
                        var connection = (Connection)gridElements.elements[x, y];

                        if (connection.Connected) //if already has a connection don't connect
                            return;

                        connection.Connected = true;
                        draggingNode.connection = connection;

                        //add point (extend line)
                        draggingNode.AddPoint(cellPos);
                    }
                    else
                    {
                        //add point (extend line)
                        draggingNode.AddPoint(cellPos);
                        gridElements.AddElementPositionOwnership(draggingNode, x, y);
                    }
                }
            }

        }
    }

    private void TryStartDrag()
    {
        int x, y;
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (grid.GetCellAtPosition(mousePos, out x, out y))
        {
            if (gridElements.elements[x, y] is Node)
            {
                var node = (Node)gridElements.elements[x, y];

                //clear previous movements
                gridElements.ClearAllAdditionalElementPositions(node);
                node.ResetPositions();

                //disconnect
                if (node.connection)
                {
                    node.connection.Connected = false;
                    node.connection = null;
                }

                draggingNode = node;
            }
        }
    }

    private void StopDrag()
    {
        draggingNode.CancelDrag();
        draggingNode = null;
    }

    private bool CheckForWinGame()
    {
        foreach (var element in gridElements.elements)
        {
            if (element is Connection)
            {
                var connection = (Connection)element;
                if (!connection.Connected)
                {
                    return false;
                }
            }
            else if (element is Node)
            {
                var node = (Node)element;
                if (node.positions.Count <= node.MaxPositions)
                {
                    return false;
                }
            }
        }

        return true;
    }

    [ContextMenu("Win game")]
    private void OnWinGame()
    {
        onWinGame.Invoke();

        ClearCurrentGame();
        var previousLevel = CurrentLevel;
        levelsFile.GetNextLevel(CurrentLevel, ref gameState.currentLevel);

        gameState.SaveGame();

        if (previousLevel.x != CurrentLevel.x)
            background.UpdateColor();

        StartCoroutine(ShowAddAndStartLevel());
    }

    private void ClearCurrentGame()
    {
        if (gridElements.elements == null)
            return;

        foreach (var el in gridElements.elements)
        {
            if (el != null)
                Destroy(((Component)el).gameObject);
        }
    }

    private IEnumerator ShowAddAndStartLevel()
    {
        if (Time.time > lastInterstitialTime + 60)
        {
            lastInterstitialTime = Time.time;
            gameOn = false;

            AdsManager.DisplayInterstitial();
            yield return new WaitWhile(() => AdsManager.isInterstitialVisible);

            gameOn = true;
        }

        PlayCurrentLevel();
    }

    private void PlayCurrentLevel()
    {
        Level level;

        if (levelsFile.HasLevel(CurrentLevel))
            level = levelsFile.levels[CurrentLevel.x].Levels[CurrentLevel.y];
        else //create a procedural game if has no more levels
            level = LevelGenerator.CreateNewLevel(8, 10, grid.gridSize);


        InstantiateLevel(level);
    }

    private void InstantiateLevel(Level level)
    {
        StartCoroutine(ZoomGame(false));
        gridElements = grid.GetGridElementsArray();

        for (int i = 0; i < level.nodes.Length; i++)
        {
            var pos = level.nodes[i];
            var position = grid.GetCellPosition(pos.x, pos.y);
            var node = Instantiate(nodePrefab, position, Quaternion.identity, objectsHolder);
            node.MaxPositions = level.nodesMaxPositions[i];
            gridElements.AddElement(node, pos.x, pos.y);
        }

        foreach (var pos in level.connections)
        {
            var position = grid.GetCellPosition(pos.x, pos.y);
            var connection = Instantiate(connectionPrefab, position, Quaternion.identity, objectsHolder);
            gridElements.AddElement(connection, pos.x, pos.y);
        }

        gameOn = true;
    }

    public static void LoadNewGame(Vector2Int level)
    {
        instance.ClearCurrentGame();
        CurrentLevel = level;
        var levelFile = LevelsFile.levels[CurrentLevel.x].Levels[CurrentLevel.y];
        instance.InstantiateLevel(levelFile);
    }

    public void PauseGame()
    {
        gameOn = false;
        StartCoroutine(ZoomGame(true));
        menuManager.OpenMenu();
    }

    public void ResumeGame()
    {
        gameOn = true;
        StartCoroutine(ZoomGame(false));
        menuManager.CloseMenus();
    }

    IEnumerator ZoomGame(bool zoom)
    {
        var from = objectsHolder.localScale;
        var to = zoom ? Vector3.one * 0.7f : Vector3.one;

        float timer = 0;
        float time = 0.25f;
        while (timer <= time)
        {
            timer += Time.deltaTime;
            var t = Mathf.InverseLerp(0, time, timer);
            t = Easing.EaseOutQuad(0, 1, t);
            objectsHolder.transform.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }

        objectsHolder.transform.localScale = to;
    }
}