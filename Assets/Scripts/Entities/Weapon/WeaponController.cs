using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject m_bulletPrefab;
    public Transform m_firePoint;

    [Header("Sound")]
    [SerializeField] private float m_gunshotSoundRadius = 35f;

    public void Fire()
    {
        var bullet = Instantiate(m_bulletPrefab, m_firePoint.position, m_firePoint.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 500f, ForceMode.VelocityChange);
        bullet.GetComponent<Bullet>().killYourself = true;

        SoundEmitter.Emit(m_firePoint.position, m_gunshotSoundRadius);
    }
}
