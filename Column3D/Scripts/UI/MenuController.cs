using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Transform Content;
    public ButtonController MenuButton;

    [Space]

    public List<GameObject> Buttons;

    private List<ScenarioAsset> scenarios;
    private bool isOpenedMenu;

    private void Awake()
    {
        if (Buttons != null)
        {
            if (MenuButton != null)
            {
                foreach (var button in Buttons)
                {
                    if (button.TryGetComponent<Button>(out var buttonComponent))
                    {
                        buttonComponent.onClick.AddListener(HideMenu);
                    }
                }
            }
        }
    }

    public void SwitchActiveMenu()
    {
        if (isOpenedMenu)
        {
            HideMenu();
        }
        else
        {
            ShowMenu();
        }
    }

    public void HideMenu()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            
            if (MenuButton != null)
            {
                MenuButton.ChangeTextureOnClick();
            }
        }

        isOpenedMenu = false;
    }
    public void ShowMenu()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);

            if (MenuButton != null)
            {
                MenuButton.ChangeTextureOnClick();
            }
        }

        isOpenedMenu = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (!EventSystem.current.IsPointerOverGameObject() && isOpenedMenu)
            {
                HideMenu();
            }
        }
    }

    public void RefreshScenariosList()
    {
        if (Content == null)
        {
            Debug.LogError("Please, put Content transform in \"Content\" field.");
            return;
        }

        InitializeScenarios();

        if (Buttons != null)
        {
            foreach (var button in Buttons)
            {
                DestroyImmediate(button);
            }
        }

        Buttons = new List<GameObject>();

        if (scenarios != null)
        {
            var scenarioButtonPrefab = Utility.GetScenarioButtonPrefab();
            if (scenarioButtonPrefab != null)
            {
                for(int i = 0; i < scenarios.Count; i++)
                {
                    var scenarioButtonObject = Instantiate(scenarioButtonPrefab, Content);
                    scenarioButtonObject.name = scenarios[i].name;
                    scenarioButtonObject.transform.SetSiblingIndex(i);

                    var textButton = scenarioButtonObject.GetComponentInChildren<Text>();
                    textButton.text = scenarios[i].name;

                    Buttons.Add(scenarioButtonObject);
                }
            }
        }
    }
    private void InitializeScenarios()
    {
        if (ScenarioPlayer.Instance != null)
        {
            scenarios = ScenarioPlayer.Instance.Scenarios;
        }
        else
        {
            Debug.LogError("ScenarioPlayer object is missing on scene.");
        }
    }
    
}
