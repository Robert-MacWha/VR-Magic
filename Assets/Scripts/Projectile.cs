using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Tooltip("How fast the objects accelerates to when spawning in")]
    public float impulse;
    [Tooltip("Particle effect used when the projectile hits somthing")]
    public GameObject hitEffect;

    private float maxLifespan = 10;
    private float lifespan;

    private void Awake()
    {
        transform.GetComponent<Rigidbody>().AddForce(transform.forward * impulse * 100);
    }

    private void Update()
    {
        // Make sure to delete the projectile if it's existing for too long (maxLifespan seconds)
        lifespan += Time.deltaTime;
        if (lifespan > maxLifespan)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Create an explosion, stop the projectile, and set a timer for the projectile's lifeimt
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        lifespan = maxLifespan - (maxLifespan / 50);
        Destroy(transform.GetComponent<Rigidbody>());
        Destroy(transform.GetComponent<Collider>());
    }
}
