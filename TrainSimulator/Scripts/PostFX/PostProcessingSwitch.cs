using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using Zenject;
using System.Collections.Generic;

public class PostProcessingSwitch : MonoBehaviour
{
    private SignalBus _signalBus;
    private GameplaySettings _settings;
    private Volume _currentVolume;

    private Dictionary<PlayerStates, Volume> _volumes;

    [Inject]
    private void Construct(SignalBus signalBus, GameplaySettings settings)
    {
        _signalBus = signalBus;
        _settings = settings;
    }

    private void Start()
    {
        AttachSettings();
        _signalBus.Subscribe<PlayerStateChangeSignal>(OnPlayerStateChangeSignal);
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<PlayerStateChangeSignal>(OnPlayerStateChangeSignal);
    }

    private void OnPlayerStateChangeSignal(PlayerStateChangeSignal signal)
    {
        SwitchVolume(signal.Value);
    }

    private void SwitchVolume(PlayerStates value)
    {
        if (!_volumes.TryGetValue(value, out Volume newVolume)) return;

        if (newVolume == _currentVolume) return;

        Volume previousVolume = _currentVolume;
        _currentVolume = newVolume;

        DOTween.To(
            () => newVolume.weight,
            x => { newVolume.weight = x; previousVolume.weight = (1f - x); },
            1f, _settings.PostProcessing.ChangeVolumeDuration);
    }

    private void AttachSettings()
    {
        _volumes = new();

        foreach (var effect in _settings.PostProcessing.Effects)
        {
            var newVolume = gameObject.AddComponent<Volume>();
            newVolume.profile = effect.Profile;
            newVolume.weight = 0f;
            _volumes.Add(effect.AssociatedState, newVolume);
        }

        _currentVolume = _volumes[PlayerStates.Moving];
        _currentVolume.weight = 1f;
    }
}