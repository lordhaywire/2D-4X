using Godot;
using System.Linq;

namespace PlayerSpace
{
    public partial class SpawnedTokenButton : Button
    {
        public CountyPopulation countyPopulation;
        private SelectToken selectToken;
        [Export] public TextureRect tokenIconTextureRect;

        public void UpdateButtonIcon()
        {
            selectToken = countyPopulation.token;
            tokenIconTextureRect.Texture = selectToken.sprite.Texture;
        }
        public void OnButtonUp()
        {
            if(countyPopulation.factionData == Globals.Instance.playerFactionData)
            {
                GD.Print("You pressed the hero button.");
                selectToken = countyPopulation.token;

                selectToken.IsSelected = true;
                UpdateTokenTextures();
            }
            else
            {
                GD.Print("Get wrecked mother fucker.");
            }
        }

        public void UpdateTokenTextures()
        {
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
            GD.Print($"Select County Name: {selectCounty.Name} vs County Population Location {countyPopulation.location}");
            GD.Print("Select Counties Spawned Token Buttons List Count: " + selectCounty.countyData.spawnTokenButtons.Count);
            foreach (SpawnedTokenButton spawnedTokenButton in selectCounty.countyData.spawnTokenButtons.Cast<SpawnedTokenButton>())
            {
                GD.Print($"Going through buttons {spawnedTokenButton.countyPopulation.firstName}");
                spawnedTokenButton.UpdateButtonIcon();
            }
        }
        public void OnMouseEntered()
        {
            PlayerControls.Instance.stopClickThrough = true;
            //GD.Print("Hero Token Control Mouse Over UI: " + PlayerControls.Instance.mouseOverUI);
        }

        public void OnMouseExited()
        {
            PlayerControls.Instance.stopClickThrough = false;
            //GD.Print("Hero Token Control Mouse Over UI: " + PlayerControls.Instance.mouseOverUI);
        }

    }
}