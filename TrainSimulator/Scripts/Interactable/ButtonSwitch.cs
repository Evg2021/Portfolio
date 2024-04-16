using DG.Tweening;
using System;
using UnityEngine;

public class ButtonSwitch : Switch
{
    [Header("Press Animation")]
    [SerializeField][Min(0)] protected float _animationDuration;
    [SerializeField] private bool _returnToStartPosition;

    [Header("Values")]
    [SerializeField] private ButtonValue _firstValue;
    [SerializeField] private ButtonValue _secondValue;
    [SerializeField] protected int _selectedValue;

    private ButtonValue[] _values = new ButtonValue[2];

    private void Start()
    {
        _values[0] = _firstValue;
        _values[1] = _secondValue;

        transform.localPosition = _values[_selectedValue].PressedPosition;
    }

    public override void StartInteract()
    {
        base.StartInteract();

        _selectedValue ^= 1;

        PlayAnimation();
    }

    protected virtual void PlayAnimation()
    {
        transform.DOKill();

        transform
            .DOLocalMove(_values[_selectedValue].PressedPosition, _animationDuration)
            .OnComplete(() =>
            {
                if (_returnToStartPosition)
                    transform.DOLocalMove(_values[_selectedValue^1].PressedPosition, _animationDuration);
            });
    }


    public override int GetValue() => _selectedValue;

    public override string GetValueName() => _values[_selectedValue].Name;

    [Serializable]
    public class ButtonValue : Value
    {
        public Vector3 PressedPosition => _pressedPosition;

        [SerializeField] private Vector3 _pressedPosition;

        public override int GetValue()
        {
            throw new NotImplementedException();
        }
    }
}
