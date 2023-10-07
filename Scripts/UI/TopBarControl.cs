using Godot;

namespace PlayerSpace
{
    public partial class TopBarControl : Control
    {
        [Export] private Clock clock;

        public void ChangeSpeed(int speed)
        {
            clock.ChangeSpeed(speed);
        }
    }
}