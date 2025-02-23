using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Image image;

    public bool fadeInOnSceneLoad = true;

    public List<GameObject> completedDatesHearts = new();

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

        for (var i = 0; i < completedDatesHearts.Count; i++)
        {
            if(GamerManager.completedMonsters[i]) completedDatesHearts[i].gameObject.SetActive(true);
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
    
    public void setMonster(int monsterID)
    {
        GamerManager.setCurrentMonster(monsterID);
    }

    public void LoadNextScene()
    {
        StartCoroutine(FadeOutThenLoadScene());
        IEnumerator FadeOutThenLoadScene()
        {
            yield return FadeOut();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    Coroutine FadeIn()
    {
        return StartCoroutine(FadeIn());
        IEnumerator FadeIn()
        {
            image.gameObject.SetActive(true);

            for (float t = 1.0f; t > 0.0f; t -= Time.deltaTime * 2.0f)
            {
                var color = image.color;
                color.a = t;
                image.color = color;
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);

            image.gameObject.SetActive(false);
        }
    }

    Coroutine FadeOut()
    {
        return StartCoroutine(FadeOut());
        IEnumerator FadeOut()
        {
            image.gameObject.SetActive(true);

            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * 2.0f)
            {
                var color = image.color;
                color.a = t;
                image.color = color;
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Close()
    {
        Application.Quit();
    }
}
