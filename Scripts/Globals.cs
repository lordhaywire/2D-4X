using Godot;

namespace PlayerSpace
{
    public partial class Globals : Node2D
    {
        public static Globals Instance { get; private set; }

        public int researchClicked;
        [Export] public bool playerControlsEnabled = true;

        [Export] public Control countyInfoControl;
        [Export] public Label countyNameLabel;
        public override void _Ready()
        {
            Instance = this;
        }
    }
}

