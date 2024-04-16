using TMPro;
using UnityEngine;

public class SelectSituationButton : MonoBehaviour
{
    public SituationData SituationData => _situationData;

    [SerializeField] private TMP_Text _text;

    private SituationData _situationData;
    private SituationsList _situationList;

    public void SetButton(SituationsList situationList, SituationData situationData)
    {
        _situationList = situationList;

        if (situationData == null) return;

        _situationData = situationData;
        _text.text = _situationData.Name;
    }

    public void OnClick()
    {
        if (_situationData != null)
            _situationList.OnClickSituationButton(this);
    }
}
