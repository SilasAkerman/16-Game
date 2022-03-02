using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    public GameObject TitleScreen;
    public GameObject InfoScreen;
    public GameObject SettingsScreen;

    public Toggle UndoToggle;
    public Toggle SelectToggle;

    float outsideScreenOffset = 1500;

    void Start()
    {
        UndoToggle.isOn = PlayerPrefs.GetInt("undo", 1) == 1;
        SelectToggle.isOn = PlayerPrefs.GetInt("select", 1) == 1;

        SetUndoToggleValue(UndoToggle.isOn);
        SetSelectToggleValue(SelectToggle.isOn);
    }

    public void SetUndoToggleValue(bool active)
    {
        PlayerPrefs.SetInt("undo", active ? 1 : 0);
    }

    public void SetSelectToggleValue(bool active)
    {
        PlayerPrefs.SetInt("select", active ? 1 : 0);
    }

    public void PlayButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void InfoButton()
    {
        StartCoroutine(AnimateScreen(TitleScreen, Vector2.zero, Vector2.left * outsideScreenOffset));
        StartCoroutine(AnimateScreen(InfoScreen, Vector2.right * outsideScreenOffset, Vector2.zero));
    }

    public void InfoBackButton()
    {
        StartCoroutine(AnimateScreen(InfoScreen, Vector2.zero, Vector2.right * outsideScreenOffset));
        StartCoroutine(AnimateScreen(TitleScreen, Vector2.left * outsideScreenOffset, Vector2.zero));
    }

    public void SettingsButton()
    {
        StartCoroutine(AnimateScreen(TitleScreen, Vector2.zero, Vector2.left * outsideScreenOffset));
        StartCoroutine(AnimateScreen(SettingsScreen, Vector2.right * outsideScreenOffset, Vector2.zero));
    }

    public void SettingsBackButton()
    {
        StartCoroutine(AnimateScreen(SettingsScreen, Vector2.zero, Vector2.right * outsideScreenOffset));
        StartCoroutine(AnimateScreen(TitleScreen, Vector2.left * outsideScreenOffset, Vector2.zero));
    }

    IEnumerator AnimateScreen(GameObject screen, Vector2 fromPosition, Vector2 toPosition)
    {
        RectTransform screenRT = screen.GetComponent<RectTransform>();
        float percent = 0;
        float speed = 1.3f;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            screenRT.anchoredPosition = Vector2.Lerp(fromPosition, toPosition, percent);
            yield return null;
        }
    }
}
