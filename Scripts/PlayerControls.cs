using Godot;

namespace PlayerSpace
{
    public partial class PlayerControls : Node
    {
        public override void _Input(InputEvent @event)
        {
            if (Globals.Instance.playerControlsEnabled == true)
            {
                if (@event is InputEventKey keyEvent && keyEvent.Pressed == false)
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