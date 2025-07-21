using System;
using Netick.Unity;

public class CharacterInput : NetworkBehaviour
{
    private PlayerCharacterInput _playerCharacterInput;

    private bool IsLeftKeyHeld => _playerCharacterInput.IsLeftKeyHeld;
    private bool IsRightKeyHeld => _playerCharacterInput.IsRightKeyHeld;

    public event Action<int> OnHorizontalInput;
    public event Action OnHorizontalInputUp;

    public override void NetworkFixedUpdate()
    {
        if (!FetchInput(out _playerCharacterInput)) return;

        if (IsLeftKeyHeld) OnHorizontalInput?.Invoke(-1);
        if (IsRightKeyHeld) OnHorizontalInput?.Invoke(1);

        if (!IsLeftKeyHeld && !IsRightKeyHeld) OnHorizontalInput?.Invoke(0);
    }
}