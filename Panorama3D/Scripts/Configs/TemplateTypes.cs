using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TemaplteTypes", menuName = "Create Template Types")]
public class TemplateTypes : ScriptableObject
{
    public List<TemplateType> Templates;

    public GameObject GetTemplate(string name)
    {
        if (Templates != null && Templates.Count > 0)
        {
            return Templates.FirstOrDefault(h => h.Name == name).TemplateObject;
        }

        return null;
    }
}