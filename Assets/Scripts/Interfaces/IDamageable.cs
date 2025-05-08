namespace Interfaces.IDamageable
{
    public interface IDamageable
    {
        float m_maxHealth { get; set; }
        float m_health { get; set; }

        void TakeDamage(float damage)
        {
            m_health -= damage;
            if (m_health <= 0)
            {
                onDeath();
            }
        }

        void onDeath();
    }
}