using Godot;

namespace PlayerSpace
{


    public partial class AllTokenTextures : Node
    {
        public static AllTokenTextures Instance { get; private set; }

        [Export] public Texture2D unselectedHeroTexture;
        [Export] public Texture2D selectedHeroTexture;

        [Export] public Texture2D unselectedArmyTexture;
        [Export] public Texture2D selectedArmyTexture;

        public override void _Ready()
        {
            Instance = this;
        }

        public void AssignTokenTextures(SelectToken token)
        {
            //GD.Print("Assign Token Textures: " + token.populationData.firstName + token.populationData.IsArmyLeader);
            if (token.populationData.IsArmyLeader == false)
            {
                token.selectedTexture = selectedHeroTexture;
                token.unselectedTexture = unselectedHeroTexture;
            }
            else
            {
                token.selectedTexture = selectedArmyTexture;
                token.unselectedTexture = unselectedArmyTexture;
            }
            // This is so the AI tokens get the correct textures assigned.
            if(token.populationData.factionData != Globals.Instance.playerFactionData)
            {
                token.sprite.Texture = token.unselectedTexture;
            }
        }
    }
}