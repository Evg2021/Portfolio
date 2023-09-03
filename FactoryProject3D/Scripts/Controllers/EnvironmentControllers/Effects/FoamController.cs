using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoamController : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        var particles = other.GetComponentsInChildren<ParticleSystem>();
        if (particles.Length > 0)
        {
            particles[0].Stop();
        }
    }
}
