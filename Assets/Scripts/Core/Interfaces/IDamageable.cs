namespace Interfaces.IDamageable
{
    public interface IDamageable
    {
        float m_maxHealth { get; set; }
        float m_health { get; set; }
        bool m_isDead => m_health <= 0;

        /// <summary>
        /// Applies damage to the entity.
        /// </summary>
        /// <param name="damage"></param>
        void TakeDamage(float damage)
        {
            m_health -= damage;
            OnDamaged();
            if (m_health <= 0)
            {
                OnDeath();
            }
        }

        /// <summary>
        /// Called each time the entity is damaged.
        /// </summary>
        void OnDamaged();

        /// <summary>
        /// Called when the entity's health reaches zero.
        /// </summary>
        void OnDeath();
    }
}