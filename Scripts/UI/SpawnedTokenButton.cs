using Godot;
using System.Linq;

namespace PlayerSpace
{
    public partial class SpawnedTokenButton : Button
    {
        public CountyPopulation countyPopulation;
        [Export] public TextureRect tokenIconTextureRect;

        public void UpdateButtonIcon()
        {
            SelectToken selectToken = (SelectToken)countyPopulation.token;
            tokenIconTextureRect.Texture = selectToken.sprite.Texture;
        }
        public void OnButtonUp()
        {
            GD.Print("You pressed the hero button.");
            Globals.Instance.selectedCountyPopulation = countyPopulation;
            UpdateTokenTextures();
        }

        public void UpdateTokenTextures()
        {
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
            GD.Print($"Select County Name: {selectCounty.Name} vs County Population Location {countyPopulation.location}");
            GD.Print("Select Counties List Count: " + selectCounty.spawnTokenButtons.Count);
            foreach (SpawnedTokenButton spawnedTokenButton in selectCounty.spawnTokenButtons.Cast<SpawnedTokenButton>())
            {
                SelectToken selectToken = (SelectToken)spawnedTokenButton.countyPopulation.token;

                if (spawnedTokenButton.countyPopulation != Globals.Instance.selectedCountyPopulation)
                {
                    selectToken.sprite.Texture = selectToken.unselectedTexture;
                }
                else
                {
                    selectToken.sprite.Texture = selectToken.selectedTexture;
                }
                GD.Print("Going through buttons.");
                spawnedTokenButton.UpdateButtonIcon();
            }
        }
        public void OnMouseEntered()
        {
            PlayerControls.Instance.mouseOverUI = true;
            //GD.Print("Hero Token Control Mouse Over UI: " + PlayerControls.Instance.mouseOverUI);
        }

        public void OnMouseExited()
        {
            PlayerControls.Instance.mouseOverUI = false;
            //GD.Print("Hero Token Control Mouse Over UI: " + PlayerControls.Instance.mouseOverUI);
        }

    }
}