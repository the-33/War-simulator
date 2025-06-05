using UnityEngine;

public class WeaponEvents : MonoBehaviour
{
    public WeaponController weaponHandler;

    public void Fire()
    {
        weaponHandler.Fire();
    }
}