using Godot;

namespace PlayerSpace;

public partial class OptionsControl : Control
{
    public static OptionsControl Instance { get; private set; }

    [Export] private PanelContainer optionsPanelContainer;
    [Export] private Button saveButton;
    [Export] private Button loadButton;
    [Export] private Button developerOptionsButton;

    public override void _Ready()
    {
        Instance = this;
        SubscribeToSignals();
    }

    private void SubscribeToSignals()
    {
        VisibilityChanged += OnOptionsControlVisibilityChanged;
        saveButton.Pressed += OnSaveButtonPressed;
        loadButton.Pressed += OnLoadButtonPressed;
        developerOptionsButton.Pressed += OnDeveloperOptionButtonsPressed;
    }

    private static void OnSaveButtonPressed()
    {
        GD.Print("Save Button Pressed");
        SaveManager.Instance.SaveGame();
    }

    private static void OnLoadButtonPressed()
    {
        GD.Print("Load Button Pressed");
    }

    private static void OnDeveloperOptionButtonsPressed()
    {
        GD.Print("There are no developer options you donkey.");
    }


    private void OnOptionsControlVisibilityChanged()
    {
        if (Visible)
        {
            GD.Print("Options Panel is visible.");
            PlayerControls.Instance.stopClickThrough = true;
            Clock.Instance.PauseTime();
        }
        else
        {
            GD.Print("Options Panel is NOT visible.");
            PlayerControls.Instance.stopClickThrough = false;
            Clock.Instance.UnpauseTime();
        }
    }
}