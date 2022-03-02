using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinishScreen : MonoBehaviour
{
    GameManager gameManager;
    RectTransform rectTransform;

    public TileHolderStack[] stacks;
    public Tile[] shownTiles;


    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        rectTransform = GetComponent<RectTransform>();
    }

    void SetStackValues()
    {
        int[] values = new int[shownTiles.Length];
        for (int i = 0; i < stacks.Length; i++)
        {
            values[i] = stacks[i].GetTopTile() == null ? 0 : stacks[i].GetTopTile().tileValue;
        }
        System.Array.Sort(values);
        for (int i = 0; i < values.Length; i++)
        {
            shownTiles[i].tileValue = values[i];
        }
    }

    public void ChangeActivation()
    {
        gameManager.ChangeGameState(GameManager.GameState.Menu);
        SetStackValues();
        StartCoroutine(Animate(true));
        
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("Menu");
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    IEnumerator Animate(bool toShow)
    {
        float speed = 1f;
        float percent = 0f;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            if (toShow) rectTransform.anchoredPosition = Vector2.up * Mathf.Lerp(-700, 0, percent);
            else rectTransform.anchoredPosition = Vector2.up * Mathf.Lerp(0, -700, percent);

            yield return null;
        }

    }
}
