using Godot;
using System;

namespace PlayerSpace
{
    public partial class PlayerUICanvas : CanvasLayer
    {
        public static PlayerUICanvas Instance { get; private set; }

        [Export] public Control BattleLogControl;
        [Export] public HeroPanelContainer selectedHeroPanelContainer;

        public override void _Ready()
        {
            Instance = this;
        }
    }
}