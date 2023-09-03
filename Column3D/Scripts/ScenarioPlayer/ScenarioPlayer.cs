using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScenarioPlayer : SingletonEditor<ScenarioPlayer>
{
    public CameraController Camera;
    public List<ScenarioAsset> Scenarios;
    public string StartScanerioName = "Нормальный режим";

    [Space]

    public int CurrentIndex;
    public ScenarioAsset currentScenario;

    private List<ScenarioStage> currentStages;
    private ScenarioStage previousScenarioLastStage;
    private int trackIndex = 0;
    private bool scenarioIsStarted = false;
    private DialogWindowController mainWindow;

    private string stageName = "Stage";

    private void Start() 
    {
        if (!string.IsNullOrEmpty(StartScanerioName))
        {
            PlayScenario(StartScanerioName);
        }
    }

    public void PlayScenario(string scenarioName)
    {
        var scenario = Scenarios.FindLast(h => h.name == scenarioName);
        if (scenario != null)
        {
            PlayScenario(scenario);
        }
        else
        {
            Debug.LogError("Scenario Loading was Failed. Scenario with name " + scenarioName + " is missing.");
        }
    }
    private void PlayScenario(ScenarioAsset scenario)
    {
        currentScenario = scenario;

        InitializationScenario();
        InitializationMainDialogWindow();
        StartScenario();
    }

    private void InitializationScenario()
    {
        if (currentStages != null)
        {
            EndScenario();

            if (currentStages.Count > 0)
            {
                foreach (var stage in currentStages)
                {
                    if (stage != null && stage.gameObject != null)
                    {
                        stage.transform.SetParent(null);
                        RemoveGradientsNDescriptionWindows(stage);
                        Destroy(stage.gameObject);
                    }
                }
            }
        }

        currentStages = new List<ScenarioStage>();

        if (currentScenario != null)
        {
            currentStages = new List<ScenarioStage>();
            foreach (var stageStruct in currentScenario.Stages)
            {
                var newStage = ScenarioCreator.CreateScenarioStage(transform, stageName);
                Utility.InitializeScenarioStage(newStage, stageStruct);
                currentStages.Add(newStage);
            }

        }
    }
    private void InitializationMainDialogWindow()
    {
        mainWindow = DialogWindowController.Instance;
        if (mainWindow == null)
        {
            Debug.LogError("Main Dialog Window was not Initialized.");
        }
        else
        {
            if(currentStages != null)
            {
                string[] messages = new string[currentStages.Count];
                for (int i = 0; i < messages.Length; i++)
                {
                    messages[i] = currentStages[i].MainWindowMessage;
                }

                mainWindow.Initialize(messages);
                mainWindow.ClearWindow();
            }
        }
    }
    private void StartScenario()
    {
        CurrentIndex = 0;

        if (currentStages.Count > 0 && Camera != null)
        {
            foreach(var stage in currentStages)
            {
                SetActiveDescriptionWindows(stage.DescriptionWindows, false);
                SetActiveGradientObjects(stage.GradientObjects, false);
            }

            trackIndex = 0;
            var staticIndex = trackIndex;
            SetActiveDescriptionWindows(currentStages[staticIndex].DescriptionWindows, true);
            SendMainWindowMessage(staticIndex);
            Camera.PlayTrack(currentStages[staticIndex].KeyObject.Track, delegate
            {
                currentStages[staticIndex].OnStartStage?.Invoke();
            });
            currentStages[staticIndex].BeforeStartStage?.Invoke();
            scenarioIsStarted = true;
        }
        else
        {
            string errorMessage = string.Empty;
            if(Camera == null)
            {
                errorMessage += " CameraController is missing in ScenarioPlayer.";
            }
            if(currentStages.Count == 0)
            {
                errorMessage += " Stages list is empty.";
            }

            Debug.LogError("Start scenario error:" + errorMessage);

            scenarioIsStarted = false;
        }
    }
    private void EndScenario()
    {
        if (currentStages != null)
        {
            if (CameraController.isMoving)
            {
                Camera.ClosePreviuseTrack();
            }

            currentStages[trackIndex].BeforeEndStage?.Invoke();
            currentStages[trackIndex].OnEndStage?.Invoke();
            currentStages[trackIndex].transform.SetParent(null);
            RemoveGradientsNDescriptionWindows(currentStages[trackIndex]);
            Destroy(currentStages[trackIndex].gameObject);
            currentStages.RemoveAt(trackIndex);
        }
    }

    private void SetActiveGradientObjects(List<GradientController> gradients, bool show)
    {
        foreach (var gradient in gradients)
        {
            if (show)
            {
                gradient.Enable();
            }
            else
            {
                gradient.Disable();
            }
        }
    }
    private void PlayGradientObjects(List<GradientController> gradients)
    {
        foreach (var gradient in gradients)
        {
            gradient.PlayGradient();
        }
    }
    private void SetActiveDescriptionWindows(List<DescriptionController> windows, bool show)
    {
        foreach (var window in windows)
        {
            if (show)
            {
                window.Enable();
            }
            else
            {
                window.Disable();
            }
        }
    }
    private void SendMainWindowMessage(int key)
    {
        if (mainWindow != null)
        {
            mainWindow.ShowAllMessagesBefore(key);
        }
    }

    public void SetStage(int nextIndex)
    {
        if (currentStages != null)
        {
            int transitionStageIndex = 0;

            CheckIndex(ref nextIndex);

            bool hasTransition = currentStages[nextIndex].IsTransitionalStage;
            if (hasTransition)
            {
                transitionStageIndex = nextIndex;
                
                if (nextIndex > trackIndex)
                {
                    if (trackIndex == 0 && nextIndex == currentStages.Count - 1)
                    {
                        nextIndex--;
                    }
                    else
                    {
                        nextIndex++;
                    }
                }
                else if (nextIndex < trackIndex)
                {
                    if (nextIndex == 0)
                    {
                        nextIndex++;
                    }
                    else
                    {
                        nextIndex--;
                    }
                }

                CheckIndex(ref nextIndex);
            }

            int previousIndex = trackIndex;
            trackIndex = nextIndex;

            if(nextIndex < previousIndex)
            {
                mainWindow.ClearWindow();
            }

            SendMainWindowMessage(nextIndex);

            if (!hasTransition)
            {
                Camera.PlayTrack(currentStages[nextIndex].KeyObject.Track, delegate
                {
                    ActionAfterCameraMoving(previousIndex, nextIndex);
                });

                ActionBeforeCameraMoving(previousIndex, nextIndex);
            }
            else if (hasTransition)
            {
                Camera.PlayTrack(currentStages[nextIndex].KeyObject.Track, delegate
                {
                    ActionAfterCameraMoving(previousIndex, nextIndex);
                }, currentStages[transitionStageIndex].transform);

                ActionBeforeCameraMoving(previousIndex, nextIndex);
            }

            CurrentIndex = trackIndex;
        }
    }
    public void NextStage()
    {
        if (currentStages != null && currentStages.Count > 1)
        {
            SetStage(trackIndex + 1);
        }
    }
    public void PreviousStage()
    {
        if (currentStages != null && currentStages.Count > 1)
        {
            SetStage(trackIndex - 1);
        }
    }
    public void UnselectAllKeyObjects()
    {
        var selectedObjects = FindObjectsOfType<KeyObject>().Where(h => h.isSelected);
        foreach (var selectedObject in selectedObjects)
        {
            selectedObject.SelectedMaterialOff();
        }
    }
    public void StopAllEffects()
    {
        
    }
    public void ResetScenario()
    {
        if (currentScenario != null)
        {
            PlayScenario(currentScenario);
        }
    }

    private void ActionAfterCameraMoving(int previousIndex, int nextIndex)
    {
        currentStages[previousIndex].OnEndStage?.Invoke();
        currentStages[nextIndex].OnStartStage?.Invoke();
        SetActiveDescriptionWindows(currentStages[previousIndex].DescriptionWindows, false);
        SetActiveGradientObjects(currentStages[previousIndex].GradientObjects, false);
        PlayGradientObjects(currentStages[nextIndex].GradientObjects);

    }
    private void ActionBeforeCameraMoving(int previousIndex, int nextIndex)
    {
        currentStages[previousIndex].BeforeEndStage?.Invoke();
        currentStages[nextIndex].BeforeStartStage?.Invoke();
        SetActiveDescriptionWindows(currentStages[nextIndex].DescriptionWindows, true);
        SetActiveGradientObjects(currentStages[nextIndex].GradientObjects, true);
    }
    private void CheckIndex(ref int index)
    {
        if (currentStages != null)
        {
            if (Mathf.Abs(index) >= currentStages.Count)
            {
                index %= currentStages.Count;
                if (mainWindow != null)
                {
                    mainWindow.ClearWindow();
                }
            }

            if (index < 0)
            {
                index = currentStages.Count + index;
            }
        }
    }
    private void RemoveDescriptionWindows(ScenarioStage stage)
    {
        if (stage != null)
        {
            if (stage.DescriptionWindows != null && stage.DescriptionWindows.Count > 0)
            {
                foreach (var description in stage.DescriptionWindows)
                {
                    Destroy(description.gameObject);
                }
            }
        }
    }
    private void RemoveGradients(ScenarioStage stage)
    {
        if (stage != null)
        {
            if (stage.GradientObjects != null && stage.GradientObjects.Count > 0)
            {
                foreach (var gradient in stage.GradientObjects)
                {
                    Destroy(gradient.gameObject);
                }
            }
        }
    }
    private void RemoveGradientsNDescriptionWindows(ScenarioStage stage)
    {
        if (stage != null)
        {
            RemoveDescriptionWindows(stage);
            RemoveGradients(stage);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextStage();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousStage();
        }
    }

    private void OnGUI()
    {
        //For Debugging Scenario
        GUI.Label(new Rect(Screen.width - 100, 10, 100, 50), "Шаг: " + CurrentIndex.ToString());
        //GUI.Label(new Rect(Screen.width - 100, 10, 100, 50), "FPS: " + 1.0f/Time.deltaTime);
        string scenarioName = (currentScenario != null) ? currentScenario.name : "Выберите сценарий из меню";
        GUI.Label(new Rect(Screen.width * 0.5f - 100, 10, 200, 50), scenarioName);
    }
}
