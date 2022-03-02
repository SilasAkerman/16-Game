using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    GameManager gameManager;
    MainUIManager uiManager;

    public GameObject undoButton;

    [SerializeField]
    List<TileAction> tileActions;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        gameManager.OnSomeTileMoved += RecordTileMovement;
        uiManager = FindObjectOfType<MainUIManager>();
        uiManager.OnMainMenuReturned += MainMenuReturned;
    }

    void RecordTileMovement(Tile movedTile, GameManager.TileDestination fromPosition, GameManager.TileDestination toPosition)
    {
        tileActions.Add(new TileAction(movedTile, fromPosition, toPosition, gameManager));
        uiManager.UndoController = true;
    }

    void MainMenuReturned()
    {
        uiManager.UndoController = tileActions.Count > 0;
    }

    public void Undo()
    {
        PerformUndo(tileActions[tileActions.Count-1]);

        tileActions.RemoveAt(tileActions.Count-1);
        if (tileActions.Count <= 0)
        {
            uiManager.UndoController = false;
        }
    }

    void PerformUndo(TileAction action)
    {
        switch (action.FromPosition)
        {
            case GameManager.TileDestination.Pool:
                gameManager.AddBackValueToList(action.MovedTile.tileValue);
                action.MovedTile.MoveToPosition(action.MovedTile.transform.position + Vector3.back * 4);
                Destroy(action.MovedTile.gameObject, 3); // Maybe a bit too much delay
                break;

            case GameManager.TileDestination.AvailableHolder:
                gameManager.tileHolder.AddTile(action.MovedTile);
                break;

            case GameManager.TileDestination.HolderRow:
                action.fromHolderRow.AddTile(action.MovedTile);
                break;
        }

        switch (action.ToPosition)
        {
            case GameManager.TileDestination.HolderStack:
                action.toHolderStack.RemoveTopTile();
                break;

            case GameManager.TileDestination.HolderRow:
                action.toHolderRow.RemoveTopTile();
                break;

            case GameManager.TileDestination.AvailableHolder:
                gameManager.tileHolder.RemoveTile();
                break;
        }

        gameManager.CheckIfAnythingOutsideFrame();
    }

    [System.Serializable]
    public class TileAction
    {
        GameManager gameManager; // shoudl be singleton

        public Tile MovedTile;
        public GameManager.TileDestination FromPosition;
        public GameManager.TileDestination ToPosition;

        public TileHolderRow fromHolderRow; // And this is why we use abstract classes and inheritance
        public TileHolderRow toHolderRow;
        public TileHolderStack fromHolderStack;
        public TileHolderStack toHolderStack;

        public TileAction(Tile movedTile, GameManager.TileDestination fromPosition, GameManager.TileDestination toPosition, GameManager aGameManager)
        {
            gameManager = aGameManager; // so messy
            MovedTile = movedTile;
            FromPosition = fromPosition;
            ToPosition = toPosition;

            switch (fromPosition)
            {
                case GameManager.TileDestination.HolderRow:
                    fromHolderRow = gameManager.LastMovedFromRow;
                    break;

                //case GameManager.TileDestination.HolderStack: // From stack will never happen
                //    fromHolderStack = gameManager.LastMovedStack;
                //    break;
            }

            switch (toPosition)
            {
                case GameManager.TileDestination.HolderRow:
                    toHolderRow = gameManager.LastMovedToRow;
                    break;

                case GameManager.TileDestination.HolderStack:
                    toHolderStack = gameManager.LastMovedToStack;
                    break;
            }
        }

        
    }
}
