using System;
using System.Collections;
using UnityEngine;

public abstract class Timer
{
    public TimeSpan TimePassed => _timeSpan;
    public bool CountTime = true;

    protected readonly TimerView _view;
    protected TimeSpan _timeSpan;
    protected int _seconds = 0;

    protected Coroutine _coroutine;

    public Timer(TimerView view)
    {
        _view = view;
    }

    public virtual void SetTimer()
    {
        _view.gameObject.SetActive(true);

        if (_coroutine != null)
            _view.StopCoroutine(_coroutine);

        _coroutine = _view.StartCoroutine(TimerCoroutine());
    }

    public virtual void Dispose()
    {
        _view.StopCoroutine(_coroutine);
        _view.gameObject.SetActive(false);
    }

    protected virtual IEnumerator TimerCoroutine()
    {
        yield return null;
    }
}

public class TimerDefault : Timer
{
    public TimerDefault(TimerView view) : base(view) { }

    protected override IEnumerator TimerCoroutine()
    {
        WaitForSeconds second = new(1f);

        while (true)
        {
            if (CountTime)
            {
                _seconds++;

                _timeSpan = TimeSpan.FromSeconds(_seconds);
                _view.SetText(_timeSpan.ToString(@"mm\:ss"));
            }

            yield return second;
        }
    }
}

public class TimerReverse : Timer
{
    public TimerReverse(TimerView view) : base(view) { }

    public delegate void TimeOut();
    public event TimeOut OnTimeOut = delegate { };

    private int _secondsLeft;
    private bool _autoDisableTimer;

    public void SetTimer(int secondsLeft, bool autoDisableTimer)
    {
        _secondsLeft = secondsLeft;
        _autoDisableTimer = autoDisableTimer;

        SetTimer();
    }

    protected override IEnumerator TimerCoroutine()
    {
        WaitForSeconds second = new(1f);

        while (_seconds < _secondsLeft)
        {
            if (CountTime)
            {
                _seconds++;
                _timeSpan = TimeSpan.FromSeconds(_secondsLeft - _seconds);
                _view.SetText(_timeSpan.ToString(@"mm\:ss"));
            }

            yield return second;
        }

        if (_autoDisableTimer)
            _view.gameObject.SetActive(false);

        OnTimeOut?.Invoke();
    }
}