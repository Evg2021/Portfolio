using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
    public List<KeyObject> Effects;

    public void StopAllEffects()
    {
        if (Effects != null)
        {
            foreach (var effect in Effects)
            {
                effect.StopEffects();
            }
        }
    }
    
}
