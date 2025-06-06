using Interfaces;
using System.Collections;
using UnityEngine;

public class LimitController : MonoBehaviour
{
    public Radio radio;
    public GameObject player;
    public GameObject explosionPrefab;
    public Missions missions;

    private float timer;
    public float maxStayTime;

    private bool isPlayer = false;

    private string lastRadioMessage;

    void Update()
    {
        if (isPlayer)
        {
            timer += Time.deltaTime;

            if (timer >= maxStayTime)
            {
                StartCoroutine(blowPlayer());
                isPlayer = false; // Para evitar múltiples ejecuciones
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !isPlayer)
        {
            isPlayer = true;
            lastRadioMessage = radio.currentMessage;
            radio.TransmitirMensaje(missions.MinefieldText, missions.MinefieldAudio, 0.03f);
            timer = 0f; // Reinicia el tiempo cuando entra
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && isPlayer)
        {
            isPlayer = false;
            radio.TransmitirMensaje(lastRadioMessage);
            timer = 0f; // Reinicia el contador al salir
        }
    }

    private IEnumerator blowPlayer()
    {
        Debug.Log("¡Jugador explotado!");
        var explosion = Instantiate(explosionPrefab, player.transform);
        explosion.transform.position = player.transform.position + player.transform.forward*2f;
        yield return new WaitForSeconds(0.2f);
        player.GetComponent<IDamageable>()?.TakeDamage(100);
    }
}
