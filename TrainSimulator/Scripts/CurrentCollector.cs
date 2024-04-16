using UnityEngine;

public class CurrentCollector : MonoBehaviour
{
    [SerializeField] private GameObject _normalCollector;
    [SerializeField] private GameObject _brokenCollector;

    [ContextMenu("Broken")]
    public void BrokenActive()
    {
        _normalCollector.SetActive(false);
        _brokenCollector.SetActive(true);
    }

    [ContextMenu("Normal")]
    public void NormalActive()
    {
        _normalCollector.SetActive(true);
        _brokenCollector.SetActive(false);
    }
}
