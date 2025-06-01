using Interfaces.IDamageable;
using StarterAssets;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    private StarterAssetsInputs _input;
    public LoadingCircle _loadingCircle;

    public float m_maxHealth { get; set; } = 4;
    public float m_health { get; set; } = 2;

    public bool healing = false;
    public float timeToHeal = 5f;

    public int remainingHeals = 5;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        healing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_input.heal && !healing && m_health < m_maxHealth && remainingHeals < 0)
        {
            _loadingCircle.CheckConditionForTime(() => _input.heal, timeToHeal, Heal, () => healing = false);
            healing = true;
        }
    }

    public void OnDamaged()
    {
        
    }

    public void OnDeath()
    {
        
    }

    public void Heal()
    {
        m_health = m_maxHealth;
        healing = false;
        remainingHeals -= 1;
    }

    private void UpdateUI()
    {

    }
}
