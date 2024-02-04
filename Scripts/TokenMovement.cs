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
                    GD.PrintRich($"[rainbow]Token in TokenMovement: " + token.Name);
                   // target = Globals.Instance.heroMoveTarget;
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
            token.countyPopulation.currentActivity = AllText.Activities.MOVING;

            GD.Print("Destination Global Position: " + destinationCounty.heroSpawn.GlobalPosition);
            target = destinationCounty.heroSpawn.GlobalPosition;
            GD.Print("Target Global Position: " + target);
            GD.Print("Token Global Position: " + token.GlobalPosition);
            MoveToken = true;
        }

        private void Move()
        {
            GD.Print("Target Global Position: " + target);

            GD.Print("Token Global Position: " + token.GlobalPosition);

            //GD.Print("Target: " + target);
            token.GlobalPosition = GlobalPosition.MoveToward(target, Globals.Instance.movementSpeed * Clock.Instance.ModifiedTimeScale);
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
            startingCounty.countyData.heroCountyPopulation.Remove(token.countyPopulation);
        }
        private void AddToDestinationCounty()
        {
            GD.Print("Add To Destination County.");
            token.countyPopulation.location = destinationCounty.countyData.countyId;
            destinationCounty.countyData.spawnTokenButtons.Add(token.spawnedTokenButton);
            token.spawnedTokenButton.Reparent(destinationCounty.heroesHBox);
            token.spawnedTokenButton.UpdateTokenTextures();

            token.Hide();

            // Add token to County Data hero token list.  Except we are going to have to determine if the hero is visiting.
            destinationCounty.countyData.heroCountyPopulation.Add(token.countyPopulation);
            CountyInfoControl.Instance.UpdateEverything();
            token.countyPopulation.destination = -1;
        }
        private void ReachedDestination()
        {
            GD.Print("Reached Destination County Population: " + token.countyPopulation.firstName);
            GD.Print("Token Destination: " + token.countyPopulation.destination);
            destinationCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(token.countyPopulation.destination);

            // This needs to happen automatically somehow.
            //selectToken.countyPopulation.currentActivity = AllText.Activities.IDLE;
            // Are you at war with the owner of the county the token just arrived at?
            if (token.countyPopulation.isArmyLeader == false)
            {
                RemoveFromStartingCounty();
                AddToDestinationCounty();
            }
            else
            {
                destinationCounty.countyData.visitingPopulation.Add(token.countyPopulation);

                foreach (War war in Globals.Instance.playerFactionData.wars)
                {
                    if (destinationCounty.countyData.factionData == war.defenderFactionData
                        || destinationCounty.countyData.factionData == war.attackerFactionData)
                    {

                        token.countyPopulation.location = destinationCounty.countyData.countyId;

                        GD.Print("New Battle!");
                        Battle battle = new(destinationCounty.countyData);
                        destinationCounty.countyData.battles.Add(battle);
                        destinationCounty.heroTokensControl.StartBattle();
                    }
                    else
                    {
                        GD.Print("No Battle!");
                    }
                }
            }
        }
    }
}