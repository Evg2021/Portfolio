using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightController : MonoBehaviour
{
    private const string emissionShaderKeyname = "_EMISSION";

    public Material material;
    public Transform timeSlider;
    public GameObject lampsSpotsNight;

    private Slider slider;

    [Range(0f, 1f)]
    private float sliderMaxValue = 0.75f;
    [Range(0f, 1f)]
    private float sliderMinValue = 0.25f;

    private void Start()
    {
        if (!timeSlider)
        {
            Debug.LogError("Time slider was losted");
        }
        else
        {
            slider = timeSlider.GetComponent<Slider>();
            if (!slider)
            {
                Debug.LogError("Time slider was losted");
            }
            else
            {
                slider.onValueChanged.AddListener(delegate (float value) { OnOffLamps(value);});
            }
        }
    }

    private void OnOffLamps(float value)
    {
        if (!material)
        {
            Debug.LogError("Material was losted");
        }

        if (!lampsSpotsNight)
        {
            Debug.LogError("Lamps spots night was losted");
        }

        if (material && lampsSpotsNight)
        {
            if (value < sliderMinValue || value > sliderMaxValue)
            {
                material.EnableKeyword(emissionShaderKeyname);
                lampsSpotsNight.SetActive(true);
            }
            else
            {
                material.DisableKeyword(emissionShaderKeyname);
                lampsSpotsNight.SetActive(false);
            }
        }
    }
}
