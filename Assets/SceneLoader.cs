using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Image image;

    public bool fadeInOnSceneLoad = true;

    private void Start()
    {
        if (fadeInOnSceneLoad)
        {
            FadeIn();
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }

    public void LoadScene(int index)
    {
        StartCoroutine(FadeOutThenLoadScene());
        IEnumerator FadeOutThenLoadScene()
        {
            yield return FadeOut();
            SceneManager.LoadScene(index);
        }
    }

    Coroutine FadeIn()
    {
        return StartCoroutine(FadeIn());
        IEnumerator FadeIn()
        {
            for (float t = 1.0f; t > 0.0f; t -= Time.deltaTime)
            {
                var color = image.color;
                color.a = t;
                image.color = color;
                yield return null;
            }

            image.gameObject.SetActive(false);
        }
    }

    Coroutine FadeOut()
    {
        return StartCoroutine(FadeOut());
        IEnumerator FadeOut()
        {
            image.gameObject.SetActive(true);

            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime)
            {
                var color = image.color;
                color.a = t;
                image.color = color;
                yield return null;
            }
        }
    }

    public void Close()
    {
        Application.Quit();
    }
}
