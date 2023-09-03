using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DescriptionController : MonoBehaviour
{
    [SerializeField]
    private Text TextField;

    [SerializeField]
    private Text TitleObject;

    [Space(10)]

    public string Title;

    [Space(10)]
    [Header("Main Text in Window:")]
    [TextArea]
    public string Message;

    //Arguments for message with dynamic params
    //float array format: {{value0, step0, maxValue0}, {value1, step1, maxValue1}, {value2, step2, maxValue2}}
    [HideInInspector]
    public float[,] Arguments;
    [HideInInspector]
    public bool[] IsLoop;
    private float[] startArguments;

    private bool[] isReversing;
    private bool[] isIncreasing;

    private Action ActionOnDestroy;

    private static string defaultTitle = "Описание";
    private static float reverseSpeed = 1.0f;
    private static string increasingSymbol = " <color=#00ff00ff>▲</color>";
    private static string decreasingSymbol = " <color=#ff0000ff>▼</color>";

    private void OnEnable()
    {
        if (Arguments != null && Arguments.Length > 0)
        { 
              
            if (IsLoop != null)
            {
                isReversing = new bool[IsLoop.Length];
                for (int i = 0; i < isReversing.Length; i++)
                {
                    isReversing[i] = false;
                }
            }

            startArguments = new float[Arguments.GetLength(0)];
            isIncreasing = new bool[startArguments.Length];
            for (int i = 0; i < startArguments.Length; i++)
            {
                startArguments[i] = Arguments[i, 0];
            }

            SetText(string.Format(Message, startArguments.Select(h => h.ToString("0.00")).ToArray()));
        }
    }

    private void FixedUpdate()
    {
        if (Arguments != null && startArguments != null)
        {
            for (int i = 0; i < startArguments.Length; i++)
            {
                var newValue = startArguments[i] + Arguments[i, 1];

                isIncreasing[i] = Arguments[i, 1] >= 0;

                if (Arguments[i, 0] < Arguments[i, 2])
                {
                    if (IsLoop != null && IsLoop.Length > i)
                    {
                        if (IsLoop[i] && !isReversing[i] && newValue >= Arguments[i, 2])
                        {
                            StartCoroutine(ResetArguments(i));
                        }
                    }

                    startArguments[i] = Mathf.Clamp(newValue, Arguments[i, 0], Arguments[i, 2]);
                }
                else
                {
                    if (IsLoop != null && IsLoop.Length > i)
                    {
                        if (IsLoop[i] && !isReversing[i] && newValue <= Arguments[i, 2])
                        {
                            StartCoroutine(ResetArguments(i));
                        }
                    }

                    startArguments[i] = Mathf.Clamp(newValue, Arguments[i, 2], Arguments[i, 0]);

                }
            }

            string[] arguments = new string[startArguments.Length];
            for (int i = 0; i < startArguments.Length; i++)
            {
                bool condition = isIncreasing[i];
                if (IsLoop != null)
                {
                    condition = condition == !isReversing[i];
                }
                arguments[i] = startArguments[i].ToString("0.00") + (condition ? increasingSymbol : decreasingSymbol);
            }

            SetText(string.Format(Message, arguments));
        }
    }

    private IEnumerator ResetArguments(int index)
    {
        if (Arguments != null && startArguments != null)
        {
            isReversing[index] = true;

            float time = 0.0f;
            var oldArgument = startArguments[index];
            while (time < 1.0f)
            {
                time += Time.deltaTime * reverseSpeed;
                startArguments[index] = Mathf.Lerp(oldArgument, Arguments[index, 0], time);
                yield return new WaitForFixedUpdate();
            }
            isReversing[index] = false;
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
    public void Destroy()
    {
        CanvasController.RemoveDescriptionWindow(gameObject);
        ActionOnDestroy?.Invoke();
        Destroy(gameObject);
    }

    public void SetTitle(string title)
    {
        if (!string.IsNullOrEmpty(title))
        {
            TitleObject.text = title;
        }
        else
        {
            TitleObject.text = defaultTitle;
        }
    }
    public void Initialize(string message, string title = null, float[,] args = null, bool[] loopParams = null, Action onDestroy = null)
    {
        Message = message;
        SetText(Message);

        if (!string.IsNullOrEmpty(title))
        {
            Title = title;
            SetTitle(title);
        }

        Arguments = args;
        IsLoop = loopParams;
        ActionOnDestroy = onDestroy;
    }
    public void SetText(string text)
    {
        if (TextField != null)
        {
            TextField.text = text;
        }
    }

    private void OnValidate()
    {
        SetText(Message);
        SetTitle(Title);
    }
}
