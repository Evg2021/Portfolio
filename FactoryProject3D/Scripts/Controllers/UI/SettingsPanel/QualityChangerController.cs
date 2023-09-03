using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QualityChangerController : MonoBehaviour
{

  private TextMeshProUGUI QualityDisplay;
  private static string qualityDisplayName = "QualityDisplay";

  private void Awake()
  {
    InitializeQualityDisplay();
    SetQualityToDisplay();
  }

  public void IncreaseQuality()
  {
    QualitySettings.IncreaseLevel();
    SetQualityToDisplay();
  }

  public void DecreaseQuality()
  {
    QualitySettings.DecreaseLevel();
    SetQualityToDisplay();
  }


  private void InitializeQualityDisplay()
    {
    var qualityDisplayTransform = transform.Find(qualityDisplayName);
    if (qualityDisplayTransform)
    {
      if (!qualityDisplayTransform.TryGetComponent(out QualityDisplay))
      {
        Debug.LogError(qualityDisplayName + " has no TextMeshPro component.");
      }
    }
    else
    {
      Debug.LogError(transform.name + " couldn't find child with name: " + qualityDisplayName + '.');
    }
  }

  private void SetQualityToDisplay()
  {
    if (QualityDisplay)
    {
      QualityDisplay.text = QualityString;
    }
  }

  string QualityString
  {
    get
    {
      return QualitySettings.names[QualitySettings.GetQualityLevel()];
    }
  }


}