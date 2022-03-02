using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    public event System.Action OnMainMenuReturned;

    GameManager gameManager;
    CameraScript cameraScript;

    public AvailableTileHolder tileHolder;
    public GameObject addNewRandomTileButton;
    public GameObject selectNewTileButton;

    public GameObject CameraControlUpperButton;
    public GameObject CameraControlLowerButton;

    public GameObject UndoButton;
    public GameObject PauseButton;

    public GameObject FinishButton;

    public SelectTilesScreenManager selectTilesScreen;
    public PauseMenu pauseMenuScreen;
    public FinishScreen finishScreen;

    public bool CameraControlUpper { get { return CameraControlUpperButton.activeSelf; } set { CameraControlUpperButton.SetActive(value); } }
    public bool CameraControlLower { get { return CameraControlLowerButton.activeSelf; } set { CameraControlLowerButton.SetActive(value); } }
    public bool SelectController { set { selectNewTileButton.SetActive(value && PlayerPrefs.GetInt("select", 1) == 1); } }
    public bool UndoController { set { UndoButton.SetActive(value && PlayerPrefs.GetInt("undo", 1) == 1); } }
    public bool PauseController { set { PauseButton.SetActive(value); } }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        tileHolder.OnMyTileChanged += OnAvailableTileHolderChangedTile;

        cameraScript = FindObjectOfType<CameraScript>();
        cameraScript.OnCameraScroll += OnCameraScroll;
        cameraScript.OnCameraFinishScroll += OnCameraFinishScroll;

        addNewRandomTileButton.SetActive(true);
        SelectController = true;
        CameraControlUpper = false;
        CameraControlLower = false;
        UndoController = false;
        PauseController = true;

        selectTilesScreen.OnScreenHasFinished += OnSelectScreenFinished;
        pauseMenuScreen.OnFinished += OnPauseFinished;
        OnMainMenuReturned += MainMenuReturned;
    }

    void OnAvailableTileHolderChangedTile(bool hasTile)
    {
        if (!gameManager.IsLastTile)
        {
            addNewRandomTileButton.SetActive(!hasTile);
            SelectController = !hasTile;
        }
        else
        {
            addNewRandomTileButton.SetActive(false);
            SelectController = false;
            FinishButton.SetActive(!hasTile);
        }
    }

    void OnCameraScroll(bool up)
    {
        CameraControlUpper = false;
        CameraControlLower = false;
    }

    void OnCameraFinishScroll()
    {
        gameManager.CheckIfAnythingOutsideFrame();
    }

    public void OnSelectNewTilePressed()
    {
        addNewRandomTileButton.SetActive(false);
        SelectController = false;
        CameraControlUpper = false;
        CameraControlLower = false;
        UndoController = false;
        PauseController = false;

        selectTilesScreen.ChangeActivation(true);
    }

    void OnSelectScreenFinished(Tile selectedTile)
    {
        OnMainMenuReturned?.Invoke();
        if (selectedTile is null)
        {
            if (!gameManager.IsLastTile)
            {
                addNewRandomTileButton.SetActive(true);
                SelectController = true;
            }
            else
            {
                FinishButton.SetActive(true);
            }
            return;
        }
        if (selectedTile.tileValue > 0)
        {
            gameManager.AddNewTile(selectedTile.tileValue);
        }
    }

    public void OnPausePressed()
    {
        addNewRandomTileButton.SetActive(false);
        SelectController = false;
        CameraControlUpper = false;
        CameraControlLower = false;
        UndoController = false;
        PauseController = false;
        FinishButton.SetActive(false);

        pauseMenuScreen.ChangeActivation(true);
    }

    void OnPauseFinished()
    {
        OnMainMenuReturned?.Invoke();
        if (gameManager.tileHolder.currentTile == null)
        {
            if (!gameManager.IsLastTile)
            {
                addNewRandomTileButton.SetActive(true);
                SelectController = true;
            }
            else
            {
                FinishButton.SetActive(true);
            }
        }
    }

    public void OnFinishPressed()
    {
        addNewRandomTileButton.SetActive(false);
        SelectController = false;
        CameraControlUpper = false;
        CameraControlLower = false;
        UndoController = false;
        PauseController = false;
        FinishButton.SetActive(false);

        Invoke(nameof(AppearFinishMenu), 1);
    }

    void AppearFinishMenu()
    {
        finishScreen.ChangeActivation();
    }

    void MainMenuReturned()
    {
        gameManager.CheckIfAnythingOutsideFrame();
        PauseController = true;
    }
}
