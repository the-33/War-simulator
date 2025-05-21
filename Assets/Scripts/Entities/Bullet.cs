using UnityEngine;
using UnityEngine.XR;

public class Bullet : MonoBehaviour
{
    public bool waiting = false;
    public bool killYourself = false;

    public Rigidbody rb;
    public Collider collider;

    private void OnCollisionEnter(Collision collision)
    {
        if (!killYourself)
        {
            transform.position = Vector3.zero;
            rb.isKinematic = true;
            collider.enabled = false;
            waiting = true;
        }
        else Destroy(gameObject);
    }
}
