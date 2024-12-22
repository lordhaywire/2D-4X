using Godot;
using System;
using System.Linq;

namespace PlayerSpace
{
    public partial class SpawnedTokenButton : Button
    {
        public PopulationData populationData;
        private SelectToken selectToken;
        [Export] public TextureRect tokenIconTextureRect;

        public void UpdateButtonIcon()
        {
            selectToken = populationData.token;
            tokenIconTextureRect.Texture = selectToken.sprite.Texture;
        }
        public void OnButtonUp()
        {
            if(populationData.factionData == Globals.Instance.playerFactionData)
            {
                //GD.Print("You pressed the hero button.");
                selectToken = populationData.token;

                selectToken.IsSelected = true;
                UpdateTokenTextures();
            }
            else
            {
                //GD.Print("You don't own this token so you can't select it, wrecked mother fucker.");
            }
        }

        public void UpdateTokenTextures()
        {
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(populationData.location);
            //GD.Print($"Select County Name: {selectCounty.Name} vs County Population Location {populationData.location}");
            //GD.Print("Select Counties Spawned Token Buttons List Count: " + selectCounty.countyData.spawnedTokenButtons.Count);
            foreach (SpawnedTokenButton spawnedTokenButton in selectCounty.countyData.spawnedTokenButtons.Cast<SpawnedTokenButton>())
            {
                //GD.Print($"Going through buttons {spawnedTokenButton.populationData.firstName}");
                spawnedTokenButton.UpdateButtonIcon();
                //UpdateToolTip();
            }
        }

        // Moved this somewhere else, but may need to put it back here.
        private void UpdateToolTip()
        {
            TooltipText = $"{selectToken.populationData.firstName} {selectToken.populationData.lastName}";
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