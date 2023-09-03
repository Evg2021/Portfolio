using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientController : MonoBehaviour
{
    public Material GradientMaterial;
    public bool IsIncreasing;

    private Material instanceMaterial;
    private Coroutine currentCoroutine;

    private static string scaleGradientShaderName = "_Scale";
    private static float maxScaleGradient = 2.0f;
    private static float gradientSpeed = 0.4f;

    public void Initialize(bool isIncreasing)
    {
        IsIncreasing = isIncreasing;
    }
    
    public void PlayGradient()
    {
        if(instanceMaterial == null)
        {
            Debug.LogError("Material was not Initialized.");
            return;
        }    

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        if (IsIncreasing)
        {
            currentCoroutine = StartCoroutine(IncreaseGradient());
        }
        else
        {
            currentCoroutine = StartCoroutine(DecreaseGradient());
        }
    }
    public IEnumerator IncreaseGradient()
    {
        while (instanceMaterial.GetFloat(scaleGradientShaderName) < maxScaleGradient)
        {
            instanceMaterial.SetFloat(scaleGradientShaderName, instanceMaterial.GetFloat(scaleGradientShaderName) + Time.deltaTime * gradientSpeed);
            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator DecreaseGradient()
    {
        while (instanceMaterial.GetFloat(scaleGradientShaderName) > 0.0f)
        {
            instanceMaterial.SetFloat(scaleGradientShaderName, instanceMaterial.GetFloat(scaleGradientShaderName) - Time.deltaTime * gradientSpeed);
            yield return new WaitForEndOfFrame();
        }
    }
    public void Enable()
    {
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (instanceMaterial == null)
        {
            if (GradientMaterial != null)
            {
                if (TryGetComponent<Image>(out var image))
                {
                    instanceMaterial = Instantiate(GradientMaterial);
                    image.material = instanceMaterial;
                }
                else
                {
                    Debug.LogError("GradientObject has no Image component.");
                }
            }
            else
            {
                Debug.LogError("GradientMaterial is missing.");
            }
        }
        
        if (instanceMaterial != null)
        {
            if (IsIncreasing)
            {
                instanceMaterial.SetFloat(scaleGradientShaderName, 0.0f);
            }
            else
            {
                instanceMaterial.SetFloat(scaleGradientShaderName, maxScaleGradient);
            }
        }
    }
    private void OnDestroy()
    {
        if (instanceMaterial != null)
        {
            Destroy(instanceMaterial);
            instanceMaterial = null;
        }
    }
}
