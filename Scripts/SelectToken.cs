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
                    if (Globals.Instance.selectedCountyPopulation != null)
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
            GD.Print("Select Token Ready. " + tokenMovement);
        }
        private void OnMouseEnter()
        {
            //GD.Print("Mouse is inside the token.");
            Globals.Instance.isInsideToken = true;
        }

        private void OnMouseExit()
        {
            //GD.Print("Mouse is outside the token.");
            Globals.Instance.isInsideToken = false;
        }

        private void OnClick(Viewport viewport, InputEvent @event, int _shapeIdx)
        {
            /*
            if (@event is InputEventMouseButton eventMouseButton && countyPopulation.factionData == Globals.Instance.playerFactionData)
            {
                if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false)
                {
                    viewport.SetInputAsHandled(); // This stops the click at this event, so it doesn't pass through.
                                                  // It doesn't seem to work all the time though.
                    GD.Print($"You have clicked on {countyPopulation.firstName} {countyPopulation.lastName}");
                    SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
                    if(Globals.Instance.CurrentlySelectedToken == null)
                    {
                        Globals.Instance.CurrentlySelectedToken = this;
                    }
                    else
                    {
                        if (Globals.Instance.CurrentlySelectedToken.Name == Name && selectCounty.heroSpawn.spawnedTokenList.Count() > 1)
                        {
                            // Move the top token to the bottom.
                            var spawnedTokenList = selectCounty.heroSpawn.spawnedTokenList;
                            Globals.Instance.CurrentlySelectedToken = null;
                            spawnedTokenList.RemoveAt(0);
                            spawnedTokenList.Insert(spawnedTokenList.Count(), this);
                            
                        }
                        else //if (selectCounty.heroSpawn.spawnedTokenList.Count() >= 1)
                        {
                            Globals.Instance.CurrentlySelectedToken = selectCounty.heroSpawn.spawnedTokenList[0];
                        }
                        
                    }
                }
            }
            */
        }
    }
}