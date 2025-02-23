using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonGuiEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioSource audioSource;

    public TMP_Text text;
    public Image bgImage;

    public Color bg;
    public Color fg;

    private void OnDisable()
    {
        bgImage.color = bg;
        text.color = fg;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(clickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(hoverSound);
        StartCoroutine(LerpColors(bg, fg));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(LerpColors(fg, bg));
    }

    IEnumerator LerpColors(Color a, Color b)
    {
        for(float t = 0.0f; t < 1.0f; t += Time.deltaTime * 5.0f)
        {
            bgImage.color = Color.Lerp(a, b, t);
            text.color = Color.Lerp(b, a, t);
            yield return null;
        }
    }
}
