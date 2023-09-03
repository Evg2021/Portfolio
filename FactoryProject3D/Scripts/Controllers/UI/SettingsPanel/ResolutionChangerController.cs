using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ResolutionChangerController : MonoBehaviour, IParameter
{
    private TextMeshProUGUI ResolutionDisplay;

    private Resolution[] Resolutions;
    private int currentIndexResolution;

    private static string resolutionDisplayName = "ResolutionDisplay";

    private void Awake()
    {
        InitilaizeResolutions();
        InitializeResolutionDisplay();
    }

    private void OnEnable()
    {
        GetCurrentResolution();
        SetResolution(currentIndexResolution);
    }

    private void InitializeResolutionDisplay()
    {
        var resolutionDisplayTransform = transform.Find(resolutionDisplayName);
        if (resolutionDisplayTransform)
        {
            if (!resolutionDisplayTransform.TryGetComponent(out ResolutionDisplay))
            {
                Debug.LogError(resolutionDisplayName + " has no TextMeshPro component.");
            }
        }
        else
        {
            Debug.LogError(transform.name + " couldn't find child with name: " + resolutionDisplayName + '.');
        }
    }

    private void InitilaizeResolutions()
    {
        var filteredResolutions = new List<Resolution>();

        for(int i = 0; i < Screen.resolutions.Length; i++)
        {
            var currentResolution = Screen.resolutions[i];

            if (!filteredResolutions.Any(h => h.width == currentResolution.width && h.height == currentResolution.height))
            {
                filteredResolutions.Add(currentResolution);
            }
        }

        Resolutions = filteredResolutions.ToArray();
    }

    private int GetCurrentResolution()
    {
        if (Resolutions != null)
        {
            for (int i = 0; i < Resolutions.Length; i++)
            {
                if (Screen.width == Resolutions[i].width && Screen.height == Resolutions[i].height)
                {
                    currentIndexResolution = i;
                    return i;
                }
            }
        }

        return 0;
    }

    private void SetResolution(int resolutionIndex)
    {
        if (Resolutions != null && Resolutions.Length > resolutionIndex)
        {
            currentIndexResolution = resolutionIndex;
            SetResolutionToDisplay(Resolutions[resolutionIndex]);
        }
    }
    private void SetResolutionToDisplay(Resolution resolution)
    {
        if (ResolutionDisplay)
        {
            ResolutionDisplay.text = resolution.width.ToString() + 'x' + resolution.height.ToString();
        }
    }

    public void NextResolution()
    {
        if (Resolutions != null && Resolutions.Length > 0)
        {
            if (Resolutions.Length - 1 > currentIndexResolution)
            {
                SetResolution(currentIndexResolution + 1);
            }
        }
    }
    public void PreviousResolution()
    {
        if (Resolutions != null && Resolutions.Length > 0)
        {
            if (currentIndexResolution > 0)
            {
                SetResolution(currentIndexResolution - 1);
            }
        }
    }

    public void Apply()
    {
        if (Resolutions.Length > currentIndexResolution)
        {
            Settings.SetResolution(Resolutions[currentIndexResolution]);
        }
    }

    public bool IsParameterChanged()
    {
        if (Resolutions.Length > currentIndexResolution)
        {
            return Resolutions[currentIndexResolution].width != Screen.width || Resolutions[currentIndexResolution].height != Screen.height;
        }

        return false;
    }
}