using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public Sprite SelectedImage;

    private Sprite defaultImage;
    private Image imageComponent;
    private bool textureIsChanged;

    private void Awake()
    {
        if (TryGetComponent<Image>(out imageComponent))
        {
            defaultImage = imageComponent.sprite;
            textureIsChanged = false;
        }
    }

    public void OnClickLoadScenario()
    {
        var player = ScenarioPlayer.Instance;
        if (player != null)
        {
            player.PlayScenario(transform.name);
        }
        else
        {
            Debug.LogError("Scenario Loading was Failed. ScenarioPlayer Object is missing on Scene.");
        }
    }
    public void OnClickExitButton()
    {
        Application.Quit();
    }

    public void ChangeTextureOnClick()
    {
        if (defaultImage != null && imageComponent != null && SelectedImage != null)
        {
            if (!textureIsChanged)
            {
                imageComponent.sprite = SelectedImage;
            }
            else
            {
                imageComponent.sprite = defaultImage;
            }

            textureIsChanged = !textureIsChanged;
        }
    }
}
