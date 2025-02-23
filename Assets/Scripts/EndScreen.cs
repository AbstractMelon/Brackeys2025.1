using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;


public class EndScreen : MonoBehaviour
{
    public string nextScene;
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    private bool fadedIn = false;

    public Sprite demonWinSprite;
    public Sprite playerWinSprite;

    public void ShowEndScreen(bool demonWon)
    {
        fadeImage.sprite = demonWon ? demonWinSprite : playerWinSprite;
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
                color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }

            fadedIn = true;
            yield return new WaitForSeconds(3f);
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
            color.a = Mathf.Clamp01(1 - elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        SceneManager.LoadScene(nextScene);
    }
}