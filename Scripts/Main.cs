using Godot;
using System;

public partial class Main : Node
{

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("quit"))
        {
            GetTree().Quit();
        }
    }
}
