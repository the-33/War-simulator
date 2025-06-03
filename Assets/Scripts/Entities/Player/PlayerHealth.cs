using Interfaces.IDamageable;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public LoadingCircle _loadingCircle;
    public UIIfak IfakUI;
    public Image vignette;
    private Color vignetteColor;

    private PlayerShooting _shooting;
    private FirstPersonController _firstPersonController;
    private StarterAssetsInputs _input;

    public float m_maxHealth { get; set; } = 4;
    public float m_health { get; set; }

    public bool healing = false;
    public float timeToHeal = 5f;

    public int remainingHeals;
    public int maxHeals = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _shooting = GetComponent<PlayerShooting>();
        _firstPersonController = GetComponent<FirstPersonController>(); 

        healing = false;
        remainingHeals = maxHeals;
        m_health = m_maxHealth;
        UpdateIfakUI(false);
        UpdateUI();
        vignetteColor = vignette.color;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.M)) TakeDamage(1f); // Uncomment for testing

        if (_input.heal && !healing && m_health < m_maxHealth && remainingHeals > 0)
        {
            _shooting.deselectMag();
            UpdateIfakUI(true);
            _loadingCircle.CheckConditionForTime(() => _input.heal, timeToHeal, Heal, () => { healing = false; UpdateIfakUI(false); _shooting.reselectMag(); });
            healing = true;
        }
    }

    public void OnDamaged()
    {
        UpdateUI();
    }

    public void OnDeath()
    {
        
    }

    public void TakeDamage(float damage)
    {
        m_health -= damage;
        OnDamaged();

        if ((m_health / (float)m_maxHealth) <= 0.25f) _firstPersonController.canRun = false;

        if (m_health <= 0)
        {
            OnDeath();
        }
    }

    public void Heal()
    {
        _shooting.reselectMag();
        m_health = m_maxHealth;
        healing = false;
        remainingHeals -= 1;
        _firstPersonController.canRun = true;
        UpdateIfakUI(false);
        UpdateUI();
    }

    public void UpdateUI()
    {
        vignetteColor.a = 1f - (m_health / (float)m_maxHealth);
        vignette.color = vignetteColor;
    }

    private void UpdateIfakUI(bool selected)
    {
        float fillAmount = remainingHeals / (float)maxHeals;
        IfakUI.updateIfak(fillAmount, selected);
    }
}
