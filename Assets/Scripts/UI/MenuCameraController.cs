using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
  

    public Transform target;   // centro del mapa
    public float speed = 10f;  // velocidad de rotación

    void Update()
    {
        transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime);
        transform.LookAt(target);
    }
}

