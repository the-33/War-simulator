using UnityEngine;

public class KillInNSeconds : MonoBehaviour
{
    public float secondsToDie;

    void Start()
    {
        Destroy(gameObject, secondsToDie);
    }
}
