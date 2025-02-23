using System;
using System.Collections;
using UnityEngine;

public class MartinAnimator : MonoBehaviour
{
    Vector2 origin;
    public Vector2 underTable;

    public void DuckUnderTable(Action whenUnderTable)
    {
        StartCoroutine(Animate());
        IEnumerator Animate()
        {
            for(float t = 0.0f; t < 1.0f; t += Time.deltaTime * 2)
            {
                transform.position = Vector2.Lerp(origin, underTable, t);
                yield return null;
            }
            yield return new WaitForSeconds(0.4f);
            whenUnderTable();
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * 2)
            {
                transform.position = Vector2.Lerp(origin, underTable, 1.0f - t);
                yield return null;
            }
        }
    }
}
