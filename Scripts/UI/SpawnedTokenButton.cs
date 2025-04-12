using Godot;
using System.Linq;

namespace PlayerSpace
{
    public partial class SpawnedTokenButton : Button
    {
        
        public PopulationData populationData;
        private HeroToken heroToken;
        public TextureRect tokenIconTextureRect;

        public override void _Ready()
        {
            tokenIconTextureRect = (TextureRect)GetChild(0);
        }
        public void UpdateButtonIcon()
        {
            GD.Print("Update Button Icon hero token: " + populationData.heroToken);
            GD.Print("Update Button Icon hero token sprite: " + populationData.heroToken.sprite);
            GD.Print("Update Button Icon hero token sprite texture: " + populationData.heroToken.sprite.Texture);
            GD.Print("Update Button Icon tokenIconTextureRect: " + tokenIconTextureRect);

            tokenIconTextureRect.Texture = populationData.heroToken.sprite.Texture;
        }

        public void OnButtonUp()
        {
            if(populationData.factionData == Globals.Instance.playerFactionData)
            {
                //GD.Print("You pressed the hero button.");
                heroToken = populationData.heroToken;

                heroToken.IsSelected = true;
                UpdateTokenTextures();
            }
            else
            {
                //GD.Print("You don't own this token so you can't select it, get wrecked mother fucker.");
            }
        }

        public void UpdateTokenTextures()
        {
            CountyData countyData = Globals.Instance.GetCountyDataFromLocationId(populationData.location);
            GD.Print($"County Name: {countyData.countyNode.Name} vs County Population Location {populationData.location}");
            GD.Print("County's Spawned Token Buttons List Count: " + countyData.spawnedTokenButtons.Count);
            foreach (SpawnedTokenButton spawnedTokenButton in countyData.spawnedTokenButtons.Cast<SpawnedTokenButton>())
            {
                GD.Print($"Going through buttons {spawnedTokenButton.populationData.firstName}");
                spawnedTokenButton.UpdateButtonIcon();
                //UpdateToolTip();
            }
        }

        // Moved this somewhere else, but may need to put it back here.
        private void UpdateToolTip()
        {
            TooltipText = $"{heroToken.populationData.firstName} {heroToken.populationData.lastName}";
        }

        public static void OnMouseEntered()
        {
            PlayerControls.Instance.stopClickThrough = true;
        }

        public static void OnMouseExited()
        {
            PlayerControls.Instance.stopClickThrough = false;
        }
        
    }
    
}