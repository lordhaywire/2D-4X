using Godot;
using System;

namespace PlayerSpace
{
    public partial class TokenMovement : Node2D
    {
        [Export] public SelectToken token;
        [Export] private bool moveToken;
        private Vector2 target;
        private County destinationCounty;

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
            destinationCounty
                = (County)Globals.Instance.countiesParent.GetChild(destinationCountyID);

            token.countyPopulation.destination = destinationCountyID;
            token.RemoveFromResearch();

            token.countyPopulation.UpdateActivity(AllEnums.Activities.Move);

            //GD.Print("Destination Global Position: " + destinationCounty.heroSpawn.GlobalPosition);
            target = destinationCounty.heroSpawn.GlobalPosition;
            //GD.Print("Target Global Position: " + target);
            //GD.Print("Token Global Position: " + token.GlobalPosition);
            CheckForDefenders();
            CheckIfRetreating();

            MoveToken = true;
        }

        private void CheckForDefenders()
        {
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(token.countyPopulation.destination);

            if (token.countyPopulation.factionData.factionWarDictionary
                [selectCounty.countyData.factionData.factionName] == true && DefenderOnTheWay() == false)
            {
                selectCounty.countyData.factionData.diplomacy.DefenderSpawnArmies(destinationCounty);
                EventLog.Instance.AddLog($"{selectCounty.countyData.factionData.factionName}" +
                        $" is raising armies at {selectCounty.countyData.countyName}!");
            }
            else
            {
                GD.Print("Checking for Defenders - Defender on the way, or not at war.");
            }
        }

        private bool DefenderOnTheWay()
        {
            // Get the All Heroes List in the desination county for that county's faction and see if any of that
            // factions heros are on the way to it.
            //GD.Print("Seeing if someone is on the way.");
            foreach (CountyPopulation countyPopulation in destinationCounty.countyData.factionData.allHeroesList)
            {
                if (countyPopulation.destination == token.countyPopulation.destination)
                {
                    GD.Print("Hero on the way is: " + countyPopulation.firstName);
                    return true;
                }
            }
            return false;
        }

        private void CheckIfRetreating()
        {
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(token.countyPopulation.location);

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
            destinationCounty = (County)Globals.Instance.countiesParent.GetChild(token.countyPopulation.destination);
            GD.Print("Faction of Destination County: " + destinationCounty.countyData.factionData.factionName);
            if (destinationCounty.countyData.factionData == token.countyPopulation.factionData)
            {
                if (token.countyPopulation.IsArmyLeader == false)
                {
                    HeroReachedCounty();
                    token.countyPopulation.UpdateActivity(AllEnums.Activities.Idle);
                }
                else
                {
                    ArmyReachedCounty();
                    token.countyPopulation.UpdateActivity(AllEnums.Activities.Idle);
                }
            }
            else
            {
                if (token.countyPopulation.IsArmyLeader == false)
                {
                    HeroVisitingCounty();
                    // We will probably need to change this to what the token occupation does.
                    // For example, if the token is a diplomat, then the activity will be diplmating.
                    token.countyPopulation.UpdateActivity(AllEnums.Activities.Idle);
                }
                else
                {
                    ArmyAttackingCounty();
                    token.countyPopulation.UpdateActivity(AllEnums.Activities.Combat);
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
            ArmyVisitingCounty();
            if (destinationCounty.countyData.armiesInCountyList.Count() > 0)
            {
                Battle battle = new(destinationCounty.countyData);
                destinationCounty.countyData.battles.Add(battle);
                destinationCounty.battleControl.StartBattle(battle);
            }
            else
            {
                CountyDictator.Instance.CaptureCounty(token.countyPopulation.destination, token.countyPopulation.factionData);
            }
        }

        private void AddToDestinationCounty()
        {
            //GD.Print("Add To Destination County " + token.countyPopulation.firstName);
            token.countyPopulation.location = destinationCounty.countyData.countyId;
            destinationCounty.countyData.spawnedTokenButtons.Add(token.spawnedTokenButton);
        }
        private void RemoveFromStartingCounty()
        {
            //GD.Print($"Removed token from Starting County {token.countyPopulation.firstName} {token.countyPopulation.location}");
            //GD.Print($"Removed {token.countyPopulation.firstName} {token.countyPopulation.factionData.factionName}");
            // Remove countyPopulation from the heroes starting county location list.
            County startingCounty = (County)Globals.Instance.countiesParent.GetChild(token.countyPopulation.location);

            // We don't need to check which list the hero is in because C# doesn't give a shit if the hero isn't in the list.
            // So we just try to remove it from both and it will remove it from the correct one.
            startingCounty.countyData.herosInCountyList.Remove(token.countyPopulation);
            startingCounty.countyData.armiesInCountyList.Remove(token.countyPopulation);
            startingCounty.countyData.visitingHeroList.Remove(token.countyPopulation);
            startingCounty.countyData.visitingArmyList.Remove(token.countyPopulation);
        }

        private void HeroReachedCounty()
        {
            token.spawnedTokenButton.Reparent(destinationCounty.heroesHBox);
            destinationCounty.countyData.herosInCountyList.Add(token.countyPopulation);
        }

        private void ArmyReachedCounty()
        {
            token.spawnedTokenButton.Reparent(destinationCounty.armiesHBox);
            destinationCounty.countyData.armiesInCountyList.Add(token.countyPopulation);
        }
        private void HeroVisitingCounty()
        {
            token.spawnedTokenButton.Reparent(destinationCounty.heroesHBox);
            destinationCounty.countyData.visitingHeroList.Add(token.countyPopulation);
        }

        private void ArmyVisitingCounty()
        {
            token.spawnedTokenButton.Reparent(destinationCounty.armiesHBox);
            destinationCounty.countyData.visitingArmyList.Add(token.countyPopulation);
        }
    }
}