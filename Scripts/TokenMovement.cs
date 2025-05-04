using Godot;

namespace PlayerSpace
{
    public partial class TokenMovement : Node2D
    {
        [Export] public HeroToken heroToken;
        [Export] private bool moveToken;
        private Vector2 target;
        private County destinationCounty;

        public bool MoveToken
        {
            get => moveToken;
            private set
            {
                moveToken = value;
                if (moveToken)
                {
                    heroToken.populationData.lastLocation = heroToken.populationData.location;
                    heroToken.Show();
                    if (Globals.Instance.playerFactionData != heroToken.populationData.factionData)
                    {
                        return;
                    }

                    CountyInfoControl.Instance.UpdateEverything();

                }
                
                if (PlayerUICanvas.Instance.selectedHeroPanelContainer.populationData.heroToken == heroToken)
                {
                    CountyInfoControl.UpdateSelectedHero();
                }
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (MoveToken)
            {
                Move();
            }
        }

        public void StartMove(int destinationCountyId)
        {
            GD.Print($"{heroToken.populationData.firstName} has location of {heroToken.populationData.location}");
            destinationCounty
                = (County)Globals.Instance.countiesParent.GetChild(destinationCountyId);

            heroToken.populationData.destination = destinationCountyId;

            // If a subordinate is in recruited activity and getting their shit together, then they don't move with the hero.
            Recruiter.FireSubordinatesInRecruitedActivity(heroToken.populationData);
            
            // Remove a hero from research
            heroToken.RemoveFromResearch();

            // Remove the hero's current county improvement.
            heroToken.populationData.UpdateCurrentCountyImprovement(null);
            heroToken.populationData.UpdateActivity(AllEnums.Activities.Move);

            //GD.Print("Destination Global Position: " + destinationCounty.heroSpawn.GlobalPosition);
            target = destinationCounty.heroSpawn.GlobalPosition;
            CheckForDefenders();
            CheckIfRetreating();

            MoveToken = true;
        }

        private void CheckForDefenders()
        {
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(heroToken.populationData.destination);

            if (heroToken.populationData.factionData.factionWarDictionary
                    [selectCounty.countyData.factionData.factionName] && DefenderOnTheWay() == false)
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
            // Get the All Heroes List in the destination county for that county's faction and see if any of that
            // faction heroes are on the way to it.
            //GD.Print("Seeing if someone is on the way.");
            foreach (PopulationData populationData in destinationCounty.countyData.factionData.allHeroesList)
            {
                if (populationData.destination != heroToken.populationData.destination) continue;
                GD.Print("Hero on the way is: " + populationData.firstName);
                return true;
            }

            return false;
        }

        private void CheckIfRetreating()
        {
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(heroToken.populationData.location);

            // Check to see if the starting county has battles and if it does, it should end the battle because the
            // token is retreating.
            if (selectCounty.countyData.battles.Count > 0)
            {
                selectCounty.battleControl.EndBattle();
            }
        }

        private void Move()
        {
            float speed = Globals.Instance.movementSpeed * Clock.Instance.TimeMultiplier;
            heroToken.GlobalPosition = GlobalPosition.MoveToward(target, speed);
            if (heroToken.GlobalPosition.IsEqualApprox(target))
            {
                ReachedDestination();
            }
        }

        private void ReachedDestination()
        {
            MoveToken = false;
            GD.Print("Top of Reached Destination County Population: " + heroToken.populationData.firstName);
            GD.Print("Token Destination: " + heroToken.populationData.destination);
            destinationCounty = (County)Globals.Instance.countiesParent.GetChild(heroToken.populationData.destination);
            GD.Print("Faction of Destination County: " + destinationCounty.countyData.factionData.factionName);
            // Checking to see if the hero is in a friendly county.
            if (destinationCounty.countyData.factionData == heroToken.populationData.factionData)
            {
                if (heroToken.populationData.IsThisAnArmy() == false)
                {
                    HeroReachedCounty();
                }
                else
                {
                    ArmyReachedCounty();
                }
            }
            else
            {
                if (heroToken.populationData.IsThisAnArmy() == false)
                {
                    HeroVisitingCounty();
                    // We will probably need to change this to what the token occupation does.
                    // For example, if the token is a diplomat, then the activity will be diplomacy.
                    heroToken.populationData.UpdateActivity(AllEnums.Activities.Idle);
                }
                else
                {
                    ArmyAttackingCounty();
                    heroToken.populationData.UpdateActivity(AllEnums.Activities.Combat);
                }
            }
            
            heroToken.RemoveHeroAndSubordinatesFromStartingCounty(); // Move to Hero Token at some point.
            
            heroToken.AddHeroAndSubordinatesToDestinationCounty(destinationCounty); // Move to Hero Token at some point.

            CountyInfoControl.Instance.UpdateEverything();
            heroToken.populationData.destination = -1; // This is -1 because this is like a "null" int.
            heroToken.spawnedTokenButton.UpdateTokenTextures();
            heroToken.isRetreating = false;
            heroToken.Hide();
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
                CountyDictator.Instance.CaptureCounty(heroToken.populationData.destination,
                    heroToken.populationData.factionData);
            }
        }

        private void HeroReachedCounty()
        {
            heroToken.spawnedTokenButton.Reparent(destinationCounty.heroesHBox);
            destinationCounty.countyData.heroesInCountyList.Add(heroToken.populationData);
            Recruiter.CheckForRecruitingActivity(heroToken.populationData);
        }

        private void ArmyReachedCounty()
        {
            heroToken.spawnedTokenButton.Reparent(destinationCounty.armiesHBox);
            destinationCounty.countyData.armiesInCountyList.Add(heroToken.populationData);
            Recruiter.CheckForRecruitingActivity(heroToken.populationData);
        }

        private void HeroVisitingCounty()
        {
            heroToken.spawnedTokenButton.Reparent(destinationCounty.heroesHBox);
            destinationCounty.countyData.visitingHeroList.Add(heroToken.populationData);
        }

        private void ArmyVisitingCounty()
        {
            heroToken.spawnedTokenButton.Reparent(destinationCounty.armiesHBox);
            destinationCounty.countyData.visitingArmyList.Add(heroToken.populationData);
        }
    }
}