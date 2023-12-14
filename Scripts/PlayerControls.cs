using Godot;

namespace PlayerSpace
{
    public partial class PlayerControls : Node
    {
        public static PlayerControls Instance { get; private set; }

        public bool playerControlsEnabled = true;

        public override void _Ready()
        {
            Instance = this;
        }
        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventKey keyEvent && keyEvent.Pressed == false)
            {
                if (playerControlsEnabled == true)
                {
                    GD.Print($"{keyEvent.Keycode}");
                    switch (keyEvent.Keycode)
                    {
                        case Key.Space:
                            Clock.Instance.PauseandUnpause();
                            break;
                    }
                }
            }
        }

        public void AdjustPlayerControls(bool controls)
        {
            playerControlsEnabled = controls;
        }
    }
}