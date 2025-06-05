using UnityEngine;

public class c4 : MonoBehaviour
{
    public GameObject explosion;

    public void explode()
    {
        // Genera una rotación aleatoria
        Vector3 randomRotation = new Vector3(
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f)
        );

        // Instancia la explosión con esa rotación
        Instantiate(explosion, transform.position, Quaternion.Euler(randomRotation));
        Destroy(gameObject);
    }
}