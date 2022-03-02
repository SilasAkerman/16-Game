using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event System.Action OnAddNewAvailableTile;
    public event System.Action<Tile> OnSomeTilePlaced;
    public event System.Action<Tile, TileDestination, TileDestination> OnSomeTileMoved;

    public enum GameState { Gameplay, Menu };
    public static GameState CurrentGameState { get; }
    static GameState _currentGameState = GameState.Gameplay;

    public AvailableTileHolder tileHolder;
    public TileHolderRow[] tileHolderRows = new TileHolderRow[6];
    public TileHolderStack[] tileHolderStacks = new TileHolderStack[6];

    public InputManager inputManager;
    public MainUIManager mainUIManager;
    public CameraScript cameraScript;

    public Tile tilePrefab;
    List<int> availableTileValues = new List<int>();

    public Tile SelectedTile { get; set; }
    public enum TileDestination { Pool, AvailableHolder, HolderRow, HolderStack }
    TileDestination selectedTilePosition;

    public bool IsLastTile { get; private set; }

    public TileHolderRow LastMovedToRow;
    public TileHolderStack LastMovedToStack;
    public TileHolderRow LastMovedFromRow;
    public TileHolderStack LastMovedFromStack;

    void Start()
    {
        TestCreateInitialTiles();

        inputManager.OnNothingPressed += OnNothingPressed;
        inputManager.OnTilePressed += OnSomeTilePressed;
        tileHolder.OnMyTilePressed += OnAvailableTileHolderPressed;
        foreach (TileHolderRow holderRow in tileHolderRows)
        {
            holderRow.OnMyTopTilePressed += OnTileHolderRowPressedTop;
            holderRow.OnMyAvailableIndicatorPressed += OnTileHolderRowPressedAvailable;
            holderRow.OnOutsideFrame += OnSomeHolderPositionOutsideView;
        }
        foreach(TileHolderStack holderStack in tileHolderStacks)
        {
            holderStack.OnMyTopTilePressed += OnTileHolderStackPressedTop;
            holderStack.OnMyAvailableIndicatorPressed += OnTileHolderStackPressedAvailable;
            holderStack.OnOutsideFrame += OnSomeHolderPositionOutsideView;
        }

        cameraScript.OnCameraFinishScroll += OnCameraFinishScroll;
    }

    void Update() // For testing
    {
        if (Input.GetKeyDown(KeyCode.Space) && !tileHolder.currentTile) TestAddRandomTile();
    }

    void TestCreateInitialTiles() // Only for testing, will be replaced by proper UI
    {
        for (int value = 1; value <= 16; value++)
        {
            for (int amount = 0; amount < 6; amount++)
            {
                availableTileValues.Add(value);
            }
        }
        availableTileValues = new List<int>(Utility.ShuffleArray<int>(availableTileValues.ToArray(), Random.Range(0, int.MaxValue)));
    }

    void TestAddRandomTile()
    {
        AddNewTileIndex(0);
    }

    public void AddNewTile(int value)
    {
        for (int i = 0; i < availableTileValues.Count; i++)
        {
            if (availableTileValues[i] == value)
            {
                AddNewTileIndex(i);
                break;
            }
        }
    }

    public void AddNewTileIndex(int index)
    {
        int value = availableTileValues[index];
        availableTileValues.RemoveAt(index);
        CheckIfLastTile();
        tileHolder.AddTileFromPool(tilePrefab, value);
        OnSomeTileMoved?.Invoke(tileHolder.currentTile, TileDestination.Pool, TileDestination.AvailableHolder);
        OnAddNewAvailableTile?.Invoke();
        OnAvailableTileHolderPressed(tileHolder.currentTile);
        print(availableTileValues.Count + 1);
    }

    public int GetAmount (int value)
    {
        int count = 0;
        foreach(int tileValue in availableTileValues)
        {
            if (tileValue == value) count++;
        }

        return count;
    }

    public void AddBackValueToList(int value) // Ads in front
    {
        availableTileValues.Insert(0, value);
        CheckIfLastTile();
    }

    public void OnNewTileButtonPressed()
    {
        TestAddRandomTile();
    }

    void SelectTile(Tile pressedTile, TileDestination fromPosition)
    {
        UnselectCurrentTile();
        pressedTile.selected = true;
        SelectedTile = pressedTile;
        selectedTilePosition = fromPosition;
    }

    void UnselectCurrentTile()
    {
        if (!SelectedTile) return;

        SelectedTile.selected = false;
        SelectedTile = null;

        foreach (TileHolderRow holderRow in tileHolderRows)
        {
            holderRow.RemoveAvailableSlotIndicator();
        }
        foreach (TileHolderStack holderStack in tileHolderStacks)
        {
            holderStack.RemoveAvailableSlotIndicator();
        }
    }

    void CheckAvailableIndicatorRows()
    {
        foreach(TileHolderRow holderRow in tileHolderRows)
        {
            holderRow.CheckForAvailableSlot(SelectedTile);
        }
    }

    void CheckAvailableIndicatorStacks()
    {
        foreach (TileHolderStack holderStack in tileHolderStacks)
        {
            holderStack.CheckForAvailableSlot(SelectedTile);
        }
    }

    void RemoveSelectedTileFromPosition(bool action = true)
    {
        switch (selectedTilePosition)
        {
            case TileDestination.AvailableHolder:
                tileHolder.RemoveTile();
                break;

            case TileDestination.HolderRow:
                foreach (TileHolderRow holderRow in tileHolderRows)
                {
                    if (holderRow.GetTopTile() != null && holderRow.GetTopTile().Equals(SelectedTile))
                    {
                        LastMovedFromRow = holderRow;
                        holderRow.RemoveTopTile();
                        break;
                    }
                }
                break;

            case TileDestination.HolderStack:
                foreach (TileHolderStack holderStack in tileHolderStacks)
                {
                    if (holderStack.GetTopTile() != null && holderStack.GetTopTile().Equals(SelectedTile))
                    {
                        LastMovedFromStack = holderStack;
                        holderStack.RemoveTopTile();
                        break;
                    }
                }
                break;
        }
    }

    void OnAvailableTileHolderPressed(Tile pressedTile)
    {
        SelectTile(pressedTile, TileDestination.AvailableHolder);
        CheckAvailableIndicatorRows();
        CheckAvailableIndicatorStacks();
    }

    void OnTileHolderRowPressedTop(Tile pressedTile)
    {
        SelectTile(pressedTile, TileDestination.HolderRow);
        CheckAvailableIndicatorStacks();
    }

    void OnTileHolderRowPressedAvailable(TileHolderRow holderRowPressed)
    {
        RemoveSelectedTileFromPosition();
        holderRowPressed.AddTile(SelectedTile);
        LastMovedToRow = holderRowPressed;
        OnSomeTilePlaced?.Invoke(SelectedTile);
        OnSomeTileMoved?.Invoke(SelectedTile, selectedTilePosition, TileDestination.HolderRow);
        UnselectCurrentTile();
    }

    void OnTileHolderStackPressedTop(Tile pressedTile)
    {

    }

    void OnTileHolderStackPressedAvailable(TileHolderStack holderStackPressed)
    {
        RemoveSelectedTileFromPosition();
        holderStackPressed.AddTile(SelectedTile);
        LastMovedToStack = holderStackPressed;
        OnSomeTilePlaced?.Invoke(SelectedTile);
        OnSomeTileMoved?.Invoke(SelectedTile, selectedTilePosition, TileDestination.HolderStack);
        UnselectCurrentTile();
    }

    void OnNothingPressed()
    {
        if (CurrentGameState == GameState.Gameplay) UnselectCurrentTile();
    }

    void OnSomeTilePressed(Tile pressedTile)
    {
        if (!pressedTile.Equals(SelectedTile)) UnselectCurrentTile();
    }

    void OnSomeHolderPositionOutsideView(bool above)
    {
        if (above) mainUIManager.CameraControlUpper = true;
        else mainUIManager.CameraControlLower = true;
    }

    void OnCameraFinishScroll()
    {
        CheckIfAnythingOutsideFrame();
    }

    public void CheckIfAnythingOutsideFrame()
    {
        foreach (TileHolderRow tileHolderRow in tileHolderRows)
        {
            tileHolderRow.CheckIfOutsideGameFrame();
        }
        foreach (TileHolderStack tileHolderStack in tileHolderStacks)
        {
            tileHolderStack.CheckIfOutsideGameFrame();
        }
    }

    public void ChangeGameState(GameState stateTo)
    {
        _currentGameState = stateTo;
    }

    void CheckIfLastTile()
    {
        IsLastTile = availableTileValues.Count <= 0;
    }
}
