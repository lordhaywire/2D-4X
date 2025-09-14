using System;
using System.Collections.Generic;
using AutoloadSpace;
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
                    heroToken.populationData.lastLocation = heroToken.populationData.Location;
                    heroToken.Show();
                    FactionData factionData =
                        SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(heroToken.populationData
                            .factionId);
                    if (Globals.CheckIfPlayerFaction(factionData) == false)
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
            GD.Print($"{heroToken.populationData.firstName} has location of {heroToken.populationData.Location}");
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
            //CheckForDefenders();
            CheckIfRetreating();

            MoveToken = true;
        }

        /*
        private void CheckForDefenders()
        {
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(heroToken.populationData.destination);
            FactionData heroFactionData = SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(heroToken.populationData.factionId);
            FactionData selectedFactionData = SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(selectCounty.countyData.factionId);
            if (Diplomacy.IsFactionAtWar(heroFactionData, selectedFactionData) && DefenderOnTheWay() == false)
            {
                Diplomacy.CheckForAndSpawnDefendingHero(destinationCounty);

            }
            else
            {
                GD.Print("Checking for Defenders - Defender on the way, or not at war.");
            }
        }
        */

        private bool DefenderOnTheWay()
        {
            // Get the All Heroes List in the destination county for that county's faction and see if any of that
            // faction heroes are on the way to it.
            GD.Print("Seeing if someone is on the way.");
            FactionData factionData =
                SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(destinationCounty.countyData.factionId);
            foreach (KeyValuePair<int, int> keyValuePair in factionData.allHeroesDictionary)
            {
                County county = (County)Globals.Instance.countiesParent.GetChild(keyValuePair.Value);
                PopulationData populationData =
                    PopulationData.ReturnPopulationDataFromPopulationId(county.countyData.heroesInCountyList,
                        keyValuePair.Key);
                if (populationData.destination != heroToken.populationData.destination) continue;
                GD.Print("Hero on the way is: " + populationData.firstName);
                return true;
            }

            GD.Print("Hero NOT on the way.");
            return false;
        }

        private void CheckIfRetreating()
        {
            County county = (County)Globals.Instance.countiesParent.GetChild(heroToken.populationData.Location);

            // Check to see if the starting county has battles and if it does, it should end the battle because the
            // token is retreating.
            if (county.countyData.battles.Count > 0)
            {
                county.battleControl.EndBattle();
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
            FactionData factionData =
                SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(destinationCounty.countyData.factionId);
            GD.Print("Faction of Destination County: " + factionData.factionName);
            // Checking to see if the hero is in a friendly county.
            if (destinationCounty.countyData.factionId == heroToken.populationData.factionId)
            {
                HeroReachedFriendlyCounty();
            }
            else
            {
                if (Diplomacy.IsFactionAtWar(FactionData.GetFactionDataFromId(heroToken.populationData.factionId)
                        , FactionData.GetFactionDataFromId(destinationCounty.countyData.factionId)))
                {
                    HeroAttackingCounty();
                    heroToken.populationData.UpdateActivity(AllEnums.Activities.Combat);
                }
                else
                {
                    // We will probably need to change this to what the token occupation does.
                    // For example, if the token is a diplomat, then the activity will be diplomacy.
                    HeroVisitingCounty();
                    heroToken.populationData.UpdateActivity(AllEnums.Activities.Idle);

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

        private void HeroReachedEnemyCounty()
        {
            heroToken.spawnedTokenButton.Reparent(destinationCounty.armiesHBox);
            destinationCounty.countyData.visitingHeroList.Add(heroToken.populationData);
            Recruiter.UpdateRecruitingActivity(heroToken.populationData);
        }

        private void HeroAttackingCounty()
        {
            HeroArmyVisitingEnemyCounty();

            if (destinationCounty.countyData.heroesInCountyList.Count > 0)
            {
                Battle battle = new(destinationCounty.countyData);
                destinationCounty.countyData.battles.Add(battle);
                destinationCounty.battleControl.StartBattle(battle);
            }
            else
            {
                FactionData factionData =
                    SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(heroToken.populationData.factionId);
                CountyDictator.Instance.CaptureCounty(heroToken.populationData.destination,
                    factionData);
            }
        }

        private void HeroArmyVisitingEnemyCounty()
        {
            heroToken.spawnedTokenButton.Reparent(destinationCounty.armiesHBox);
            destinationCounty.countyData.visitingArmyList.Add(heroToken.populationData);
        }

        private void HeroReachedFriendlyCounty()
        {
            heroToken.spawnedTokenButton.Reparent(destinationCounty.heroesHBox);
            destinationCounty.countyData.heroesInCountyList.Add(heroToken.populationData);
            Recruiter.UpdateRecruitingActivity(heroToken.populationData);
        }

        private void HeroVisitingCounty()
        {
            heroToken.spawnedTokenButton.Reparent(destinationCounty.heroesHBox);
            destinationCounty.countyData.visitingHeroList.Add(heroToken.populationData);
        }
    }
}