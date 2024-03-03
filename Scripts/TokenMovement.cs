using Godot;
using System;

namespace PlayerSpace
{
    public partial class TokenMovement : Node2D
    {
        [Export] public SelectToken token;
        [Export] private bool moveToken;
        private Vector2 target;
        private SelectCounty destinationCounty;

        public bool MoveToken
        {
            get { return moveToken; }
            set
            {
                moveToken = value;
                if (moveToken == true)
                {
                    token.countyPopulation.lastLocation = token.countyPopulation.location;
                    token.Show();
                }
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (MoveToken == true)
            {
                Move();
            }
        }

        public void StartMove(int destinationCountyID) // Let's move this to token movement.
        {
            GD.Print($"{token.countyPopulation.firstName} has location of {token.countyPopulation.location}");
            SelectCounty destinationCounty
                = (SelectCounty)Globals.Instance.countiesParent.GetChild(destinationCountyID);

            token.countyPopulation.destination = destinationCountyID;
            token.UpdateCurrentActivity(AllText.Activities.MOVING);

            //GD.Print("Destination Global Position: " + destinationCounty.heroSpawn.GlobalPosition);
            target = destinationCounty.heroSpawn.GlobalPosition;
            //GD.Print("Target Global Position: " + target);
            //GD.Print("Token Global Position: " + token.GlobalPosition);

            CheckIfRetreating(); 
            
            MoveToken = true;
        }

        private void CheckIfRetreating()
        {
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(token.countyPopulation.location);
            
            if (selectCounty.countyData.battles.Count > 0)
            {
                selectCounty.battleControl.EndBattle();
            }
        }
        private void Move()
        {
            //GD.Print("Target Global Position: " + target);
            //GD.Print("Token Global Position: " + token.GlobalPosition);

            //GD.Print("Target: " + target);
            float speed = Globals.Instance.movementSpeed * Clock.Instance.ModifiedTimeScale;
            token.GlobalPosition = GlobalPosition.MoveToward(target, speed);
            //GD.Print($"{GetParent().Name} is moving!");
            if (token.GlobalPosition.IsEqualApprox(target))
            {
                ReachedDestination();
                MoveToken = false;
            }
        }

        private void RemoveFromStartingCounty()
        {
            GD.Print("Removed token from Starting County");
            // Remove countyPopulation from the heroes starting county location list.
            SelectCounty startingCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(token.countyPopulation.location);
            if (startingCounty.countyData.factionData == Globals.Instance.playerFactionData)
            {
                startingCounty.countyData.heroCountyList.Remove(token.countyPopulation);
            }
            else
            {
                startingCounty.countyData.visitingPopulationList.Remove(token.countyPopulation);
            }
        }
        private void AddToDestinationCounty()
        {
            GD.Print("Add To Destination County.");
            token.countyPopulation.location = destinationCounty.countyData.countyId;
            destinationCounty.countyData.spawnTokenButtons.Add(token.spawnedTokenButton);
            if (token.countyPopulation.IsArmyLeader == false)
            {
                token.spawnedTokenButton.Reparent(destinationCounty.heroesHBox);
            }
            else
            {
                token.spawnedTokenButton.Reparent(destinationCounty.armiesHBox);
            }
            token.spawnedTokenButton.UpdateTokenTextures();

            token.UpdateCurrentActivity(AllText.Activities.IDLE);
            token.countyPopulation.nextActivity = AllText.Activities.IDLE;
            token.Hide();

            // Add token to County Data hero token list.  Except we are going to have to determine if the hero is visiting.
            if (destinationCounty.countyData.factionData == Globals.Instance.playerFactionData)
            {
                destinationCounty.countyData.heroCountyList.Add(token.countyPopulation);
            }
            else
            {
                destinationCounty.countyData.visitingPopulationList.Add(token.countyPopulation);
            }
            token.countyPopulation.destination = -1; // This is -1 because this is like a "null" int.
            CountyInfoControl.Instance.UpdateEverything();

            CheckForBattles();

            token.isRetreating = false;
        }
        private void ReachedDestination()
        {
            GD.Print("Reached Destination County Population: " + token.countyPopulation.firstName);
            GD.Print("Token Destination: " + token.countyPopulation.destination);
            destinationCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(token.countyPopulation.destination);

            // This needs to happen automatically somehow.
            //selectToken.countyPopulation.currentActivity = AllText.Activities.IDLE;
            // Are you at war with the owner of the county the token just arrived at?
            if (token.countyPopulation.IsArmyLeader == false
                || destinationCounty.countyData.factionData == Globals.Instance.playerFactionData)
            {
                RemoveFromStartingCounty();
                AddToDestinationCounty();
            }
            else
            {
                RemoveFromStartingCounty();
                AddToDestinationCounty();

                CountyInfoControl.Instance.UpdateEverything();
            }
        }

        // ChatGPT refactored the code below this.
        private void CheckForBattles()
        {
            SelectToken selectToken = (SelectToken)GetParent();

            if (selectToken.isRetreating)
            {
                GD.Print("Is retreating!");
                token.isRetreating = false; // Reset isRetreating to false.
                return;
            }

            foreach (War war in selectToken.countyPopulation.factionData.wars)
            {
                if (IsCountyPartOfWar(war, selectToken))
                {
                    GD.Print("New Battle!" + destinationCounty.countyData.countyId);
                    HandleBattle();
                }
                else
                {
                    GD.Print("No Battle!");
                }
            }
            token.isRetreating = false; // Reset isRetreating to false just in case.
        }

        private bool IsCountyPartOfWar(War war, SelectToken selectToken)
        {
            CountyData countyData = destinationCounty.countyData;
            FactionData aggressorFactionData = war.aggressorFactionData;
            FactionData defenderFactionData = war.defenderFactionData;
            FactionData selectTokenFactionData = selectToken.countyPopulation.factionData;
            return (countyData.factionData == aggressorFactionData || countyData.factionData == defenderFactionData) 
                && selectTokenFactionData != countyData.factionData;
        }

        private void HandleBattle()
        {
            token.UpdateCurrentActivity(AllText.Activities.COMBAT);
            Battle battle = new(destinationCounty.countyData);
            destinationCounty.countyData.battles.Add(battle);
            destinationCounty.battleControl.StartBattle(battle);
        }
    }
}