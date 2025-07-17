using System;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{
    private bool IsLeftKeyHeld => Input.GetKey(KeyCode.LeftArrow);
    private bool IsRightKeyHeld => Input.GetKey(KeyCode.RightArrow);

    private bool IsLeftKeyUp => Input.GetKeyUp(KeyCode.LeftArrow);
    private bool IsRightKeyUp => Input.GetKeyUp(KeyCode.RightArrow);

    public event Action<int> OnHorizontalInputDown;
    public event Action OnHorizontalInputUp;

    private void Update()
    {
        if (IsLeftKeyHeld) OnHorizontalInputDown?.Invoke(-1);
        if (IsRightKeyHeld) OnHorizontalInputDown?.Invoke(1);

        if (IsLeftKeyUp || IsRightKeyUp) OnHorizontalInputUp?.Invoke();
    }
}