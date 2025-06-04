using System.Collections;
using UnityEngine;
using TMPro;

public class Radio : MonoBehaviour
{
    public TextMeshProUGUI consolaTexto; // Asigna esto en el inspector
    public AudioSource audioSource; // Asigna esto también
    public float velocidadEscritura = 0.05f; // Tiempo entre letras
    public float retardoDespuesAudio = 0.2f;

    private Coroutine escrituraActual;
    private bool mostrarCursor = true;

    void Start()
    {
        StartCoroutine(ParpadearCursor());
    }

    void Update()
    {
    }

    public void TransmitirMensaje(string texto, AudioClip audioOpcional = null)
    {
        if (escrituraActual != null)
        {
            StopCoroutine(escrituraActual);
        }

        escrituraActual = StartCoroutine(ProcesarTransmision(texto, audioOpcional));
    }

    private IEnumerator ProcesarTransmision(string texto, AudioClip audioOpcional)
    {
        consolaTexto.text = "";

        if (audioOpcional != null)
        {
            audioSource.clip = audioOpcional;
            audioSource.Play();
            yield return new WaitForSeconds(audioOpcional.length + retardoDespuesAudio);
        }

        yield return StartCoroutine(AnimarTextoConCursor(texto));
    }

    private IEnumerator AnimarTextoConCursor(string texto)
    {
        consolaTexto.text = "";

        for (int i = 0; i <= texto.Length; i++)
        {
            consolaTexto.text = texto.Substring(0, i) + (mostrarCursor ? "<size=70%>▌</size>" : "");
            yield return new WaitForSeconds(velocidadEscritura);
        }

        consolaTexto.text = texto;
    }

    private IEnumerator ParpadearCursor()
    {
        while (true)
        {
            mostrarCursor = !mostrarCursor;
            yield return new WaitForSeconds(0.3f);
        }
    }
}
