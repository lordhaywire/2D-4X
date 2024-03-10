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

            // Check to see if the starting county has battles and if it does, it should end the battle because the
            // token is retreating.
            if (selectCounty.countyData.battles.Count > 0)
            {
                selectCounty.battleControl.EndBattle();
            }
        }
        private void Move()
        {
            float speed = Globals.Instance.movementSpeed * Clock.Instance.ModifiedTimeScale;
            token.GlobalPosition = GlobalPosition.MoveToward(target, speed);
            if (token.GlobalPosition.IsEqualApprox(target))
            {
                ReachedDestination();
            }
        }

        private void ReachedDestination()
        {
            MoveToken = false;
            GD.Print("Top of Reached Destination County Population: " + token.countyPopulation.firstName);
            GD.Print("Token Destination: " + token.countyPopulation.destination);
            destinationCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(token.countyPopulation.destination);
            if (destinationCounty.countyData.factionData == token.countyPopulation.factionData)
            {
                if (token.countyPopulation.IsArmyLeader == false)
                {
                    HeroReachedCounty();
                    token.UpdateCurrentActivity(AllText.Activities.IDLE);
                    token.UpdateNextActivity(AllText.Activities.IDLE);
                }
                else
                {
                    ArmyReachedCounty();
                    token.UpdateCurrentActivity(AllText.Activities.IDLE);
                    token.UpdateNextActivity(AllText.Activities.IDLE);
                }
            }
            else
            {
                if (token.countyPopulation.IsArmyLeader == false)
                {
                    HeroVisitingCounty();
                    // We will probably need to change this to what the token occupation does.
                    // For example, if the token is a diplomat, then the activity will be diplmating.
                    token.UpdateCurrentActivity(AllText.Activities.IDLE);
                    token.UpdateNextActivity(AllText.Activities.IDLE);
                }
                else
                {
                    ArmyAttackingCounty();
                    token.UpdateCurrentActivity(AllText.Activities.COMBAT);
                    token.UpdateNextActivity(AllText.Activities.COMBAT);
                }

            }
            RemoveFromStartingCounty();
            AddToDestinationCounty();
            CountyInfoControl.Instance.UpdateEverything();
            token.countyPopulation.destination = -1; // This is -1 because this is like a "null" int.
            token.spawnedTokenButton.UpdateTokenTextures();
            token.isRetreating = false;
            token.Hide();
        }
        private void ArmyAttackingCounty()
        {
            token.spawnedTokenButton.Reparent(destinationCounty.armiesHBox);
            destinationCounty.countyData.visitingPopulationList.Add(token.countyPopulation);

            Battle battle = new(destinationCounty.countyData);
            destinationCounty.countyData.battles.Add(battle);
            destinationCounty.battleControl.StartBattle(battle);
        }

        private void AddToDestinationCounty()
        {
            GD.Print("Add To Destination County " + token.countyPopulation.firstName);
            token.countyPopulation.location = destinationCounty.countyData.countyId;
            destinationCounty.countyData.spawnTokenButtons.Add(token.spawnedTokenButton);
        }
        private void RemoveFromStartingCounty()
        {
            GD.Print($"Removed token from Starting County {token.countyPopulation.firstName} {token.countyPopulation.location}");
            GD.Print($"Removed {token.countyPopulation.firstName} {token.countyPopulation.factionData.factionName}");
            // Remove countyPopulation from the heroes starting county location list.
            SelectCounty startingCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(token.countyPopulation.location);

            // We don't need to check which list the hero is in because C# doesn't give a shit if the hero isn't in the list.
            // So we just try to remove it from both and it will remove it from the correct one.
            startingCounty.countyData.heroCountyList.Remove(token.countyPopulation);
            startingCounty.countyData.visitingPopulationList.Remove(token.countyPopulation);
        }

        private void HeroReachedCounty()
        {
            token.spawnedTokenButton.Reparent(destinationCounty.heroesHBox);
            destinationCounty.countyData.heroCountyList.Add(token.countyPopulation);
        }

        private void ArmyReachedCounty()
        {
            GD.Print("Army Reached County: " + token.countyPopulation.firstName);
            token.spawnedTokenButton.Reparent(destinationCounty.armiesHBox);
            destinationCounty.countyData.heroCountyList.Add(token.countyPopulation);
        }
        private void HeroVisitingCounty()
        {
            token.spawnedTokenButton.Reparent(destinationCounty.heroesHBox);
            destinationCounty.countyData.visitingPopulationList.Add(token.countyPopulation);
        }
    }
}