using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileHolderStack : MonoBehaviour
{
    public event System.Action<Tile> OnMyTopTilePressed;
    public event System.Action<TileHolderStack> OnMyAvailableIndicatorPressed;
    public event System.Action<bool> OnOutsideFrame; // bool above

    public InputManager inputManager;
    public Image availableIndicatorImage;

    GameManager gameManager;
    CameraScript cameraScript;

    public bool ActiveIndicator { get { return _activeIndicator; } set { availableIndicatorImage.gameObject.SetActive(value); _activeIndicator = value; } }
    private bool _activeIndicator;

    List<Tile> tiles = new List<Tile>();

    void Start()
    {
        inputManager.OnTilePressed += OnTilePressed;
        inputManager.OnAvailableHolderPressed += OnAvailableHolderPressed;
        availableIndicatorImage.gameObject.SetActive(_activeIndicator);

        gameManager = FindObjectOfType<GameManager>();
        gameManager.OnSomeTilePlaced += OnSomeTilePlaced;

        cameraScript = Camera.main.GetComponent<CameraScript>();
        cameraScript.OnCameraScroll += OnCameraScroll;
    }

    public void AddTile(Tile tile)
    {
        tiles.Add(tile);
        tile.MoveToPosition(availableIndicatorImage.gameObject.transform.position);
        availableIndicatorImage.gameObject.transform.position += Vector3.up * transform.localScale.y;
    }

    public void CheckForAvailableSlot(Tile selectedTile)
    {
        int targetValue = tiles.Count + 1; // cheeky
        ActiveIndicator = selectedTile.tileValue == targetValue;
        //ActiveIndicator = true;
    }

    public void RemoveAvailableSlotIndicator()
    {
        ActiveIndicator = false;
    }

    public Tile GetTopTile()
    {
        if (tiles.Count == 0) return null;
        return tiles[tiles.Count - 1];
    }

    public void OnTilePressed(Tile pressedTile)
    {
        if (tiles.Count == 0) return;

        if (pressedTile.Equals(GetTopTile()))
        {
            OnMyTopTilePressed?.Invoke(GetTopTile());
        }
    }

    public void OnAvailableHolderPressed(GameObject availableHolder)
    {
        if (availableHolder.Equals(availableIndicatorImage.gameObject)) OnMyAvailableIndicatorPressed?.Invoke(this);
    }

    public void RemoveTopTile()
    {
        tiles.RemoveAt(tiles.Count - 1);
        availableIndicatorImage.gameObject.transform.position += Vector3.down * transform.localScale.y;
    }

    void OnSomeTilePlaced(Tile placedTile)
    {
        CheckIfOutsideGameFrame();
    }

    public void CheckIfOutsideGameFrame()
    {
        float topPosition;
        float bottomPosition;
        Ray cameraRayTop = Camera.main.ScreenPointToRay(new Vector3(0, Screen.height));
        Ray cameraRayBottom = Camera.main.ScreenPointToRay(new Vector3(0, 150));
        Plane floor = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        floor.Raycast(cameraRayTop, out rayDistance);
        topPosition = (cameraRayTop.GetPoint(rayDistance) - Camera.main.transform.position).z;

        floor.Raycast(cameraRayBottom, out rayDistance);
        bottomPosition = (cameraRayBottom.GetPoint(rayDistance) - Camera.main.transform.position).z;

        if ((transform.position - Camera.main.transform.position).z > topPosition) OnOutsideFrame?.Invoke(true); // too above
        if ((availableIndicatorImage.gameObject.transform.position - Camera.main.transform.position).z < bottomPosition) OnOutsideFrame?.Invoke(false); // too below
    }

    void OnCameraScroll(bool up)
    {
        foreach (Tile tile in tiles)
        {
            Vector3 position = tile.transform.position + (cameraScript.scrollAmount * (up ? Vector3.forward : Vector3.back));
            tile.MoveToPosition(position);
        }
        availableIndicatorImage.gameObject.transform.position = availableIndicatorImage.gameObject.transform.position + (cameraScript.scrollAmount * (up ? Vector3.forward : Vector3.back));
    }
}
