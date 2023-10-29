using Godot;

namespace PlayerSpace
{
    public partial class PlayerControls : Node
    {
        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventKey keyEvent && keyEvent.Pressed == false)
            {
                if (Globals.Instance.playerControlsEnabled == true)
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
    }
}