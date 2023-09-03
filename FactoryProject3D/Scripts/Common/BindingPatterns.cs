using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BindingPatternsHolder", menuName = "Create Binding Pattern Holder")]
public class BindingPatterns : SingletonScriptableObject<BindingPatterns>
{
    public List<BindingPattern> Patterns;

    public bool ExistsPattern(List<PrimitiveObject> primitives, out BindingPattern resultPattern)
    {
        resultPattern = default(BindingPattern);

        if (Patterns != null && Patterns.Count > 0)
        {
            var filteredPatterns = Patterns.Where(h => h.Primitives.Count == primitives.Count);
            if (filteredPatterns.Count() > 0)
            {
                foreach (var filteredPattern in filteredPatterns)
                {
                    var mismatchedPrimitivesCount = primitives.Count;
                    foreach (var primitive in primitives)
                    {
                        //Does't take into account the presence of two and more same primitives
                        if (filteredPattern.Primitives.Contains(primitive))
                        {
                            mismatchedPrimitivesCount--;
                        }
                    }

                    if (mismatchedPrimitivesCount == 0)
                    {
                        resultPattern = filteredPattern;
                        return true;
                    }
                }
            }
        }

        return false;
    }
}

[Serializable]
public struct BindingPattern
{
    public string Name;
    public List<PrimitiveObject> Primitives;
}
