using Godot;
using System;

namespace PlayerSpace
{
    public partial class Globals : Node2D
    {
        public static Globals Instance { get; private set; }

        [Export] public PopupPanel countyInfoPanel;
        public override void _Ready()
        {
            Instance = this;
        }


    }
}

