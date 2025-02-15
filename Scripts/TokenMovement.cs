using Godot;

namespace PlayerSpace
{
    public partial class TokenMovement : Node2D
    {
        [Export] public HeroToken token;
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
                    token.populationData.lastLocation = token.populationData.location;
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
            GD.Print($"{token.populationData.firstName} has location of {token.populationData.location}");
            destinationCounty
                = (County)Globals.Instance.countiesParent.GetChild(destinationCountyID);

            token.populationData.destination = destinationCountyID;

            // Remove hero from research
            token.RemoveFromResearch();

            // Remove the hero's current county improvement.
            token.populationData.UpdateCurrentCountyImprovement(null);
            token.populationData.UpdateActivity(AllEnums.Activities.Move);

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
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(token.populationData.destination);

            if (token.populationData.factionData.factionWarDictionary
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
            foreach (PopulationData populationData in destinationCounty.countyData.factionData.allHeroesList)
            {
                if (populationData.destination == token.populationData.destination)
                {
                    GD.Print("Hero on the way is: " + populationData.firstName);
                    return true;
                }
            }
            return false;
        }

        private void CheckIfRetreating()
        {
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(token.populationData.location);

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
            GD.Print("Top of Reached Destination County Population: " + token.populationData.firstName);
            GD.Print("Token Destination: " + token.populationData.destination);
            destinationCounty = (County)Globals.Instance.countiesParent.GetChild(token.populationData.destination);
            GD.Print("Faction of Destination County: " + destinationCounty.countyData.factionData.factionName);
            if (destinationCounty.countyData.factionData == token.populationData.factionData)
            {
                if (token.populationData.IsThisAnArmy() == false)
                {
                    HeroReachedCounty();
                    token.populationData.UpdateActivity(AllEnums.Activities.Idle);
                }
                else
                {
                    ArmyReachedCounty();
                    token.populationData.UpdateActivity(AllEnums.Activities.Idle);
                }
            }
            else
            {
                if (token.populationData.IsThisAnArmy() == false)
                {
                    HeroVisitingCounty();
                    // We will probably need to change this to what the token occupation does.
                    // For example, if the token is a diplomat, then the activity will be diplmating.
                    token.populationData.UpdateActivity(AllEnums.Activities.Idle);
                }
                else
                {
                    ArmyAttackingCounty();
                    token.populationData.UpdateActivity(AllEnums.Activities.Combat);
                }

            }
            RemoveFromStartingCounty();
            AddToDestinationCounty();
            CountyInfoControl.Instance.UpdateEverything();
            token.populationData.destination = -1; // This is -1 because this is like a "null" int.
            token.spawnedTokenButton.UpdateTokenTextures();
            token.isRetreating = false;
            token.Hide();
        }

        private void ArmyAttackingCounty()
        {
            ArmyVisitingCounty();
            if (destinationCounty.countyData.armiesInCountyList.Count > 0)
            {
                Battle battle = new(destinationCounty.countyData);
                destinationCounty.countyData.battles.Add(battle);
                destinationCounty.battleControl.StartBattle(battle);
            }
            else
            {
                CountyDictator.Instance.CaptureCounty(token.populationData.destination, token.populationData.factionData);
            }
        }

        private void AddToDestinationCounty()
        {
            //GD.Print("Add To Destination County " + token.populationData.firstName);
            token.populationData.location = destinationCounty.countyData.countyId;
            destinationCounty.countyData.spawnedTokenButtons.Add(token.spawnedTokenButton);
        }
        private void RemoveFromStartingCounty()
        {
            //GD.Print($"Removed token from Starting County {token.populationData.firstName} {token.populationData.location}");
            //GD.Print($"Removed {token.populationData.firstName} {token.populationData.factionData.factionName}");
            // Remove populationData from the heroes starting county location list.
            County startingCounty = (County)Globals.Instance.countiesParent.GetChild(token.populationData.location);

            // We don't need to check which list the hero is in because C# doesn't give a shit if the hero isn't in the list.
            // So we just try to remove it from both and it will remove it from the correct one.
            startingCounty.countyData.heroesInCountyList.Remove(token.populationData);
            startingCounty.countyData.armiesInCountyList.Remove(token.populationData);
            startingCounty.countyData.visitingHeroList.Remove(token.populationData);
            startingCounty.countyData.visitingArmyList.Remove(token.populationData);

            startingCounty.countyData.spawnedTokenButtons.Remove(token.spawnedTokenButton);
        }

        private void HeroReachedCounty()
        {
            token.spawnedTokenButton.Reparent(destinationCounty.heroesHBox);
            destinationCounty.countyData.heroesInCountyList.Add(token.populationData);
        }

        private void ArmyReachedCounty()
        {
            token.spawnedTokenButton.Reparent(destinationCounty.armiesHBox);
            destinationCounty.countyData.armiesInCountyList.Add(token.populationData);
        }
        private void HeroVisitingCounty()
        {
            token.spawnedTokenButton.Reparent(destinationCounty.heroesHBox);
            destinationCounty.countyData.visitingHeroList.Add(token.populationData);
        }

        private void ArmyVisitingCounty()
        {
            token.spawnedTokenButton.Reparent(destinationCounty.armiesHBox);
            destinationCounty.countyData.visitingArmyList.Add(token.populationData);
        }
    }
}