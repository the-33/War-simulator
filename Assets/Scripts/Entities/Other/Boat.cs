using StarterAssets;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public GameObject player;
    public bool Cinematic = true;
    private Animator animator;
    public Collider beachCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (Cinematic)
        {
            player.GetComponent<StarterAssetsInputs>().lockPlayer(false, false);
            player.transform.position = transform.position;
            player.GetComponent<PlayerAnimations>().standingSize = 1f;
            player.transform.parent = transform;
            animator.SetBool("Cinematic", true);
            beachCollider.enabled = false;
        }
    }

    public void endAnimation()
    {
        player.GetComponent<StarterAssetsInputs>().unlockPlayer();
        player.transform.parent = null;
        float yRotation = player.transform.eulerAngles.y;
        player.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        player.GetComponent<PlayerAnimations>().standingSize = 2f;
        DontDestroyOnLoad(player);
        Cinematic = false;
        beachCollider.enabled = true;
    }
}
