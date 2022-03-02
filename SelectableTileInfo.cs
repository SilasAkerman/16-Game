using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableTileInfo : MonoBehaviour
{
    public event System.Action<int> OnTilePressed;

    public Text amountText;
    public int startingValue = 0;

    GameManager gameManager;
    Tile tileScript;

    Image buttonImage;
    Image amountImage;

    bool available;

    public int Value { get { return tileScript.tileValue; } set { tileScript.tileValue = value; } }
    public int Amount { get { return _amount; } set { amountText.text = value.ToString(); _amount = value; } }
    int _amount;
    public Tile Script { get { return tileScript; } }

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        tileScript = GetComponent<Tile>();
        tileScript.tileValue = startingValue;
        buttonImage = GetComponent<Image>();
        amountImage = transform.Find("Image").GetComponent<Image>(); // pretty ugly but who cares
    }

    public void UpdateValues()
    {
        Amount = gameManager.GetAmount(Value);
        available = Amount > 0;
        buttonImage.color = available ? Color.white : Color.grey;
        amountImage.color = available ? Color.white : Color.grey;
    }

    public void ButtonClick()
    {
        if (available) OnTilePressed?.Invoke(Value);
    }
}
