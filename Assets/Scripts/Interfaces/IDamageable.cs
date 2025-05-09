namespace Interfaces.IDamageable
{
    public interface IDamageable
    {
        float m_maxHealth { get; set; }
        float m_health { get; set; }
        bool m_isDead => m_health <= 0;

        void TakeDamage(float damage)
        {
            m_health -= damage;
            if (m_health <= 0)
            {
                OnDeath();
            }
        }

        void OnDeath();
    }
}