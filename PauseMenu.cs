using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public event System.Action OnFinished;

    GameManager gameManager;

    RectTransform rectTransform;
    bool isActive = false;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        rectTransform.anchoredPosition = Vector2.down * 700;
    }

    public void ChangeActivation(bool activate)
    {
        if (activate)
        {
            gameManager.ChangeGameState(GameManager.GameState.Menu);
            StartCoroutine(Animate(true));
        }
        else
        {
            isActive = false;
            OnFinished?.Invoke();
            StartCoroutine(Animate(false));
            gameManager.ChangeGameState(GameManager.GameState.Gameplay);
        }
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ResumeButton()
    {
        ChangeActivation(false);
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
