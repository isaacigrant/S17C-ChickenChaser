using UI;
using UnityEngine;

public static class PlayerControls
{
    private static PlayerChicken _chicken;
    private static Controls _controls;

    public static void Initialize(PlayerChicken owner)
    {
        _chicken = owner;

        _controls = new Controls();

        _controls.Game.Dash.performed += context => owner.SetDashState(context.ReadValueAsButton());
        _controls.Game.Cluck.performed += context => owner.SetCluckState(context.ReadValueAsButton());
        _controls.Game.Jump.performed += context => owner.SetJumpState(context.ReadValueAsButton());

        _controls.Game.Move.performed += context => owner.SetMoveDirection(context.ReadValue<Vector2>());
        _controls.Game.Look.performed += context => owner.SetLookDirection(context.ReadValue<Vector2>());

        _controls.Game.EnableUI.performed += _ =>
        {
            Settings.OpenSettings(false);
            UseUIControls();
        };
        _controls.UI.DisableUI.performed += _ =>
        {
            Settings.CloseSettings();
            UseGameControls();
        };
    }

    public static void UseGameControls()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _controls.Game.Enable();
        _controls.UI.Disable();
    }
    public static void UseUIControls()
    {
        DisablePlayer();
        _controls.Game.Disable();
        _controls.UI.Enable();
    }
    public static void DisablePlayer()
    {
        _controls.UI.Disable();
        _controls.Game.Disable();

        _chicken.SetCluckState(false);
        _chicken.SetDashState(false);
        _chicken.SetJumpState(false);
        _chicken.SetLookDirection(Vector2.zero);
        _chicken.SetMoveDirection(Vector2.zero);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
