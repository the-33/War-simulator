using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        Vector3 lookPos = cam.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
        transform.Rotate(0, 180f, 0);
    }
}
