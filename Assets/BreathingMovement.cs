using UnityEngine;

public class BreathingMovement : MonoBehaviour
{
    public float amplitude;
    public float frequency;

    private void Update()
    {
        transform.localPosition = Vector3.up * Mathf.Sin(Time.time * frequency) * amplitude;
    }
}
