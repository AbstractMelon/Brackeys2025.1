using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Letter : MonoBehaviour
{
    public string nextScene;
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    private bool isFading = false;

    void Update()
    {
        if (!isFading && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        isFading = true;
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
