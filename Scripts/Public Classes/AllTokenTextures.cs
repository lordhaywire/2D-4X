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

        // I think this is missing the Faction Leader option.
        public void AssignTokenTextures(HeroToken heroToken)
        {
            //GD.Print("Assign Token Textures: " + token.populationData.firstName + token.populationData.IsArmyLeader);
            // This is so the AI tokens get the correct textures assigned.
            // I don't know why there isn't an army option in this if statement.
            if (heroToken.populationData.factionData != Globals.Instance.playerFactionData)
            {
                heroToken.sprite.Texture = heroToken.unselectedTexture;
                return;
            }
            // We don't actually have 4 different textures, but we will at some point, which is why this is this way.
            switch (heroToken.populationData.HeroType)
            {
                case AllEnums.HeroType.Aide:
                    heroToken.selectedTexture = selectedHeroTexture;
                    heroToken.unselectedTexture = unselectedHeroTexture;
                    return;
                case AllEnums.HeroType.FactionLeader:
                    heroToken.selectedTexture = selectedHeroTexture;
                    heroToken.unselectedTexture = unselectedHeroTexture;
                    return;
                case AllEnums.HeroType.ArmyLeader:
                    heroToken.selectedTexture = selectedArmyTexture;
                    heroToken.unselectedTexture = unselectedArmyTexture;
                    return;
                case AllEnums.HeroType.FactionLeaderArmyLeader:
                    heroToken.selectedTexture = selectedArmyTexture;
                    heroToken.unselectedTexture = unselectedArmyTexture;
                    return;
                default:
                    GD.Print("Assign Token Textures is messed up.");
                    return;
            }
        }
    }
}