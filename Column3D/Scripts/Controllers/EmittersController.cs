using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmittersController : MonoBehaviour
{
    public List<EmitterController> Emitters;

    public void StopEmitters()
    {
        if (Emitters != null)
        {
            foreach (var emitter in Emitters)
            {
                emitter.StopEmitter();
            }
        }
    }
}
