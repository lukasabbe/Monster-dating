using UnityEngine;

public class Wiggle : MonoBehaviour
{
    public float wiggleAmount;
    public float wiggleSpeed;

    private void Update()
    {
        var angles = transform.eulerAngles;
        angles.z = Mathf.Sin(wiggleSpeed * Time.time) * wiggleAmount;
        transform.eulerAngles = angles;
        var size = transform.localScale;
        size = Vector3.one * Mathf.Lerp(0.9f, 1.1f, Mathf.Cos(wiggleSpeed * Time.time) * 0.5f + 0.5f);
        transform.localScale = size;
    }
}
