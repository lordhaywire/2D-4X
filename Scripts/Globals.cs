using Godot;

namespace PlayerSpace
{
    public partial class Globals : Node2D
    {
        public static Globals Instance { get; private set; }

        [Export] public bool playerControlsEnabled;

        [Export] public Control countyInfoControl;
        [Export] public Label countyNameLabel;
        public override void _Ready()
        {
            Instance = this;
        }


    }
}

