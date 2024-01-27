using Godot;
using System;

namespace PlayerSpace
{
    public partial class TokenMovement : Node2D
    {
        [Export] private CharacterBody2D token;
        [Export] private bool moveToken;

        public bool MoveToken
        {
            get { return moveToken; }
            set
            {
                moveToken = value;
                if (moveToken == true)
                {
                    SelectToken selectToken = (SelectToken)GetParent();
                    selectToken.stackCountLabel.Hide();
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

        private void Move()
        {
            Vector2 target = Globals.Instance.heroMoveTarget;
            token.GlobalPosition = GlobalPosition.MoveToward(target, Globals.Instance.movementSpeed * Clock.Instance.ModifiedTimeScale);
            //GD.Print($"{GetParent().Name} is moving!");
            if (token.GlobalPosition.IsEqualApprox(target))
            {
                ReachedDestination();
                MoveToken = false;

            }
        }

        private void ReachedDestination()
        {
            SelectToken selectToken = (SelectToken)GetParent();
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(selectToken.countyPopulation.destination);

            selectToken.countyPopulation.currentActivity = AllText.Activities.IDLE;
            // Are you at war with the owner of the county the token just arrived at?
            if(selectToken.countyPopulation.isArmyLeader == false)
            {
                AddToTokenStacker();
            }
            else
            {
                selectCounty.countyData.visitingPopulation.Add(selectToken.countyPopulation);

                foreach (War war in Globals.Instance.playerFactionData.wars)
                {
                    if (selectCounty.countyData.factionData == war.defenderFactionData
                        || selectCounty.countyData.factionData == war.attackerFactionData)
                    {
                        
                        selectToken.countyPopulation.location = selectCounty.countyData.countyId;

                        GD.Print("New Battle!");
                        Battle battle = new(selectCounty.countyData);
                        selectCounty.countyData.battles.Add(battle);
                        selectCounty.heroTokensControl.StartBattle();
                    }
                    else
                    {
                        GD.Print("No Battle!");
                    }
                }
            }            
        }

        private void AddToTokenStacker()
        {
            /*
            GD.Print(token.Name + " got to destination!");
            moveToken = false;
            SelectToken selectToken = (SelectToken)GetParent();
            CountyPopulation countyPopulation = selectToken.countyPopulation;

            // Add them to their destination list and move them to the Hero Spawn Location Node2D corresponding to the County.
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.destination);

            // Check to see if they are in friendly territory or not.  If not then join the visiting list.
            if(countyPopulation.factionData == selectCounty.countyData.factionData)
            {
                selectCounty.countyData.heroCountyPopulation.Add(countyPopulation);
            }
            else
            {
                selectCounty.countyData.visitingPopulation.Add(countyPopulation);                
            }

            countyPopulation.token.Reparent(selectCounty.heroSpawn);

            GD.Print($"{countyPopulation.firstName} is in {selectCounty.countyData.countyName}");

            // Change their current location to what was their destination. This has to be above the list insert.
            countyPopulation.location = selectCounty.countyData.countyId;
            selectToken.GlobalPosition = selectCounty.heroSpawn.GlobalPosition;


            // Refresh the list of heroes beneath the CountyInfo Panel.
            CountyInfoControl.Instance.GenerateHeroesPanelList();

            // Add to the spawnedHeroList when it gets to its destination.
            selectCounty.heroSpawn.spawnedTokenList.Insert(0, selectToken);
            */
        }
    }
}