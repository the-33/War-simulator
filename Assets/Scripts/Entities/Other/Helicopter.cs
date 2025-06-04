using EPOOutline;
using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Helicopter : MonoBehaviour
{
    public float propellersSpeed = 1f;
    private float targetPropellerSpeed;
    public float accelerationSpeed = 2f; // Velocidad de aceleración (ajustable en el inspector)
    public GameObject helicopterDust;

    public Transform propeller1;
    public Transform propeller2;

    public GameObject player;
    public Collider playerCollider;
    public Transform playerPosition;

    private Animator animator;

    public Image fadeImage;           // Imagen negra del Canvas para el fade
    public GameObject uiToActivate;   // UI que se mostrará después del fade
    public float fadeDuration = 1f;   // Duración del fade

    public Missions missions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        targetPropellerSpeed = propellersSpeed;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(helicopterDust);
        helicopterDust.SetActive(false);
        gameObject.SetActive(false);
    }

    void Update()
    {
        // Interpolar velocidad actual hacia la objetivo
        propellersSpeed = Mathf.MoveTowards(propellersSpeed, targetPropellerSpeed, accelerationSpeed * Time.deltaTime);

        float rotationAmount = propellersSpeed * Time.deltaTime;

        if (propeller1 != null)
            propeller1.Rotate(propeller1.transform.up, rotationAmount, Space.Self);

        if (propeller2 != null)
            propeller2.Rotate(propeller2.right, rotationAmount, Space.World);
    }

    public void ChangePropellersSpeed(float speed)
    {
        targetPropellerSpeed = speed;
    }

    public void toggleHelicopterDust()
    {
        helicopterDust.SetActive(!helicopterDust.activeSelf);
    }

    public void ExplodeTower()
    {
        print("explosion");
        player.GetComponent<PlayerInventory>().explodeTower();
    }

    public void MissionFinished()
    {
        missions.MissionComplete();
    }

    public void doAnimation()
    {
        Destroy(gameObject.GetComponent<Outlinable>());
        player.GetComponent<StarterAssetsInputs>().lockPlayer(false);
        player.GetComponent<PlayerHealth>().m_health = 1000f;
        playerCollider.enabled = false;
        player.transform.position = playerPosition.position;
        player.transform.rotation = playerPosition.rotation;
        player.GetComponent<PlayerAnimations>().standingSize = 1f;
        player.GetComponent<PlayerAnimations>().crouchSize = 1f;
        player.transform.parent = playerPosition;
        animator.SetBool("Cinematic", true);
    }

    public void finishCredits()
    {
        StartCoroutine(FadeInAndShowUI());
    }

    private IEnumerator FadeInAndShowUI()
    {
        fadeImage.gameObject.SetActive(true);
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        uiToActivate.SetActive(true);
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(0);
    }
}
