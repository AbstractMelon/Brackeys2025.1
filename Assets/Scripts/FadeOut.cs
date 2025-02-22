using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeOut : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    void Start()
    {
        StartCoroutine(Begin());
    }

    IEnumerator Begin()
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

        Destroy(fadeImage);
    }
}
