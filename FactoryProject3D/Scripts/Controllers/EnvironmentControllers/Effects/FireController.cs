using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }

    private void OnParticleTrigger()
    {
        Debug.Log("Yes, Particle");
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log($"Da blya: {other.name}");
    }
}
