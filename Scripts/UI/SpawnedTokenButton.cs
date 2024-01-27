using Godot;

namespace PlayerSpace
{
    public partial class SpawnedTokenButton : Button
    {

        public CountyPopulation countyPopulation;
        [Export] public TextureRect tokenIconTextureRect;

        public void OnMouseEntered()
        {
            PlayerControls.Instance.mouseOverUI = true;
            GD.Print("Hero Token Control Mouse Over UI: " + PlayerControls.Instance.mouseOverUI);
        }

        public void OnMouseExited()
        {
            PlayerControls.Instance.mouseOverUI = false;
            GD.Print("Hero Token Control Mouse Over UI: " + PlayerControls.Instance.mouseOverUI);
        }

    }
}