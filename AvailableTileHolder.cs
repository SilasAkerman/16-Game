using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailableTileHolder : MonoBehaviour
{
    public event System.Action<Tile> OnMyTilePressed;
    public event System.Action<bool> OnMyTileChanged;

    public Transform Position3D;
    public InputManager inputManager;

    public Tile currentTile;

    void Start()
    {
        inputManager.OnTilePressed += OnTilePressed;
        Camera.main.GetComponent<CameraScript>().OnCameraScroll += OnCameraScroll;
    }

    public void AddTileFromPool(Tile tile, int value)
    {
        Tile newTile = Instantiate(tile, Position3D.position + Vector3.back * 4, Quaternion.identity);
        newTile.tileValue = value;
        AddTile(newTile);
    }

    public void AddTile(Tile tile)
    {
        currentTile = tile;
        currentTile.MoveToPosition(Position3D.position);
        OnMyTileChanged?.Invoke(true);
    }

    public void RemoveTile()
    {
        currentTile = null;
        OnMyTileChanged?.Invoke(false);
    }

    void OnTilePressed(Tile pressedTile)
    {
        if (pressedTile.Equals(currentTile))
        {
            OnMyTilePressed?.Invoke(currentTile);
        }
    }

    void OnCameraScroll(bool up)
    {
        Vector3 position = Position3D.position + (Camera.main.GetComponent<CameraScript>().scrollAmount * (up ? Vector3.forward : Vector3.back));
        if (currentTile) currentTile.MoveToPosition(position);
        Position3D.position = position;
    }
}
