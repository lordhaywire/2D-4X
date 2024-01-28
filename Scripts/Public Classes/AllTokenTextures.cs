using Godot;

namespace PlayerSpace
{


    public partial class AllTokenTextures : Node
    {
        public static AllTokenTextures Instance { get; private set; }

        [Export] public Texture2D unselectedHeroTexture;
        [Export] public Texture2D selectedHeroTexture;

        public override void _Ready()
        {
            Instance = this;
        }
    }
}