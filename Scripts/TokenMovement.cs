using Godot;

namespace PlayerSpace
{
    public partial class TokenMovement : Node2D
    {
        [Export] private CharacterBody2D token;
        [Export] public bool move;

        public override void _PhysicsProcess(double delta)
        {
            if (move == true)
            {
                Move();
            }
        }

        private void Move()
        {
            Vector2 target = Globals.Instance.heroMoveTarget;
            token.GlobalPosition = GlobalPosition.MoveToward(target, Globals.Instance.movementSpeed * Clock.Instance.ModifiedTimeScale);
            if (GlobalPosition.DistanceTo(target) < 0.001f)
            {
                ReachedDestination();
            }
        }

        private void ReachedDestination()
        {
            GD.Print(token.Name + " got to destination!");
            move = false;
            SelectToken selectToken = (SelectToken)GetParent();
            CountyPopulation countyPopulation = selectToken.countyPopulation;

            // Remove them from their starting location list.
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
            selectCounty.countyData.heroCountyPopulation.Remove(countyPopulation);
            countyPopulation.token.GetParent().RemoveChild(countyPopulation.token);

            // Add them to their destination list and move them to the Hero Spawn Location Node2D corresponding to the County.
            selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.destination);
            selectCounty.countyData.heroCountyPopulation.Add(countyPopulation);
            selectCounty.heroSpawn.AddChild(countyPopulation.token);
            countyPopulation.token.GlobalPosition = selectCounty.heroSpawn.GlobalPosition;
            countyPopulation.location = selectCounty.countyData.countyID;
            GD.Print($"{countyPopulation.firstName} is in {selectCounty.countyData.countyName}");

            // Refresh the list of heroes beneath the CountyInfo Panel.
            CountyInfoControl.Instance.GenerateHeroesPanelList();
        }
    }
}