using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrenObject
{
    public Vector3 GetPosition();
    public Quaternion GetRotation();
    public string GetTrenName();
    public string GetTemplateName();
    public string GetPanoramName();
    public TrenObjectInfo GetTrenObjectInfo();

    public void SetTrenObjectName(string name);
    public void SetPultObjectName(string name);
    public void SetPanoramaName(string name);
}
