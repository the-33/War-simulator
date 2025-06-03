using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerDie : MonoBehaviour
{
    [Header("Objetos a animar")]
    public Transform capsule;
    public Transform playerCameraRoot;
    public Transform hand;

    [Header("Animación")]
    public float dieDuration = 0.5f;

    [Header("UI")]
    public Image deathFadeImage;

    private float timer = 0f;
    private bool isDying = false;
    private Action onDeathComplete;

    private Vector3 capsuleStartPos, capsuleEndPos;
    private Quaternion capsuleStartRot, capsuleEndRot;

    private Vector3 camStartPos, camEndPos;
    private Quaternion camStartRot, camEndRot;

    private Vector3 handStartPos, handEndPos;
    private Quaternion handStartRot, handEndRot;

    public Quaternion relativeCapsuleRot = Quaternion.Euler(0f, 0f, 63.235f);
    public Quaternion relativeCamRot = Quaternion.Euler(0f, 0f, 57.2f);
    public Quaternion relativeHandRot = Quaternion.Euler(343.964783f, 348.425842f, 36.5539589f);

    public Vector3 capsuleOffset = new Vector3(-0.96f, 0.03f, 0f);
    public Vector3 camOffset = new Vector3(-1.34f, 0.05f, 0f);
    public Vector3 handOffset = new Vector3(-0.230000004f, -0.219999999f, 0.442993194f);

    void Update()
    {
        if (!isDying) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / dieDuration);

        // Interpolación de movimiento y rotación
        capsule.localPosition = Vector3.Lerp(capsuleStartPos, capsuleEndPos, t);
        capsule.localRotation = Quaternion.Lerp(capsuleStartRot, capsuleEndRot, t);

        playerCameraRoot.localPosition = Vector3.Lerp(camStartPos, camEndPos, t);
        playerCameraRoot.localRotation = Quaternion.Lerp(camStartRot, camEndRot, t);

        hand.localPosition = Vector3.Lerp(handStartPos, handEndPos, t);
        hand.localRotation = Quaternion.Lerp(handStartRot, handEndRot, t);

        // Interpolación de opacidad (fade in)
        if (deathFadeImage != null)
        {
            Color c = deathFadeImage.color;
            c.a = t;
            deathFadeImage.color = c;
        }

        // Fin de animación
        if (t >= 1f)
        {
            isDying = false;
            onDeathComplete?.Invoke();
            onDeathComplete = null;
        }
    }

    public void Die(Action onFinish = null)
    {
        if (isDying) return;

        timer = 0f;
        isDying = true;
        onDeathComplete = onFinish;

        capsuleStartPos = capsule.localPosition;
        capsuleStartRot = capsule.localRotation;

        camStartPos = playerCameraRoot.localPosition;
        camStartRot = playerCameraRoot.localRotation;

        handStartPos = hand.localPosition;
        handStartRot = hand.localRotation;

        capsuleEndRot = capsuleStartRot * relativeCapsuleRot;
        camEndRot = camStartRot * relativeCamRot;
        handEndRot = handStartRot * relativeHandRot;

        capsuleEndPos = capsuleStartPos + transform.TransformDirection(capsuleOffset);
        camEndPos = camStartPos + transform.TransformDirection(camOffset);
        handEndPos = handStartPos + transform.TransformDirection(handOffset);

        // Asegura que el alpha de inicio sea 0
        if (deathFadeImage != null)
        {
            Color c = deathFadeImage.color;
            c.a = 0f;
            deathFadeImage.color = c;
        }
    }
}
