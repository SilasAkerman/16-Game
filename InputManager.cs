using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public LayerMask tileMask;
    
    Camera viewCamera;

    public event System.Action<Tile> OnTilePressed;
    public event System.Action<GameObject> OnAvailableHolderPressed;
    public event System.Action OnNothingPressed;

    private void Start()
    {
        viewCamera = Camera.main;
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return; // The mouse is pressed

        if (GameManager.CurrentGameState == GameManager.GameState.Gameplay)
        {
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.red, 3);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500, tileMask, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Tile"))
                {
                    Tile pressedTile = hit.collider.GetComponent<Tile>();
                    OnTilePressed?.Invoke(pressedTile);
                }
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("AvailableHoldIndicator"))
                {
                    OnAvailableHolderPressed?.Invoke(hit.collider.gameObject);
                }
            }
            else
            {
                OnNothingPressed?.Invoke();
            }
        }
    }
}
