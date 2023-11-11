using Godot;

namespace PlayerSpace
{
    public partial class SelectToken : CharacterBody2D
    {
        [Export] public CountyPopulation countyPopulation;
        [Export] private Sprite2D sprite;
        [Export] private Texture2D selectedTexture;
        [Export] private Texture2D unselectedTexture;

        [Export] private Label tokenNameLabel;

        private bool isSelected;
        public TokenMovement tokenMovement;

        public override void _Ready()
        {
            tokenMovement = GetNode<TokenMovement>("Token Movement Node2D");
        }
        [Export] public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if(isSelected == true) 
                {
                    sprite.Texture = selectedTexture;
                    Globals.Instance.selectedToken = this;
                }
                else
                {
                    sprite.Texture = unselectedTexture;
                    Globals.Instance.selectedToken = null;
                }
            }
        }

        private void OnClick(Viewport viewport, InputEvent @event, int _shapeIdx)
        {
            if (@event is InputEventMouseButton eventMouseButton)
            {
                if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false)
                {

                    viewport.SetInputAsHandled(); // This stops the click at this event, so it doesn't pass through.                
                    IsSelected = true;
                    GD.Print($"You clicked on a hero! Motherfucker: {Globals.Instance.selectedToken.Name} and isSelected: {IsSelected}");
                    SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
                    GD.Print($"{countyPopulation.firstName} is in {selectCounty.countyData.countyName}");
                }
            }
        }  
    }
}