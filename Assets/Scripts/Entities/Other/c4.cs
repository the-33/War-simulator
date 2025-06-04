using UnityEngine;

public class c4 : MonoBehaviour
{
    public GameObject explosion;

    public void explode()
    {
        Instantiate(explosion, transform.position, Quaternion.Euler(Vector3.up));
        Destroy(gameObject);
    }
}
