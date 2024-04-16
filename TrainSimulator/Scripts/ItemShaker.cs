using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShaker : MonoBehaviour
{
    private Rigidbody body;
    private BoxCollider collider;
    private bool launch = false;
    public float _force = 1;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if(!launch)
            StartCoroutine(Shake());
        }
    }

    IEnumerator Shake()
    {
        Launch();
        yield return new WaitForSeconds(3);
        launch = false;
        Destroy(body);
        Destroy(collider);
    }
    public void Launch()
    {
        launch = true;
       collider = gameObject.AddComponent<BoxCollider>();
       body = gameObject.AddComponent<Rigidbody>();
       body.AddForce(Vector3.up * _force, ForceMode.Impulse);
    }
}
