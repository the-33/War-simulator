using Interfaces.IDamageable;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float m_maxHealth { get; set; }
    public float m_health { get; set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDamaged()
    {
        
    }

    public void OnDeath()
    {
        
    }
}
