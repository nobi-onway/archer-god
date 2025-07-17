using System;
using Netick.Unity;

public class CharacterInput : NetworkBehaviour
{
    private PlayerCharacterInput _playerCharacterInput;

    private bool IsLeftKeyHeld => _playerCharacterInput.IsLeftKeyHeld;
    private bool IsRightKeyHeld => _playerCharacterInput.IsRightKeyHeld;
    private bool IsLeftKeyUp => _playerCharacterInput.IsLeftKeyUp;
    private bool IsRightKeyUp => _playerCharacterInput.IsRightKeyUp;

    public event Action<int> OnHorizontalInputDown;
    public event Action OnHorizontalInputUp;

    public override void NetworkFixedUpdate()
    {
        if (!FetchInput(out _playerCharacterInput)) return;

        if (IsLeftKeyHeld) OnHorizontalInputDown?.Invoke(-1);
        if (IsRightKeyHeld) OnHorizontalInputDown?.Invoke(1);

        if (IsLeftKeyUp || IsRightKeyUp) OnHorizontalInputUp?.Invoke();
    }
}