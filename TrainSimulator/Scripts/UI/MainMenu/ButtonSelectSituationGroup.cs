using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectSituationGroup : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Image _image;
    [SerializeField] private string _description;

    private int _groupId;
    private SituationsList _situationsList;

    public void SetButton(int groupId, string name, Sprite preview, SituationsList situationsList)
    {
        _groupId = groupId;
        _situationsList = situationsList;

        _nameText.text = name;
        _image.sprite = preview;
    }

    public void OnClick()
    {
        _situationsList.SetSituationsGroupDescription(_groupId);
    }
}