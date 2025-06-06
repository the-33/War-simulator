using System.Collections;
using UnityEngine;
using TMPro;

public class Radio : MonoBehaviour
{
    public TextMeshProUGUI consolaTexto; // Asigna esto en el inspector
    public AudioSource audioSource; // Asigna esto también
    public float velocidadEscritura = 0.05f; // Tiempo entre letras
    public float retardoDespuesAudio = 0.2f;
    private float velocidadEscrituraActual;

    private Coroutine escrituraActual;
    private Coroutine animacionTextoActual;
    private bool mostrarCursor = true;

    public string currentMessage;

    void Start()
    {
        StartCoroutine(ParpadearCursor());
    }

    void Update()
    {
    }

    public void TransmitirMensaje(string texto, AudioClip audioOpcional = null, float velocidadEscritura = 0)
    {
        if (escrituraActual != null) StopCoroutine(escrituraActual);

        if (animacionTextoActual != null) StopCoroutine(animacionTextoActual);

        velocidadEscrituraActual = (velocidadEscritura == 0) ? this.velocidadEscritura : velocidadEscritura;

        currentMessage = texto;
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

        yield return animacionTextoActual = StartCoroutine(AnimarTextoConCursor(texto));
    }

    private IEnumerator AnimarTextoConCursor(string texto)
    {
        consolaTexto.text = "";
        string resultado = "";
        int i = 0;

        while (i < texto.Length)
        {
            if (texto[i] == '<') // Detecta inicio de etiqueta
            {
                int cierre = texto.IndexOf('>', i);
                if (cierre != -1)
                {
                    // Añade toda la etiqueta sin animación
                    resultado += texto.Substring(i, cierre - i + 1);
                    i = cierre + 1;
                    consolaTexto.text = resultado + (mostrarCursor ? "<size=70%>▌</size>" : "");
                    continue;
                }
            }

            // Añade carácter normal
            resultado += texto[i];
            consolaTexto.text = resultado + (mostrarCursor ? "<size=70%>▌</size>" : "");
            i++;
            yield return new WaitForSeconds(velocidadEscrituraActual);
        }

        consolaTexto.text = resultado;
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
