using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTilesScreenManager : MonoBehaviour
{
    public event System.Action<Tile> OnScreenHasFinished;

    RectTransform rectTransform;
    bool isActive;

    GameManager gameManager;

    public GameObject backButton;
    public GameObject confirmButton;

    public SelectableTileInfo[] selectableTiles;
    public GameObject activeTileObj;
    SelectableTileInfo activeTile;

    void Start()
    {
        rectTransform.anchoredPosition = Vector2.down * 700;
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gameManager = FindObjectOfType<GameManager>();
        activeTile = activeTileObj.GetComponent<SelectableTileInfo>();

        foreach(SelectableTileInfo selectableTile in selectableTiles)
        {
            selectableTile.OnTilePressed += SelectableTilePressed;
        }
    }

    public void ChangeActivation(bool activate)
    {
        if (activate)
        {
            gameManager.ChangeGameState(GameManager.GameState.Menu);

            backButton.SetActive(true);
            confirmButton.SetActive(false);
            activeTileObj.SetActive(false);
            foreach(SelectableTileInfo selectableTile in selectableTiles)
            {
                selectableTile.UpdateValues();
            }
            StartCoroutine(Animate(true));
        }
        else
        {
            isActive = false;
            StartCoroutine(Animate(false));
            gameManager.ChangeGameState(GameManager.GameState.Gameplay);
        }

    }

    public void BackButtonPressed()
    {
        if (isActive)
        {
            OnScreenHasFinished?.Invoke(null);
            ChangeActivation(false);
        }
    }

    public void ConfirmButtonPressed()
    {
        if (isActive)
        {
            OnScreenHasFinished?.Invoke(activeTile.Script);
            ChangeActivation(false);
        }
    }

    void SelectableTilePressed(int value)
    {
        activeTileObj.SetActive(true);
        activeTile.Value = value;
        activeTile.Amount = gameManager.GetAmount(value);
        confirmButton.SetActive(true);
    }

    IEnumerator Animate(bool toShow)
    {
        float speed = 2.5f;
        float percent = 0f;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            if (toShow) rectTransform.anchoredPosition = Vector2.up * Mathf.Lerp(-700, 0, percent);
            else rectTransform.anchoredPosition = Vector2.up * Mathf.Lerp(0, -700, percent);

            yield return null;
        }

        isActive = toShow;
    }
}
