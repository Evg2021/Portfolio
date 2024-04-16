using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ChunkObjectHelper : MonoBehaviour
{
    public string Moose;
    public string RailsRight;
    public string Car;
    public Transform _wireSearchPoint;
    public Transform _wire;
    private CutWire _nearPost;
    private CutWire _secondaryNearPost;

    public void MooseDelete()
    {
        GameObject moose = GameObject.Find(Moose);
        Destroy(moose);
    }

    public void CarDelete()
    {
        GameObject car = GameObject.Find(Car);
        Destroy(car);
    }

    public void NormalRailsEnable()
    {
        GameObject r = GameObject.Find(RailsRight);
        GameObject railBroken = r.transform.GetChild(3).gameObject;
        GameObject railNorm = r.transform.GetChild(2).gameObject;
        railBroken.SetActive(false);
        railNorm.SetActive(true);
    }

    public void WiresCut()
    {
        var posts = FindObjectsOfType<CutWire>();
        _nearPost = GetNearPostAtPoint(posts, _wireSearchPoint.position);
        _secondaryNearPost = GetNearPostAtPoint(posts, _nearPost.transform.position);
        GameObject cutWirePoint = _nearPost.GetCutWirePoint();
        _wire.gameObject.SetActive(true);
        _wire.position = cutWirePoint.transform.position;
        _secondaryNearPost.Cut();
        _nearPost.Cut();
        posts = null;
    }

    public void WireRestore()
    {
        if (_nearPost != null)
        {
            _nearPost.Restore();
            _secondaryNearPost.Restore();
            _wire.gameObject.SetActive(false);
        }
    }

    public CutWire GetNearPostAtPoint(CutWire[] cutWires, Vector3 pointWorldSpace)
    {
        CutWire wireAtPoint = null;
        float closestDistance = Mathf.Infinity;

        foreach (CutWire cutWire in cutWires)
        {
            if (cutWire.transform.position.z <= pointWorldSpace.z)
                continue;

            float distance = Vector3.Distance(pointWorldSpace, cutWire.transform.position);
            if (distance < closestDistance)
            {
                wireAtPoint = cutWire;
                closestDistance = distance;
            }
        }

        return wireAtPoint;
    }
}