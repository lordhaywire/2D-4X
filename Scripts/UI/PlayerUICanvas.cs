using Godot;
using System;

public partial class PlayerUICanvas : CanvasLayer
{
    public static PlayerUICanvas Instance { get; private set; }

    [Export] public Control BattleLogControl;

    public override void _Ready()
    {
        Instance = this;
    }
}
