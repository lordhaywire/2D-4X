using Godot;
using System;

namespace PlayerSpace;

public partial class DiplomacyControl : Control
{
    public static DiplomacyControl Instance { get; private set; }

    [Export] private Button closeButton;
    
    public override void _Ready()
    {
        Instance = this;
        ConnectSignals();
    }
    
    private void OnDiplomacyControlVisibilityChanged()
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
    private void ConnectSignals()
    {
        VisibilityChanged += OnDiplomacyControlVisibilityChanged;
        closeButton.Pressed += OnCloseButtonPressed;
    }
    
    private void OnCloseButtonPressed()
    {
        Hide();
    }
}
