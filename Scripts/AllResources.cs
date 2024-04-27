using Godot;
using PlayerSpace;
using System;

public partial class AllResources : Node
{
    public static AllResources Instance { get; private set; }
    
    [Export] public ResourceData[] allResources;

    public override void _Ready()
    {
        Instance = this;
    }
}
