
using Godot;

namespace PlayerSpace;

public partial class AllGraphics : Node
{
    public static AllGraphics Instance {  get; private set; }

    [Export] public Texture2D leaderIconTexture;
    [Export] public Texture2D aideIconTexture;
    [Export] public Texture2D armyIconTexture;


    public override void _Ready()
    {
        Instance = this;
    }
}