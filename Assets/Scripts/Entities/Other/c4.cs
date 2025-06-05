using UnityEngine;

public class c4 : MonoBehaviour
{
    public GameObject explosion;

    public void explode()
    {
        // Genera una rotaci�n aleatoria
        Vector3 randomRotation = new Vector3(
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f)
        );

        // Instancia la explosi�n con esa rotaci�n
        Instantiate(explosion, transform.position, Quaternion.Euler(randomRotation));
        Destroy(gameObject);
    }
}