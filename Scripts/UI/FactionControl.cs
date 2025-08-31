using Godot;
using System;

namespace PlayerSpace;

public partial class FactionControl : Control
{
    public static FactionControl Instance { get; private set; }

    [Export] private Button closeButton;
    public override void _Ready()
    {
        Instance = this;
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        VisibilityChanged += OnFactionControlVisibilityChanged;
        closeButton.Pressed += OnCloseButtonPressed;
    }

    private void OnFactionControlVisibilityChanged()
    {
        if (Visible)
        {
            PlayerControls.Instance.AdjustPlayerControls(false);
            Clock.Instance.PauseTime();
        }
        else
        {
            PlayerControls.Instance.AdjustPlayerControls(true);
            Clock.Instance.UnpauseTime();
        }
    }

    private void OnCloseButtonPressed()
    {
        Hide();
    }
}