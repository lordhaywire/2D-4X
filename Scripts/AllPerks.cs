using Godot;

namespace PlayerSpace;

public partial class AllPerks : Node
{
    public static AllPerks Instance {  get; private set; }

    [Export] public PerkData[] allPerks;

    public override void _Ready()
    {
        Instance = this;
    }
}