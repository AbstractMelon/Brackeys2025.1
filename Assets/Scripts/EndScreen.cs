using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private Text winText;
    [SerializeField] private Text loseText;
    [SerializeField] private Animator animator;

    private void Start()
    {
        if (Application.isEditor)
        {
            TestEndScreen();
        }
    }

    public void ShowEndScreen(bool demonWon)
    {
        animator.SetBool("DemonWon", demonWon);
        if (demonWon)
        {
            winText.gameObject.SetActive(false);
            loseText.gameObject.SetActive(true);
        }
        else
        {
            winText.gameObject.SetActive(true);
            loseText.gameObject.SetActive(false);
        }
        animator.SetTrigger("Show");
    }

    private void TestEndScreen()
    {
        StartCoroutine(ShowEndScreenTest());
    }

    private IEnumerator ShowEndScreenTest()
    {
        yield return new WaitForSeconds(2);
        ShowEndScreen(true);
        yield return new WaitForSeconds(5);
        ShowEndScreen(false);
    }
}

