using System.Collections;
using UnityEngine;

public class AnimatedBackground : MonoBehaviour
{
    public float scrollSpeed;

    private void Start()
    {
        StartCoroutine(Scroll());
    }

    IEnumerator Scroll()
    {
        while (true)
        {
            for (float t = 0.0f; t < 2.0f; t += Time.deltaTime * scrollSpeed)
            {
                var position = transform.position;
                position.x = t;
                position.y = -t;
                transform.position = position;

                yield return null;
            }
        }
    }
}
