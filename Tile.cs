using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public event System.Action<int> OnPressed; // This will probably go unused, but whatever

    public static Vector2 TileSize;
    public Text valueText;
    public Image selectedIndicatorImage;

    public int tileValue { get { return _tileValue; } set { valueText.text = value.ToString(); _tileValue = value; } }
    private int _tileValue;

    [SerializeField]
    private float moveSpeed;

    public bool selected { get { return _selected; } set { selectedIndicatorImage?.gameObject.SetActive(value); _selected = value; } }
    private bool _selected;

    void Awake()
    {
        TileSize = new Vector2(transform.localScale.x, transform.localScale.z);
    }

    private void Start()
    {
        selectedIndicatorImage?.gameObject.SetActive(_selected); // Pretty much always false
    }

    public void MoveToPosition(Vector3 position)
    {
        StartCoroutine(StartMovingPosition(position));
    }

    public void MoveToPosition(Vector3 position, Vector3 rotation)
    {
        MoveToPosition(position);
    }

    IEnumerator StartMovingPosition(Vector3 position)
    {
        Vector3 startPosition = transform.position;
        float percentage = 0;

        while (percentage < 1)
        {
            percentage += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPosition, position, percentage);
            yield return null;
        }
    }

    IEnumerator StartMovingRotation(Vector3 rotation)
    {
        yield return null;
    }

    public void SetFrontUI(bool value)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = 0; //Default
        }
    }
}
