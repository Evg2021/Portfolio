using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionFlickering : MonoBehaviour
{
    [SerializeField] private Material _materialTarget;
    [SerializeField] private float emissionDelay = 3f;

    private Renderer rend;
    private Material mat;
    private float timer = 0f;
    private bool emissionEnabled = true;
    private bool _isFLickering = false;

    public void LaunchFlickering()
    {
        _isFLickering = true;
    }

    public void DisableFlickering()
    {
        DisableEmission();
        _isFLickering = false;
    }
    
    private void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material = new Material(_materialTarget);
        mat = rend.material;
    }
    
    private void Update()
    {
        if(!_isFLickering)
            return;
        
        timer += Time.deltaTime;

        if (timer >= emissionDelay)
        {
            if (emissionEnabled)
            {
                DisableEmission();
            }
            else
            {
                EnableEmission();
            }

            emissionEnabled = !emissionEnabled;
            timer = 0f;
        }
    }

    private void EnableEmission()
    {
        mat.EnableKeyword("_EMISSION");
    }

    private void DisableEmission()
    {
        mat.DisableKeyword("_EMISSION");
    }
}
