using Godot;

namespace PlayerSpace
{
    public partial class AllCountyResources : Node
    {
        public static AllCountyResources Instance { get; private set; }

        [Export] public CountyResourceData[] allResources;

        public override void _Ready()
        {
            Instance = this;
            CountResources();
        }

        private void CountResources()
        {
            int perishable = 0;
            int nonperishable = 0;

            foreach (CountyResourceData resourceData in allResources)
            {
                if (resourceData.perishable)
                {
                    perishable++;
                }
                else
                {
                    nonperishable++;
                }
            }
            Globals.Instance.numberOfPerishableResources = perishable;
            Globals.Instance.numberOfNonperishableResources = nonperishable;
        }
    }
}