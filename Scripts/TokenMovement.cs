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
            token.GlobalPosition = GlobalPosition.MoveToward(target, Globals.Instance.movementSpeed);
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
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);

            selectCounty.countyData.heroCountyPopulation.Remove(countyPopulation);
            selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.destination);
            selectCounty.countyData.heroCountyPopulation.Add(countyPopulation);
            GD.Print($"{countyPopulation.firstName} is in {selectCounty.countyData.countyName}");

            CountyInfoControl.Instance.GenerateHeroesPanelList();
        }
    }
}