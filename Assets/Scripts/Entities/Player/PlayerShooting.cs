using NUnit.Framework;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    private StarterAssetsInputs _input;

    public bool shooting;
    public bool reloading;
    public bool aiming;

    public int maxBulletsPerMag = 30;
    public int numberOfMags = 5;
    [SerializeField] private int[] mags;
    public UIMag[] uIMags;

    private int currentMagIndex;

    public GameObject bulletPrefab;
    public Transform shootingPoint;
    public float shootingForce;

    private List<GameObject> bulletPool = new();
    public int maxBulletsInScene = 20;

    private void handleInputs()
    {
        if (_input.fire && mags[currentMagIndex] > 0 && !reloading) shooting = true;
        else shooting = false;

        if (_input.aim && !reloading) aiming = true;
        else aiming = false;

        if (_input.reload && mags[currentMagIndex] < maxBulletsPerMag)
        {
            bool avaliableMag = false;

            for (int i = 0; i < numberOfMags; i++)
            {
                if (mags[i] > 0 && i != currentMagIndex) avaliableMag = true;
            }

            if (avaliableMag) reloading = true;
        }
        else
        {
            if (_input.reload) _input.reload = false;
            reloading = false;
        }
    }

    public void endReload()
    {
        int nextMagIndex = 0;

        for(int i = 1; i<numberOfMags; i++)
        {
            if (mags[i] > mags[nextMagIndex] && i != currentMagIndex) nextMagIndex = i;
        }

        uIMags[currentMagIndex].updateMag(mags[currentMagIndex]/30f, false);
        currentMagIndex = nextMagIndex;
        uIMags[currentMagIndex].updateMag(mags[currentMagIndex]/30f, true);

        reloading = false;

        _input.reload = false;
    }

    public void replenishAmmo()
    {
        for (int i = 1; i < numberOfMags; i++)
        {
            mags[i] = maxBulletsPerMag;
            uIMags[i].updateMag(1, i == currentMagIndex);
        }
    }

    public void shoot()
    {
        GameObject bullet = null;
        if (bulletPool.Count<maxBulletsInScene)
        {
            bullet = Instantiate(bulletPrefab, shootingPoint.position + shootingPoint.forward * 0.5f, shootingPoint.rotation);
            bulletPool.Add(bullet);
        }
        else
        {
            bullet = bulletPool.Find(x => x.GetComponent<Bullet>().waiting);
            if (bullet != null)
            {
                bullet.GetComponent<Bullet>().ActivateBullet();
                bullet.transform.position = shootingPoint.position + shootingPoint.forward * 0.5f;
                bullet.transform.rotation = shootingPoint.rotation;
            }
            else
            {
                bullet = Instantiate(bulletPrefab, shootingPoint.position + shootingPoint.forward * 0.5f, shootingPoint.rotation);
                bullet.GetComponent<Bullet>().killYourself = true;
            }
        }
        
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * shootingForce, ForceMode.VelocityChange);

        mags[currentMagIndex]--;
        uIMags[currentMagIndex].updateMag(mags[currentMagIndex] / 30f, true);
    }

    private void Update()
    {
        handleInputs();
    }

    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();

        mags = new int[numberOfMags];
        uIMags = new UIMag[numberOfMags];

        for(int i = 0; i<numberOfMags; i++)
        {
            uIMags[i] = GameObject.Find("Magazine" + i).GetComponent<UIMag>();
        }

        for (int i = 0; i<numberOfMags; i++) mags[i] = maxBulletsPerMag;
        currentMagIndex = 0;
        uIMags[currentMagIndex].updateMag(mags[currentMagIndex] / 30f, true);
    }
}
