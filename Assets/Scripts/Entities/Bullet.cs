using UnityEngine;
using UnityEngine.XR;

public class Bullet : MonoBehaviour
{
    public bool waiting = false;
    public bool killYourself = false;

    public Rigidbody rb;
    public Collider _collider;

    public GameObject shotDecal;

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(shotDecal, transform.position, Quaternion.LookRotation(transform.forward));

        if (!killYourself)
        {
            transform.position = Vector3.zero;
            rb.isKinematic = true;
            _collider.enabled = false;
            waiting = true;
        }
        else Destroy(gameObject);
    }
}
