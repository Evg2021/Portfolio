using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PipeKeyObject : KeyObject
{
    public int[] StreamsNumbers;

    private static string textureName = "_MainTex";
    private static string offsetSpeedShaderName = "_OffsetSpeed";
    private static char splitArgument = '_';
    private static PipesStreamsController streamsController;

    private float defaultSelectedMaterialSpeed;

    private void Start()
    {
        streamsController = PipesStreamsController.Instance;
        InitializeClickable();
    }

    protected override void InitializeMaterials()
    {
        var resources = ResourcesAsset.Instance;
        if (resources != null)
        {
            if (resources.SelectedPipeMaterial == null)
            {
                Debug.LogError("Resources Asset has no " + nameof(resources.SelectedPipeMaterial) + '.');
            }
            else
            {
                SelectedMaterial = Instantiate(resources.SelectedPipeMaterial);
                SelectedMaterial.SetTextureOffset(textureName, Vector2.zero);
                defaultSelectedMaterialSpeed = SelectedMaterial.GetVector(offsetSpeedShaderName).x;
            }

            if (resources.IndicatorPipeMaterial == null)
            {
                Debug.LogError("Resources Asset has no " + nameof(resources.IndicatorPipeMaterial) + '.');
            }
            else
            {
                IndicatorMaterial = Instantiate(resources.IndicatorPipeMaterial);
                IndicatorMaterial.SetTextureOffset(textureName, Vector2.zero);
            }

            isSelected = false;
            isIndicated = false;
        }
        else
        {
            Debug.LogError("Resources Asset is missing in Resources folder.");
        }
    }
    private void InitializeClickable()
    {
        if (streamsController.ContainsAnyStream(StreamsNumbers))
        {
            isClickable = true && canvas2D != null;
        }
    }

    public void SpeedUpMaterialTwice()
    {
        if (SelectedMaterial != null)
        {
            SelectedMaterial.SetVector(offsetSpeedShaderName, new Vector4(defaultSelectedMaterialSpeed * 2.0f, 0.0f, 0.0f, 0.0f));
        }

        if (IndicatorMaterial != null)
        {
            IndicatorMaterial.SetVector(offsetSpeedShaderName, new Vector4(defaultSelectedMaterialSpeed * 2.0f, 0.0f, 0.0f, 0.0f));
        }
    }
    public void SpeedDownMaterialTwice()
    {
        if (SelectedMaterial != null)
        {
            SelectedMaterial.SetVector(offsetSpeedShaderName, new Vector4(defaultSelectedMaterialSpeed * 0.5f, 0.0f, 0.0f, 0.0f));
        }

        if (IndicatorMaterial != null)
        {
            IndicatorMaterial.SetVector(offsetSpeedShaderName, new Vector4(defaultSelectedMaterialSpeed * 0.5f, 0.0f, 0.0f, 0.0f));
        }
    }
    public void SetDefaultMaterialSpeed()
    {
        if (SelectedMaterial != null)
        {
            SelectedMaterial.SetVector(offsetSpeedShaderName, new Vector4(defaultSelectedMaterialSpeed, 0.0f, 0.0f, 0.0f));
        }

        if (IndicatorMaterial != null)
        {
            IndicatorMaterial.SetVector(offsetSpeedShaderName, new Vector4(defaultSelectedMaterialSpeed, 0.0f, 0.0f, 0.0f));
        }
    }

    public void RemoveStreamsData()
    {
        StreamsNumbers = null;
    }
    public void UpdateStreamsData()
    {
        StreamsNumbers = ParseStreams(transform.name);
    }

    private int[] ParseStreams(string streams)
    {
        var splitedStreams = streams.Split(splitArgument);
        if (int.TryParse(splitedStreams[0], out var count))
        {
            int[] result = new int[count];
            if (count < splitedStreams.Length)
            {
                for (int i = 1, j = 0; i <= count; i++, j++)
                {
                    if (int.TryParse(splitedStreams[i], out var number))
                    {
                        result[j] = number;
                    }
                    else
                    {
                        result[j] = 0;
                    }
                }

                return result;
            }
        }

        return null;
    }

    private void SelectedStreamOn()
    {
        if (streamsController != null && StreamsNumbers != null)
        {
            foreach (var number in StreamsNumbers)
            {
                streamsController.SelectedStreamOn(number);
            }
        }
    }
    private void SelectedStreamOff()
    {
        if (streamsController != null && StreamsNumbers != null)
        {
            foreach (var number in StreamsNumbers)
            {
                streamsController.SelectedStreamOff(number);
            }
        }
    }
    private void IndicatorStreamOn()
    {
        if (streamsController != null && StreamsNumbers != null)
        {
            foreach (var number in StreamsNumbers)
            {
                streamsController.IndicatorStreamOn(number);
            }
        }
    }
    private void IndicatorStreamOff()
    {
        if (streamsController != null && StreamsNumbers != null)
        {
            foreach (var number in StreamsNumbers)
            {
                streamsController.IndicatorStreamOff(number);
            }
        }
    }
    private void ShowStreamsDescriptions()
    {
        if (streamsController != null)
        {
            foreach (var number in StreamsNumbers)
            {
                streamsController.ShowStreamDescription(number);
            }
        }
    }

    public override void SetClickable(bool value)
    {
        isClickable = value && StreamsNumbers != null && StreamsNumbers.Length > 0;
    }

    private void OnMouseEnter()
    {
        if (isClickable && MouseController.IsActive && !EventSystem.current.IsPointerOverGameObject() && !isSelected)
        {
            IndicatorStreamOn();
        }
    }
    private void OnMouseExit()
    {
        if (isClickable && (MouseController.IsActive || isIndicated))
        {
            IndicatorStreamOff();
        }
    }
    private void OnMouseDrag()
    {
        if (isMouseDownWithoutMoving && mousePositionOnDown != Input.mousePosition)
        {
            isMouseDownWithoutMoving = false;
            if (isClickable && (MouseController.IsActive || isIndicated))
            {
                IndicatorStreamOff();
            }
        }
    }
    private void OnMouseUpAsButton()
    {
        if (isClickable && isMouseDownWithoutMoving && isIndicated && !EventSystem.current.IsPointerOverGameObject() && !isSelected)
        {
            SelectedStreamOn();
            ShowStreamsDescriptions();
            isMouseDownWithoutMoving = false;
        }
    }
}
