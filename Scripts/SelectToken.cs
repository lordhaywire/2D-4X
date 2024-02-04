using Godot;

namespace PlayerSpace
{
    public partial class SelectToken : CharacterBody2D
    {
        public CountyPopulation countyPopulation;
        [Export] public Sprite2D sprite;

        [Export] public Texture2D selectedTexture;
        [Export] public Texture2D unselectedTexture;

        [Export] public Label tokenNameLabel;
        [Export] public Label stackCountLabel; // This can probably be deleted.

        [Export] public TokenMovement tokenMovement;

        [Export] private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if (value == true)
                {
                    GD.Print("Token County Population? " + countyPopulation.firstName);
                    sprite.Texture = selectedTexture;
                    if (Globals.Instance.selectedCountyPopulation != null && countyPopulation != Globals.Instance.selectedCountyPopulation)
                    {
                        SelectToken currentSelectToken = (SelectToken)Globals.Instance.selectedCountyPopulation.token;
                        GD.PrintRich("[rainbow]Current Select Token Value True: " + currentSelectToken.Name);
                        currentSelectToken.IsSelected = false;
                    }
                    Globals.Instance.selectedCountyPopulation = countyPopulation;
                    GD.Print("Globals Instance County Population: " + Globals.Instance.selectedCountyPopulation.firstName);

                }
                else
                {
                    sprite.Texture = unselectedTexture;
                }
                GD.Print($"{Name} selection is: " + value);
            }
        }

        public SpawnedTokenButton spawnedTokenButton;

        public override void _Ready()
        {
            tokenMovement.token = this;
        }

        private void OnMouseEnter()
        {
            //GD.Print("Mouse is inside the token.");
            PlayerControls.Instance.stopClickThrough = true;

        }

        private void OnMouseExit()
        {
            //GD.Print("Mouse is outside the token.");
            PlayerControls.Instance.stopClickThrough = false;
        }

        private void OnClick(Viewport viewport, InputEvent @event, int _shapeIdx)
        {
            
            if (@event is InputEventMouseButton eventMouseButton && countyPopulation.factionData == Globals.Instance.playerFactionData)
            {
                if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false)
                {
                    GD.Print($"You have clicked on {countyPopulation.firstName} {countyPopulation.lastName}");
                    IsSelected = true;
                }
            }
            
        }
    }
}