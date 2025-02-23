using UnityEngine;

public class AlienFöljKropp : MonoBehaviour
{
    public TargetJoint2D joint;

    void Update()
    {
        joint.target = transform.position;
    }
}
