using Godot;

namespace PlayerSpace
{
    public partial class SelectToken : CharacterBody2D
    {
        [Export] public HeroData hero;

        [Export] private Sprite2D sprite;
        [Export] private Texture2D selectedTexture;
        [Export] private Texture2D unselectedTexture;

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if(isSelected == true) 
                {
                    sprite.Texture = selectedTexture;
                }
                else
                {
                    sprite.Texture = unselectedTexture;
                }
            }
        }

        private void OnClick(Viewport _viewport, InputEvent @event, int _shapeIdx)
        {
            if (@event is InputEventMouseButton eventMouseButton)
            {
                if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false)
                {
                    GD.Print("You clicked on a hero! Motherfucker.");
                    Globals.Instance.selectedToken = this;
                    IsSelected = true;
                }
            }
        }  
    }
}