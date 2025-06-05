using Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

public class Bullet : MonoBehaviour
{
    public bool waiting = false;
    public bool killYourself = false;

    public Rigidbody rb;
    public Collider _collider;

    private float timeAlive = 0f;
    public float maxTimeAlive = 30f;

    public List<string> decalTags = new();
    public List<GameObject> decals = new();

    public GameObject defaultDecal;

    public void ResetBullet()
    {
        waiting = true;
        timeAlive = 0f;

        transform.position = Vector3.zero;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        GetComponent<Collider>().enabled = false;
    }

    public void ActivateBullet()
    {
        waiting = false;
        rb.isKinematic = false;
        GetComponent<Collider>().enabled = true;
    }

    private void Start()
    {
        waiting = false;
    }

    private void Update()
    {
        if (!waiting) timeAlive += Time.deltaTime;

        if (timeAlive >= maxTimeAlive)
        {
            if (killYourself) Destroy(gameObject);
            else ResetBullet();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable context = null;

        collision.gameObject.TryGetComponent(out context);
        if (context == null && collision.transform.parent != null) collision.transform.parent.TryGetComponent(out context);

        if (context != null) context.TakeDamage(1);

        if (collision.gameObject.name != "Player")
        {
            int decalIndex = decalTags.FindIndex(x => x == collision.gameObject.tag);

            GameObject decal = Instantiate(
                (decalIndex == -1) ? defaultDecal : decals[decalIndex],
                transform.position,
                (decalIndex != -1 && decalTags[decalIndex] == "Water") ? Quaternion.LookRotation(Vector3.down) : Quaternion.LookRotation(transform.forward)
            );

            // Hacer hija la decal del objeto con el que colisionamos
            decal.transform.SetParent(collision.transform);
        }

        print(collision.gameObject.name);

        if (!killYourself)
        {
            ResetBullet();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
