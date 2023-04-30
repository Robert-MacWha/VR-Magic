using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter: MonoBehaviour
{
    public float lifespan;

    private float life;

    private void Update()
    {
        life += Time.deltaTime;
        if (life > lifespan)
        {
            Destroy(this.gameObject);
        }
    }
}
