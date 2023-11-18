using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class SelectToken : CharacterBody2D
    {
        [Export] public CountyPopulation countyPopulation;
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
                    SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
                    GD.Print($"Selected Token Name: {Globals.Instance.CurrentlySelectedToken.Name}");
                    GD.Print($"This name: {countyPopulation.firstName} and {Name}");
                    GD.Print($"the count of the list is {selectCounty.heroSpawn.spawnedTokenList.Count()}");
                    if (Globals.Instance.CurrentlySelectedToken.Name == Name && selectCounty.heroSpawn.spawnedTokenList.Count() > 1)
                    {
                        // Move the top token to the bottom.
                        GD.Print($"Move {Name} to bottom");
                        var spawnedTokenList = selectCounty.heroSpawn.spawnedTokenList;
                        spawnedTokenList.Insert(spawnedTokenList.Count(), this);
                        spawnedTokenList.RemoveAt(0);
                        Globals.Instance.CurrentlySelectedToken = null;
                    }
                    else
                    {
                        Globals.Instance.CurrentlySelectedToken = this;
                    }
                    //GD.Print($"You clicked on a hero! Motherfucker: {Globals.Instance.CurrentlySelectedToken.Name}");

                    GD.Print($"{countyPopulation.firstName} is in {selectCounty.countyData.countyName}");
                }
            }
        }  
    }
}