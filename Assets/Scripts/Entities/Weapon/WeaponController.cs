using UnityEngine;

public class WeaponController : MonoBehaviour
{

    public GameObject m_bulletPrefab;
    public Transform m_firePoint;

    public void Fire()
    {
        Debug.Log("Weapon fired!");
        var bullet = Instantiate(m_bulletPrefab, m_firePoint.position, m_firePoint.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 500f, ForceMode.VelocityChange);
        bullet.GetComponent<Bullet>().killYourself = true;
    }
}
