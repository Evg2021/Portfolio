using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInWorld : MonoBehaviour
{
    public GameObject PultMat;
    public Vector3 StartPosition;
    public Vector3 EndPosition;
    public float OffsetPosition;
    public float SpeedOfPushing;
    public float SpeedOfBackPushing;
    
    // Start is called before the first frame update
    void Start()
    {
        StartPosition = transform.position;
        EndPosition = StartPosition + new Vector3(0.0f, 0.0f, OffsetPosition);
        //StartCoroutine(OnClickPultMatButton(StartPosition, EndPosition));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator OnClickPultMatButton(Vector3 start, Vector3 end)
    {
        float time = 0.0f;
        Vector3 currentStartPosition = transform.position;
        while(time < 1)
        {
            time += Time.deltaTime * SpeedOfPushing;
            transform.position = Vector3.Lerp(currentStartPosition, end, time);
            yield return new WaitForEndOfFrame();
        }
        time = 0.0f;
        while(time < 1)
        {
            time += Time.deltaTime * SpeedOfBackPushing;
            transform.position = Vector3.Lerp(end, start, time);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnMouseDown()
    {
                
            StartCoroutine(OnClickPultMatButton(StartPosition, EndPosition));
        
    }
}
