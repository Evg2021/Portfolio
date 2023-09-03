using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyboard : InputBase
{
    public override event Action<Vector2> FirstKeyIsDown;
    public override event Action<Vector2> SecondKeyIsDown;
    public override event Action<Vector2> FirstKeyIsUp;
    public override event Action<Vector2> SecondKeyIsUp;
    public override event Action<Vector2> FirstKeyIsHold;

    public event Action HotKeyToOpenImage;
    public event Action HotKeyCancel;
    public event Action HotKeyApprove;
    public event Action HotKeySave;
    public event Action HotKeyLoad;
    public event Action HotKeyDelete;

    private HotKey keyToOpenImage = new HotKey(new KeyCode[] { KeyCode.LeftControl, KeyCode.A });
    private HotKey Cancel         = new HotKey(new KeyCode[] { KeyCode.Escape });
    private HotKey Approve        = new HotKey(new KeyCode[] { KeyCode.Return });
    private HotKey Save           = new HotKey(new KeyCode[] { KeyCode.LeftControl, KeyCode.S });
    private HotKey Load           = new HotKey(new KeyCode[] { KeyCode.LeftControl, KeyCode.O });
    private HotKey Delete         = new HotKey(new KeyCode[] { KeyCode.Delete });

    private Vector2 startPosition;

    public override Vector2 DiffCursorPosition()
    {
        return (Input.mousePosition.GetXY() - startPosition);
    }

    public override bool GetFirstKey()
    {
        return Input.GetMouseButton(0);
    }

    public override bool GetSecondKey()
    {
        return Input.GetMouseButton(1);
    }

    public Vector2 GetArrowsInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        return new Vector2(x, y);
    }

    private void CheckHotKey(HotKey hotKey, Action action, bool onHoldDown = true)
    {
        for (int i = 0; i < hotKey.Keys.Length; i++)
        {
            if (onHoldDown)
            {
                if (!Input.GetKey(hotKey.Keys[i]))
                    break;
            }
            else
            {
                if (!Input.GetKeyDown(hotKey.Keys[i]))
                    break;
            }

            if (i == hotKey.Keys.Length - 1)
                action?.Invoke();
        }
    }

    private void CheckAllHotKeys()
    {
        CheckHotKey(keyToOpenImage, HotKeyToOpenImage, true );
        CheckHotKey(Cancel        , HotKeyCancel     , false);
        CheckHotKey(Approve       , HotKeyApprove    , false);
        CheckHotKey(Save          , HotKeySave       , true );
        CheckHotKey(Load          , HotKeyLoad       , true );
        CheckHotKey(Delete        , HotKeyDelete     , true );
    }

    private void Update()
    {
        CheckAllHotKeys();

        if (Input.GetMouseButtonDown(0))
        {
            FirstKeyIsDown?.Invoke(Input.mousePosition);
            startPosition = Input.mousePosition.GetXY();
        }

        if(Input.GetMouseButton(0))
        {
            FirstKeyIsHold?.Invoke(Input.mousePosition);
        }

        if (Input.GetMouseButtonDown(1))
        {
            SecondKeyIsDown?.Invoke(Input.mousePosition);
        }

        if(Input.GetMouseButtonUp(0))
        {
            FirstKeyIsUp?.Invoke(Input.mousePosition);
        }

        if(Input.GetMouseButtonUp(1))
        {
            SecondKeyIsUp?.Invoke(Input.mousePosition);
        }
    }

    public override Vector2 CursorPosition()
    {
        return Input.mousePosition.GetXY();
    }

    public override float ScrollDelta()
    {
        return Input.mouseScrollDelta.y;
    }

    public override void Refresh()
    {
        startPosition = Input.mousePosition.GetXY();
    }

    private struct HotKey
    {
        public KeyCode[] Keys;

        public HotKey(KeyCode[] keyCodes)
        {
            Keys = keyCodes;
        }

        public HotKey(List<KeyCode> keyCodes)
        {
            Keys = keyCodes.ToArray();
        }

        public HotKey(KeyCode keyCode)
        {
            Keys = new KeyCode[]{ keyCode};
        }
    }
}
