using Godot;

namespace PlayerSpace
{
    public partial class SelectToken : CharacterBody2D
    {
        public CountyPopulation countyPopulation;
        [Export] public Sprite2D sprite;

        [Export] public Label tokenNameLabel;
        [Export] public Label stackCountLabel;

        private bool isSelected;
        public TokenMovement tokenMovement;

        public override void _Ready()
        {
            tokenMovement = GetNode<TokenMovement>("Token Movement Node2D");
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
            if (@event is InputEventMouseButton eventMouseButton)
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
        }  
    }
}