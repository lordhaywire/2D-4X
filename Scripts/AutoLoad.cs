using Godot;

namespace PlayerSpace;

public partial class AutoLoad : Node
{
    public static AutoLoad Instance { get; private set; }

    public bool gameStarted = false;

    public override void _Ready()
    {
        Instance = this;
        //GD.Print("AutoLoad.cs has auto loaded.");
    }
}