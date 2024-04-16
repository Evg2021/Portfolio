using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum EVagonNumber
{
    one = 1, 
    two = 2, 
    three = 3, 
    four = 4
}

public class TrainVagonMover : MonoBehaviour
{
    [Space(10)]
    [SerializeField] private Transform _defectCart;
    [SerializeField] private float _defectY;
    [Space(10)]
    [SerializeField] private List<Transform> _vagons;

    public void UpdatePosition(EVagonNumber vagonNumber, float distance, float speed, Ease easeType = Ease.Linear)
    {
        int vagonIndex = (int)vagonNumber - 1;
        Transform currentVagon = _vagons[vagonIndex];
        foreach (var i in currentVagon.GetComponentsInChildren<Transform>())
        {
            i.gameObject.isStatic = false;
        }
        float time = Mathf.Abs(distance) / speed;
        StartCoroutine(Wait(currentVagon, distance, time, easeType));
    }

    private IEnumerator Wait(Transform currentVagon, float distance, float duration, Ease easeType)
    {
        yield return null;
                
        Vector3 newPos = new Vector3(currentVagon.position.x, currentVagon.position.y,
            currentVagon.position.z - distance);


        currentVagon.DOMove(newPos, duration).SetEase(easeType).SetLink(currentVagon.gameObject);
    }

    [ContextMenu("Displace cart")]
    public void DisplaceCart()
    {        
        _defectCart.localRotation = Quaternion.Euler(0, _defectY, 0);
    }
}
