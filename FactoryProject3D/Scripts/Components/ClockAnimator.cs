using UnityEngine;
using System;

public class ClockAnimator : MonoBehaviour
{

  private const float
      hoursToDegrees = 360f / 12f,
      minutesToDegrees = 360f / 60f,
      secondsToDegrees = 360f / 60f;

  public Transform hours, minutes, seconds;

  void Update()
  {

      TimeSpan timespan = DateTime.Now.TimeOfDay;
      hours.localRotation =
          Quaternion.Euler(0f, 0f, (float)timespan.TotalHours * hoursToDegrees);
      minutes.localRotation =
          Quaternion.Euler(0f, 0f, (float)timespan.TotalMinutes * minutesToDegrees);
      seconds.localRotation =
          Quaternion.Euler(0f, 0f, (float)timespan.TotalSeconds * secondsToDegrees);


  }
}
