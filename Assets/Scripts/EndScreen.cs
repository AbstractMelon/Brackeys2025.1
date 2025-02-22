using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    public string nextScene;
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    private bool fadedIn = false;

    public Sprite PLSPRITE;

    public void ShowEndScreen(bool demonWon)
    {
        if(!demonWon)
        {
            fadeImage.sprite = PLSPRITE;
        }
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        if (!fadedIn)
        {
            float elapsedTime = 0f;
            Color color = fadeImage.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                color.r = Mathf.Clamp01(elapsedTime / fadeDuration);
                color.g = Mathf.Clamp01(1 - elapsedTime / fadeDuration);
                color.b = Mathf.Clamp01(1 - elapsedTime / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }

            fadedIn = true;
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.r = Mathf.Clamp01(1 - elapsedTime / fadeDuration);
            color.g = Mathf.Clamp01(1 - elapsedTime / fadeDuration);
            color.b = Mathf.Clamp01(1 - elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        SceneManager.LoadScene(nextScene);
    }
}